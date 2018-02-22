using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityIME;

namespace UnityIME
{
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(KeyboardScript))]
public class KeyboardScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        KeyboardScript myScript = target as KeyboardScript;
        myScript.KBLan = (KeyboardLanguage)EditorGUILayout.EnumPopup ("Language", myScript.KBLan);
        switch (myScript.KBLan)
        {
        case KeyboardLanguage.English:
            myScript.EngType = (Type_ENG)EditorGUILayout.EnumPopup ("Keyboard Type", myScript.EngType);
            break;
        case KeyboardLanguage.TraditionalChinese:
            myScript.ChtType = (Type_CHT)EditorGUILayout.EnumPopup ("Keyboard Type", myScript.ChtType);
            break;
        case KeyboardLanguage.Symbol:
            myScript.SymType = (Type_SYM)EditorGUILayout.EnumPopup ("Keyboard Type", myScript.SymType);
            break;
        default:
            break;
        }
    }
}
#endif

[System.Serializable]
public class KeyboardScript : MonoBehaviour
{
    public KeyboardLanguage KBLan;
    public Type_ENG EngType;
    public Type_CHT ChtType;
    public Type_SYM SymType;

    void Awake()
    {
        KeyboardManager _kbm = KeyboardManager.Instance;
        switch (KBLan)
        {
        case KeyboardLanguage.English:
            _kbm.RegisterKeyboard (KBLan, EngType, gameObject);
            break;
        case KeyboardLanguage.TraditionalChinese:
            _kbm.RegisterKeyboard (KBLan, ChtType, gameObject);
            break;
        case KeyboardLanguage.Symbol:
            _kbm.RegisterKeyboard (KBLan, SymType, gameObject);
            break;
        default:
            break;
        }
        gameObject.SetActive (false);
    }
}
}