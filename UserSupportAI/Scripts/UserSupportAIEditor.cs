using UnityEditor;
using UnityEngine;
using UnityLibrary;

[CustomEditor(typeof(UserSupportAI))]
public class UserSupportAIEditor : Editor
{

    // 詳細設定ドロップ用bool変数
    bool _advanceSettingfoldOut;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UserSupportAI _myScript = (UserSupportAI)target;

        string label_wantTodo = null;
        string label_className = null;
        string label_inputResults = null;
        string label_button = null;

        // コード規約を入力する所
        string label_rules = null;

        // その他コード生成に使う、拡張子を入力する所
        string label_extension = null;

        if (_myScript._mode==UserSupportAI.MODE.CodeGeneration)
        {
            label_wantTodo = "ここに実装したい内容を記述";
            label_className = "ここにクラス名を記述する";
            label_inputResults = "結果";
            label_button = "生成";
        }
        else if(_myScript._mode == UserSupportAI.MODE.OtherGeneration)
        {
            label_wantTodo = "ここに実装したいコード内容を記述";
            label_extension = "ここに拡張子を記述する(.(ドット)不要)";
            label_className = "ここにファイル名を記述する";
            label_inputResults = "結果";
            label_button = "生成";
        }
        else if(_myScript._mode == UserSupportAI.MODE.CodeReview)
        {
            label_wantTodo = "レビューしてほしいスクリプトを入力";
            label_rules = "修正の元にしてほしい規約";
            label_inputResults = "結果";
            label_button = "コードをレビュー";
        }
        else if(_myScript._mode == UserSupportAI.MODE.Comment)
        {
            label_wantTodo = "コメントを付けてほしい処理を入力";
            label_inputResults = "結果";
            label_button = "コメントを付ける";
        }
        else if(_myScript._mode == UserSupportAI.MODE.NamedMethod)
        {
            label_wantTodo = "メソッド内容を入力";
            label_inputResults = "結果";
            label_button = "この処理に名前をつける";
        }
        else if (_myScript._mode == UserSupportAI.MODE.FreeChat)
        {
            label_wantTodo = "入力";
            label_inputResults = "結果";
            label_button = "ChatGPTに質問する";
        }

        EditorGUILayout.Space();

        // 要求
        if(label_button != null)
        {
            EditorGUILayout.LabelField(label_wantTodo);
            _myScript._wantTodo = EditorGUILayout.TextArea(_myScript._wantTodo);
            EditorGUILayout.Space();
        }

        // 要求クラス名
        if(label_className!= null)
        {
            EditorGUILayout.LabelField(label_className);
            _myScript._className = EditorGUILayout.TextField(_myScript._className);
            EditorGUILayout.Space();
        }

        // 要求コード規約
        if(label_rules!= null)
        {
            EditorGUILayout.LabelField(label_rules);
            _myScript._rules = EditorGUILayout.TextArea(_myScript._rules);
            EditorGUILayout.Space();
        }

        if(label_extension!= null)
        {
            EditorGUILayout.LabelField(label_extension);
            _myScript._extension = EditorGUILayout.TextArea(_myScript._extension);
            EditorGUILayout.Space();
        }
        // AIの詳細設定
        _advanceSettingfoldOut = EditorGUILayout.Foldout(_advanceSettingfoldOut, "AI詳細設定(調整非推奨)");
        if(_advanceSettingfoldOut)
        {
            EditorGUILayout.LabelField("モデル名               (推奨:text-davinci-003)");
            _myScript._modelName = EditorGUILayout.TextField(_myScript._modelName);

            EditorGUILayout.LabelField("文章の多様性      (推奨:0.7f)");
            _myScript._temperature = EditorGUILayout.Slider(_myScript._temperature, 0f, 2f);

            EditorGUILayout.LabelField(new GUIContent("頻度ペナルティ      (推奨:0)",

                                                      "モデルが生成するテキストの単語の重複を抑えるためのペナルティ係数です。" +
                                                      "つまり、より多様な単語を使用するようにモデルに促すために使用されます。" +
                                                      "この係数が大きいほど、重複した単語が生成される確率が低くなり、" +
                                                      "多様な単語が使用される可能性が高くなります。"));

            _myScript._frequencyPenalty = EditorGUILayout.IntSlider(_myScript._frequencyPenalty, -2, 2);

            EditorGUILayout.LabelField(new GUIContent("記憶ペナルティ      (推奨:0)",

                                                      "モデルが生成するテキストに特定のトークン（単語やフレーズなど）を" +
                                                      "必ず含ませるようにするためのペナルティ係数です。" +
                                                      "つまり、特定のトークンを含めることを強制することで、" +
                                                      "モデルが特定のトピックやスタイルに沿った文章を生成するように促すことができます。" +
                                                      "この係数が大きいほど、指定されたトークンが必ず含まれるようになります。"));
            _myScript._presencePenalty = EditorGUILayout.IntSlider(_myScript._presencePenalty, -2, 2) ;
        }



        // 生成ボタン
        if (GUILayout.Button(label_button))
        {
            _myScript.Execute();
        }




        // 生成された結果
        if (label_inputResults != null)
        {
            EditorGUILayout.LabelField(label_inputResults);
            _myScript._inputResults = EditorGUILayout.TextArea(_myScript._inputResults);
        }
    }
}
