using UnityEditor;
using UnityEngine;
using UnityLibrary;

[CustomEditor(typeof(UserSupportAI))]
public class UserSupportAIEditor : Editor
{

    // �ڍאݒ�h���b�v�pbool�ϐ�
    bool _advanceSettingfoldOut;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UserSupportAI _myScript = (UserSupportAI)target;

        string label_wantTodo = null;
        string label_className = null;
        string label_inputResults = null;
        string label_button = null;

        // �R�[�h�K�����͂��鏊
        string label_rules = null;

        // ���̑��R�[�h�����Ɏg���A�g���q����͂��鏊
        string label_extension = null;

        if (_myScript._mode==UserSupportAI.MODE.CodeGeneration)
        {
            label_wantTodo = "�����Ɏ������������e���L�q";
            label_className = "�����ɃN���X�����L�q����";
            label_inputResults = "����";
            label_button = "����";
        }
        else if(_myScript._mode == UserSupportAI.MODE.OtherGeneration)
        {
            label_wantTodo = "�����Ɏ����������R�[�h���e���L�q";
            label_extension = "�����Ɋg���q���L�q����(.(�h�b�g)�s�v)";
            label_className = "�����Ƀt�@�C�������L�q����";
            label_inputResults = "����";
            label_button = "����";
        }
        else if(_myScript._mode == UserSupportAI.MODE.CodeReview)
        {
            label_wantTodo = "���r���[���Ăق����X�N���v�g�����";
            label_rules = "�C���̌��ɂ��Ăق����K��";
            label_inputResults = "����";
            label_button = "�R�[�h�����r���[";
        }
        else if(_myScript._mode == UserSupportAI.MODE.Comment)
        {
            label_wantTodo = "�R�����g��t���Ăق������������";
            label_inputResults = "����";
            label_button = "�R�����g��t����";
        }
        else if(_myScript._mode == UserSupportAI.MODE.NamedMethod)
        {
            label_wantTodo = "���\�b�h���e�����";
            label_inputResults = "����";
            label_button = "���̏����ɖ��O������";
        }
        else if (_myScript._mode == UserSupportAI.MODE.FreeChat)
        {
            label_wantTodo = "����";
            label_inputResults = "����";
            label_button = "ChatGPT�Ɏ��₷��";
        }

        EditorGUILayout.Space();

        // �v��
        if(label_button != null)
        {
            EditorGUILayout.LabelField(label_wantTodo);
            _myScript._wantTodo = EditorGUILayout.TextArea(_myScript._wantTodo);
            EditorGUILayout.Space();
        }

        // �v���N���X��
        if(label_className!= null)
        {
            EditorGUILayout.LabelField(label_className);
            _myScript._className = EditorGUILayout.TextField(_myScript._className);
            EditorGUILayout.Space();
        }

        // �v���R�[�h�K��
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
        // AI�̏ڍאݒ�
        _advanceSettingfoldOut = EditorGUILayout.Foldout(_advanceSettingfoldOut, "AI�ڍאݒ�(�����񐄏�)");
        if(_advanceSettingfoldOut)
        {
            EditorGUILayout.LabelField("���f����               (����:text-davinci-003)");
            _myScript._modelName = EditorGUILayout.TextField(_myScript._modelName);

            EditorGUILayout.LabelField("���͂̑��l��      (����:0.7f)");
            _myScript._temperature = EditorGUILayout.Slider(_myScript._temperature, 0f, 2f);

            EditorGUILayout.LabelField(new GUIContent("�p�x�y�i���e�B      (����:0)",

                                                      "���f������������e�L�X�g�̒P��̏d����}���邽�߂̃y�i���e�B�W���ł��B" +
                                                      "�܂�A��葽�l�ȒP����g�p����悤�Ƀ��f���ɑ������߂Ɏg�p����܂��B" +
                                                      "���̌W�����傫���قǁA�d�������P�ꂪ���������m�����Ⴍ�Ȃ�A" +
                                                      "���l�ȒP�ꂪ�g�p�����\���������Ȃ�܂��B"));

            _myScript._frequencyPenalty = EditorGUILayout.IntSlider(_myScript._frequencyPenalty, -2, 2);

            EditorGUILayout.LabelField(new GUIContent("�L���y�i���e�B      (����:0)",

                                                      "���f������������e�L�X�g�ɓ���̃g�[�N���i�P���t���[�Y�Ȃǁj��" +
                                                      "�K���܂܂���悤�ɂ��邽�߂̃y�i���e�B�W���ł��B" +
                                                      "�܂�A����̃g�[�N�����܂߂邱�Ƃ��������邱�ƂŁA" +
                                                      "���f��������̃g�s�b�N��X�^�C���ɉ��������͂𐶐�����悤�ɑ������Ƃ��ł��܂��B" +
                                                      "���̌W�����傫���قǁA�w�肳�ꂽ�g�[�N�����K���܂܂��悤�ɂȂ�܂��B"));
            _myScript._presencePenalty = EditorGUILayout.IntSlider(_myScript._presencePenalty, -2, 2) ;
        }



        // �����{�^��
        if (GUILayout.Button(label_button))
        {
            _myScript.Execute();
        }




        // �������ꂽ����
        if (label_inputResults != null)
        {
            EditorGUILayout.LabelField(label_inputResults);
            _myScript._inputResults = EditorGUILayout.TextArea(_myScript._inputResults);
        }
    }
}
