using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using UnityEngine.UI;

namespace UnityIME
{
    #if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(InputFieldManager))]
    [CanEditMultipleObjects]
    public class InputFieldManagerEditor : Editor
    {
        ReorderableList RList;
        SerializedProperty SerProp_LocaleArray;

        void OnEnable()
        {
            SerProp_LocaleArray = serializedObject.FindProperty ("LocaleArray");
            RList = new ReorderableList (serializedObject, SerProp_LocaleArray, false, false, true, true);
            RList.drawElementCallback = DrawElement;//DrawListElement;
            RList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Languages");
            };
        }

        void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty _serElement = RList.serializedProperty.GetArrayElementAtIndex(index);

            float _spacing = 10f;

            // 1st column
            Rect _rect = rect;
            _rect.height = EditorGUIUtility.singleLineHeight;
            _rect.width = 100;
            EditorGUI.LabelField (_rect, (index == 0 ? "Default" : "Language " + (index+1)));

            // last column
            rect.x = 100 + _spacing;
            rect.width = rect.width - (100 - _spacing);
            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, _serElement, GUIContent.none);
        }

        public override void OnInspectorGUI()
        {
            var myScript = target as InputFieldManager;
            myScript.IsPassword = EditorGUILayout.Toggle ("Is password?", myScript.IsPassword);
            myScript.Target = (Text)EditorGUILayout.ObjectField ("Target", myScript.Target, typeof(Text), true);

            serializedObject.Update();
            RList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif

    public class InputFieldManager : MonoBehaviour, ISubmitHandler, IPointerClickHandler
    {
        private static string LOG_TAG = "UnityIME.InputFieldManager";

        public KeyboardLanguage[] LocaleArray;
        public bool IsPassword = false;
        public Text Target;
        [HideInInspector]
        public string storedPasswd = null;

        private KeyboardManager KMInstance = null;
        private MyInputField myIF = null;
        private Hashtable htLocaleArray = new Hashtable();
        private bool initialized = false;

        public void UpdateTargetText(string str)
        {
            Target.text = "Result: " + str;
        }

        private bool ValidateInputField()
        {
            myIF = gameObject.GetComponent<MyInputField> ();
            if (myIF == null)
            {
                #if UNITY_EDITOR
                Debug.Log (LOG_TAG + ", InitKeyboards(), no MyInputField in GameObject");
                #endif
                //Log.e (LOG_TAG, "InitKeyboards(), no MyInputField in GameObject");
                return false;
            }

            return true;
        }

        private void EnableLocale(KeyboardLanguage kl)
        {
            Locale _locale = (Locale)htLocaleArray [kl];
            if (_locale != null)
            {
                _locale.ActivateCurrentKeyboard ();
                curLocale = kl;
            }
        }

        private KeyboardLanguage curLocale = KeyboardLanguage.English, originLocale = KeyboardLanguage.English;
        public void AlterKeyboard()
        {
            // switch if more than 1 keyboard
            if (LocaleArray.Length > 1)
            {
                if (curLocale != KeyboardLanguage.Symbol)
                {
                    Symbol _localeSYM = (Symbol)htLocaleArray [KeyboardLanguage.Symbol];
                    if (_localeSYM != null)
                    {
                        #if UNITY_EDITOR
                        Debug.Log (LOG_TAG + ", AlterKeyboard() switch to symbol keyboard.");
                        #endif
                        KMInstance.DeactivateAllKeyboards ();
                        _localeSYM.ActivateCurrentKeyboard ();
                        originLocale = curLocale;   // original locale before switching to Symbol
                        curLocale = KeyboardLanguage.Symbol;
                    }
                } else
                {
                    KMInstance.DeactivateAllKeyboards ();
                    if (LocaleArray [0] == KeyboardLanguage.Symbol)
                    {
                        // Default (LocaleArray[0]) keyboard is Symbol, switch to 2nd (LocaleArray[1]) keyboard.
                        Locale _locale = (Locale)htLocaleArray[LocaleArray[1]];

                        #if UNITY_EDITOR
                        Debug.Log (LOG_TAG + ", AlterKeyboard() switch to " + _locale + " keyboard.");
                        #endif

                        _locale.ActivateCurrentKeyboard ();
                        curLocale = LocaleArray [1];
                    } else
                    {
                        // Switch back from Symbol
                        Locale _locale = (Locale)htLocaleArray[originLocale];

                        #if UNITY_EDITOR
                        Debug.Log (LOG_TAG + ", AlterKeyboard() switch back to " + _locale + " keyboard.");
                        #endif

                        _locale.ActivateCurrentKeyboard ();
                        curLocale = originLocale;
                    }
                }
            }
        }

        // Use this for initialization
        void Start ()
        {
        }
        
        // Update is called once per frame
        void Update ()
        {
        }

        void OnEnable()
        {
        }

        private void SetupKeyboards()
        {
            foreach (KeyboardLanguage _kl in LocaleArray)
            {
                Locale _locale = (Locale)htLocaleArray [_kl];
                _locale.SetupKeyboards ();
            }
        }

        /// <summary>
        /// Initializes Hashtable of locale array.
        /// </summary>
        private void InitializeKeyboards()
        {
            KMInstance = KeyboardManager.Instance;

            if (ValidateInputField ())
            {
                foreach (KeyboardLanguage _kl in LocaleArray)
                {
                    if (!KMInstance.IsLocaleRegistered (_kl))
                    {
                        #if UNITY_EDITOR
                        Debug.Log(LOG_TAG + ", InitializeKeyboards() " + _kl + " does NOT have registered keyboard.");
                        #endif
                        continue;
                    }

                    switch (_kl)
                    {
                    case KeyboardLanguage.English:
                        English _eng = new English (this, IsPassword);
                        htLocaleArray.Add (_kl, _eng);
                        break;
                    case KeyboardLanguage.TraditionalChinese:
                        break;
                    case KeyboardLanguage.Symbol:
                        Symbol _sym = new Symbol (this, IsPassword);
                        htLocaleArray.Add (_kl, _sym);
                        break;
                    default:
                        break;
                    }

                    initialized = true;
                }
            }
        }

        #region Event System
        public void OnSubmit (BaseEventData eventData)
        {
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + ", " + gameObject.name + " OnSubmit()");
            #endif
            //Log.d (LOG_TAG, gameObject.name + " OnSubmit()");

            if (!initialized)
            {
                InitializeKeyboards ();
                if (htLocaleArray.Count > 0)
                {
                    foreach (KeyboardLanguage _kl in htLocaleArray.Keys)
                    {
                        curLocale = _kl;
                        break;
                    }
                }
            }
            SetupKeyboards ();
            KMInstance.DeactivateAllKeyboards ();
            EnableLocale (curLocale);
        }

        public void OnPointerClick (PointerEventData eventData)
        {
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + ", " + gameObject.name + " OnPointerClick()");
            #endif
            //Log.d (LOG_TAG, gameObject.name + " OnPointerClick()");

            if (!initialized)
            {
                InitializeKeyboards ();
                if (htLocaleArray.Count > 0)
                {
                    foreach (KeyboardLanguage _kl in htLocaleArray.Keys)
                    {
                        curLocale = _kl;
                        break;
                    }
                }
            }
            SetupKeyboards ();
            KMInstance.DeactivateAllKeyboards ();
            EnableLocale (curLocale);
        }
        #endregion
    }
}