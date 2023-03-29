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
        // �ϐ��� #####################################################################


        // �e���[�h��\���萔�l������enum
        public enum MODE
        {
            CodeGeneration,  // �R�[�h�̐���
            OtherGeneration, // ���̑��R�[�h�̐���
            CodeReview,   // �R�[�h�̃��r���[
            Comment,      // �R�����g
            NamedMethod,  // ���O�t�����\�b�h
            FreeChat,     // �`���b�g
        }

        // �������[�h��CodeExecute�ɐݒ�
        public MODE _mode = MODE.CodeGeneration;

        // �������ꂽ�R�[�h
        private string _code = default;

        // API��URL
        private const string _url = "https://api.openai.com/v1/completions";


        #region ���N�G�X�g�Q

        // ���f����
        [HideInInspector]
        public string _modelName { get; set;} = "text-davinci-003";

        // �v����
        private string _prompt { get; set;} = default;

        #region �ڍאݒ�i�����񐄏��j

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


        [SerializeField, Tooltip("�����ɔ��s����API�L�[����͂���")]
        private string _apiKey = "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";

        #region �����l

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

        // API�ʐM�����ۂ��𔻒f����bool�^�ϐ�
        private bool _isRunning = false;


        // ���\�b�h��##################################################

        /// <summary>
        /// ���s��ԂɂȂ�����false�ɂ���
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
        /// �t�@�C���𐶐�
        /// </summary>
        public void Generate()
        {

            // C#�t�@�C��������B
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
        /// ����t�@�C���𐶐�
        /// </summary>
        /// <param name="extention">�t�@�C���g���q</param>
        private void MakeFile(string extention)
        {
            string l_filePath = "Assets/" + _className + extention;
            File.WriteAllText(l_filePath, _code);
            AssetDatabase.Refresh();
        }



        /// <summary>
        /// ChatGPT�Ƀ��N�G�X�g�𑗂�A���X�|���X��Ⴄ
        /// </summary>
        public void Execute()
        {
            if( _isRunning)
            {
                Debug.Log("���łɉғ���");
                return;
            }

            _isRunning = true;

            float l_progress = 0.0f;
            EditorUtility.DisplayProgressBar("API���N�G�X�g��", "���N�G�X�g���M��...", l_progress);

            // �v�����̐��K��
            switch (_mode)
            {
                case MODE.CodeGeneration:
                    _prompt = "Unity��" + _wantTodo + "����X�N���v�g�������Ă��������B" +
                              "�N���X����" + _className + "�ŁBusing����L�q���Ă��������B" +
                              "MonoBehaviour���p�����Ă��������B" +
                              "����̃R���|�[�l���g���K�v�ȏꍇ�́ARequireComponent���N���X�ɂ��Ă��������B" +
                              "�ԓ��̓R�[�h�݂̂ł��肢���܂��B";
                    break;
                case MODE.OtherGeneration:
                    _prompt = _wantTodo + "����R�[�h�������Ă��������B" +
                              "����͊g���q"+_extension + "�̌���ŋL�q���邱��"+
                              "�t�@�C���̖��O��" + _className + "�Ő������邱��" +
                              "�ԓ��̓R�[�h�݂̂ł��肢���܂��B";
                    break;
                case MODE.CodeReview:
                    _prompt = "Unity�̃R�[�h" + _wantTodo + "���A" +
                              "�K��A" + _rules + "������" +
                              "���r���[�������e��ǂ��������ƈ������������ꂼ��ӏ������ŃR�����g������A" +
                              "������C�������R�[�h��ԓ����Ă��������B";
                    break;
                case MODE.Comment:
                    _prompt = "Unity�̃R�[�h" + _wantTodo + "�ɁA" +
                              "�R�����g�����Ă��������B" +
                              "�ԓ��̓R�����g�t���̃X�N���v�g�R�[�h�݂̂ł��肢���܂��B";
                    break;
                case MODE.NamedMethod:
                    _prompt = "Unity�̏���" + _wantTodo + "�ɁA" + 
                              "���\�b�h�������Ă�������" +
                              "�ԓ��́A�l�������\�b�h���݂̂ł��肢���܂�";
                    break;
                case MODE.FreeChat:
                    _prompt = _wantTodo;
                    break;
                default:
                    break;
            }


            // ���N�G�X�g�f�[�^
            RequestData l_requestData = new RequestData()
            {
                // GPT-3�̒�����g�p���郂�f�����w�肷��p�����[�^�[
                model = _modelName,

                // ���f���ɗ^����e�L�X�g�̏������͂��w�肷��p�����[�^�[
                prompt = _prompt,

                /* 
                 * �������ꂽ���͂̑��l���𒲐�����p�����[�^�[
                 * �l���������قǐ������ꂽ���͂͂�萧��I�ɂȂ�A
                 * �傫���قǂ�莩�R�ȕ��͂�����
                 */
                temperature = _temperature,

                /*
                 * ��x�ɐ�������ő�g�[�N�������w�肷��p�����[�^�[
                 * �g�[�N�����������قǐ�������镶�͂͒����Ȃ�
                 */
                max_tokens = _max_tokens,

                /*
                 * �g�[�N���̃T���v�����O�ɂ����āA
                 * �ł��p�x�̍����g�[�N���̗ݐϊm�����w�肷��p�����[�^�[
                 * �l���������قǁA��萧�����ꂽ���͂���������܂�
                 */
                top_p = _top_p,

                /*
                 * �p�x�̍����P�ꂪ�o�����ɂ����Ȃ�悤�Ƀy�i���e�B��^����p�����[�^�[
                 * �l���傫���قǁA���p�x�̍����P�ꂪ�o�����ɂ����Ȃ�
                 */
                frequency_penalty = _frequencyPenalty,

                /*
                 * �ȑO�ɐ������ꂽ�e�L�X�g�ɏo�������P�ꂪ
                 * �o�����ɂ����Ȃ�悤�Ƀy�i���e�B��^����p�����[�^�[
                 * �l���傫���قǁA�ȑO�ɏo�������P�ꂪ�o�����ɂ����Ȃ�
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
                EditorUtility.DisplayProgressBar("API���N�G�X�g��", "���N�G�X�g���M��...", l_progress);
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
                        Debug.LogError("API�L�[�������؂�A��������API�L�[�������ɂȂ��Ă���\��������܂�");
                    }

                }

                EditorUtility.ClearProgressBar();

                _isRunning = false;
            };

        }


        /// <summary>
        /// API�̃��[�h
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

