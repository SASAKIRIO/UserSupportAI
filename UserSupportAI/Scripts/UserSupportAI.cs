using System;
using System.Collections;
using System.IO;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityLibrary
{
    public class UserSupportAI : MonoBehaviour
    {
        // 変数部 #####################################################################


        // 各モードを表す定数値を示すenum
        public enum MODE
        {
            CodeGeneration,  // コードの生成
            OtherGeneration, // その他コードの生成
            CodeReview,   // コードのレビュー
            Comment,      // コメント
            NamedMethod,  // 名前付きメソッド
            FreeChat,     // チャット
        }

        // 初期モードをCodeExecuteに設定
        public MODE _mode = MODE.CodeGeneration;

        // 生成されたコード
        private string _code = default;

        // APIのURL
        private const string _url = "https://api.openai.com/v1/completions";


        #region リクエスト群

        // モデル名
        [HideInInspector]
        public string _modelName { get; set;} = "text-davinci-003";

        // 要求文
        private string _prompt { get; set;} = default;

        #region 詳細設定（調整非推奨）

        [HideInInspector]
        public float _temperature = 0.7f;

        [HideInInspector]
        public int _max_tokens = 1000;

        private int _top_p = 1;

        [HideInInspector]
        public int _frequencyPenalty = 0;

        [HideInInspector]
        public int _presencePenalty = 0;

        #endregion

        #endregion


        [SerializeField, Tooltip("ここに発行したAPIキーを入力する")]
        private string _apiKey = "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";

        #region 調整値

        [HideInInspector]
        public string _wantTodo { get; set; } = default;

        [HideInInspector] 
        public string _className { get; set; } = default;

        [HideInInspector]
        public string _inputResults { get; set; } = default;

        [HideInInspector] 
        public string _rules { get; set; } = default;

        [HideInInspector]
        public string _extension { get; set; } = default;

        #endregion

        // API通信中か否かを判断するbool型変数
        private bool _isRunning = false;


        // メソッド部##################################################

        /// <summary>
        /// 実行状態になったらfalseにする
        /// </summary>
        private void OnEnable()
        {
            UserSupportAI userSupportAI = GetComponent<UserSupportAI>();
            userSupportAI.enabled = false;
        }



        private void Start()
        {
            LoadAPIKey();
        }


        /// <summary>
        /// ファイルを生成
        /// </summary>
        public void Generate()
        {

            // C#ファイルをつくる。
            switch (_mode)
            {
                case MODE.CodeGeneration:
                    MakeFile(".cs");
                    break;
                case MODE.OtherGeneration:
                    MakeFile("."+_extension);
                    break;
                case MODE.CodeReview:
                    break;
                case MODE.Comment:
                    break;
                case MODE.NamedMethod:
                    break;
                case MODE.FreeChat:
                    break;
                default:
                    break;
            }

        }


        /// <summary>
        /// 特定ファイルを生成
        /// </summary>
        /// <param name="extention">ファイル拡張子</param>
        private void MakeFile(string extention)
        {
            string l_filePath = "Assets/" + _className + extention;
            File.WriteAllText(l_filePath, _code);
            AssetDatabase.Refresh();
        }



        /// <summary>
        /// ChatGPTにリクエストを送り、レスポンスを貰う
        /// </summary>
        public void Execute()
        {
            if( _isRunning)
            {
                Debug.Log("すでに稼働中");
                return;
            }

            _isRunning = true;

            float l_progress = 0.0f;
            EditorUtility.DisplayProgressBar("APIリクエスト中", "リクエスト送信中...", l_progress);

            // 要求文の正規化
            switch (_mode)
            {
                case MODE.CodeGeneration:
                    _prompt = "Unityで" + _wantTodo + "するスクリプトを書いてください。" +
                              "クラス名は" + _className + "で。usingから記述してください。" +
                              "MonoBehaviourを継承してください。" +
                              "特定のコンポーネントが必要な場合は、RequireComponentをクラスにつけてください。" +
                              "返答はコードのみでお願いします。";
                    break;
                case MODE.OtherGeneration:
                    _prompt = _wantTodo + "するコードを書いてください。" +
                              "言語は拡張子"+_extension + "の言語で記述すること"+
                              "ファイルの名前は" + _className + "で生成すること" +
                              "返答はコードのみでお願いします。";
                    break;
                case MODE.CodeReview:
                    _prompt = "Unityのコード" + _wantTodo + "を、" +
                              "規約、" + _rules + "を元に" +
                              "レビューした内容を良かった所と悪かった所それぞれ箇条書きでコメントした後、" +
                              "それを修正したコードを返答してください。";
                    break;
                case MODE.Comment:
                    _prompt = "Unityのコード" + _wantTodo + "に、" +
                              "コメントをつけてください。" +
                              "返答はコメント付きのスクリプトコードのみでお願いします。";
                    break;
                case MODE.NamedMethod:
                    _prompt = "Unityの処理" + _wantTodo + "に、" + 
                              "メソッド名をつけてください" +
                              "返答は、考えたメソッド名のみでお願いします";
                    break;
                case MODE.FreeChat:
                    _prompt = _wantTodo;
                    break;
                default:
                    break;
            }


            // リクエストデータ
            RequestData l_requestData = new RequestData()
            {
                // GPT-3の中から使用するモデルを指定するパラメーター
                model = _modelName,

                // モデルに与えるテキストの初期入力を指定するパラメーター
                prompt = _prompt,

                /* 
                 * 生成された文章の多様性を調整するパラメーター
                 * 値が小さいほど生成された文章はより制約的になり、
                 * 大きいほどより自由な文章が生成
                 */
                temperature = _temperature,

                /*
                 * 一度に生成する最大トークン数を指定するパラメーター
                 * トークン数が多いほど生成される文章は長くなる
                 */
                max_tokens = _max_tokens,

                /*
                 * トークンのサンプリングにおいて、
                 * 最も頻度の高いトークンの累積確率を指定するパラメーター
                 * 値が小さいほど、より制限された文章が生成されます
                 */
                top_p = _top_p,

                /*
                 * 頻度の高い単語が出現しにくくなるようにペナルティを与えるパラメーター
                 * 値が大きいほど、より頻度の高い単語が出現しにくくなる
                 */
                frequency_penalty = _frequencyPenalty,

                /*
                 * 以前に生成されたテキストに出現した単語が
                 * 出現しにくくなるようにペナルティを与えるパラメーター
                 * 値が大きいほど、以前に出現した単語が出現しにくくなる
                 */
                presence_penalty = _presencePenalty,
            };

            string l_jsonData = JsonUtility.ToJson(l_requestData);

            byte[] l_postData = System.Text.Encoding.UTF8.GetBytes(l_jsonData);

            UnityWebRequest l_request = UnityWebRequest.Post(_url, l_jsonData);

            l_request.uploadHandler = new UploadHandlerRaw(l_postData);
            l_request.downloadHandler = new DownloadHandlerBuffer();
            l_request.SetRequestHeader("Content-Type", "application/json");
            l_request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            UnityWebRequestAsyncOperation l_async = l_request.SendWebRequest();



            while (!l_async.isDone)
            {
                l_progress = l_async.progress;
                EditorUtility.DisplayProgressBar("APIリクエスト中", "リクエスト送信中...", l_progress);
            }



            l_async.completed += (op) =>
            {
                if (l_request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(l_request.error);
                }
                else
                {
                    //Debug.Log(l_request.downloadHandler.text);
                    try
                    {
                        OpenAIAPI _responseData = JsonUtility.FromJson<OpenAIAPI>(l_request.downloadHandler.text);
                        string l_generatedText = _responseData.choices[0].text.TrimStart('\n').TrimStart('\n');
                        _inputResults = l_generatedText;
                        _code = _inputResults;
                        Generate();
                    }
                    catch
                    {
                        Debug.LogError("APIキーが期限切れ、もしくはAPIキーが無効になっている可能性があります");
                    }

                }

                EditorUtility.ClearProgressBar();

                _isRunning = false;
            };

        }


        /// <summary>
        /// APIのロード
        /// </summary>
        public void LoadAPIKey()
        {
            string l_keyPath = Path.Combine(Application.streamingAssetsPath, "seacretKey.txt");

            if (!File.Exists(l_keyPath))
            {
                Debug.LogError("Apikey missing:" + l_keyPath);
            }
            _apiKey = File.ReadAllText(l_keyPath).Trim();
            Debug.Log("API key loaded, len =" + _apiKey.Length);
        }
    }
}

