using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityIME
{
    using T_KeyboardList           = List<CKeyboard<Type_SYM>>;

    public sealed class Symbol : Locale
    {
        private static string LOG_TAG = "UnityIME.Symbol";

        private T_KeyboardList Locale_Keyboards = null;
        private Type_SYM curKeyboard = Type_SYM.PunctuationKeyboard;

        public Symbol (InputFieldManager ifm, bool ispw) : base (ifm, ispw)
        {
            Locale_Keyboards = KMInstance.GetLocaleKeyboards<Type_SYM> (KeyboardLanguage.Symbol);
            if (Locale_Keyboards != null)
            {
                foreach (Type_SYM _type in EnumUtil.GetValues<Type_SYM>())
                {
                    foreach (CKeyboard<Type_SYM> _kb in Locale_Keyboards)
                    {
                        if (_type == _kb.Type)
                        {
                            #if UNITY_EDITOR
                            Debug.Log(LOG_TAG + ", Symbol() default keyboard is " + _type);
                            #endif
                            curKeyboard = _type;
                            goto ConstructorCompleted;
                        }
                    }
                }
            }

            ConstructorCompleted:
            ;
        }

        ~Symbol()
        {
            Locale_Keyboards = null;
        }

        public override void SetupKeyboards()
        {
            if (Locale_Keyboards == null)
                return;

            #if UNITY_EDITOR
            Debug.Log(LOG_TAG + ", SetupKeyboards()");
            #endif

            foreach (CKeyboard<Type_SYM> _keyboard in Locale_Keyboards)
            {
                switch (_keyboard.Type)
                {
                case Type_SYM.PunctuationKeyboard:
                    SetAlphabetKeyboard (_keyboard.Keyboard);
                    break;
                case Type_SYM.PunctuationKeyboardShift:
                    SetCapitalAlphabetKeyboard (_keyboard.Keyboard);
                    break;
                default:
                    break;
                }
            }
        } // SetupKeyboards()

        #region Locale specified functions
        private void SetAlphabetKeyboard(GameObject keyboard)
        {
            Button[] btns = keyboard.GetComponentsInChildren<Button> ();
            if (btns != null)
            {
                for (int i = 0; i < btns.Length; i++)
                {
                    btns [i].onClick.RemoveAllListeners ();

                    switch (btns [i].name)
                    {
                    case "Clear":
                        btns [i].onClick.AddListener (OnClearText);
                        break;
                    case "Submit":
                        btns [i].onClick.AddListener (OnSubmitText);
                        break;
                    case "Change":
                        btns [i].onClick.AddListener (OnAlterKeyboard);
                        break;
                    case "Shift":
                        btns [i].onClick.AddListener (OnShiftKeyboard);
                        break;
                    case "Backspace":
                        btns [i].onClick.AddListener (OnBackspaceText);
                        break;
                    case "Space":
                        btns [i].onClick.AddListener (() => OnInputText (" "));
                        break;
                    case "Close":
                        btns [i].onClick.AddListener (OnCloseKeyboard);
                        break;
                    default:
                        string s = btns [i].name;
                        btns [i].onClick.AddListener (() => OnInputText (s));
                        break;
                    }
                }
            }
        }

        private void SetCapitalAlphabetKeyboard(GameObject keyboard)
        {
            Button[] btns = keyboard.GetComponentsInChildren<Button> ();
            if (btns != null)
            {
                for (int i = 0; i < btns.Length; i++)
                {
                    btns [i].onClick.RemoveAllListeners ();

                    switch (btns [i].name)
                    {
                    case "Clear":
                        btns [i].onClick.AddListener (OnClearText);
                        break;
                    case "Submit":
                        btns [i].onClick.AddListener (OnSubmitText);
                        break;
                    case "Change":
                        btns [i].onClick.AddListener (OnAlterKeyboard);
                        break;
                    case "Shift":
                        btns [i].onClick.AddListener (OnShiftKeyboard);
                        break;
                    case "Backspace":
                        btns [i].onClick.AddListener (OnBackspaceText);
                        break;
                    case "Space":
                        btns [i].onClick.AddListener (() => OnInputText (" "));
                        break;
                    case "Close":
                        btns [i].onClick.AddListener (OnCloseKeyboard);
                        break;
                    default:
                        string s = btns [i].name;
                        btns [i].onClick.AddListener (() => OnInputText (s));
                        break;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Activate the keyboard of Symbol. Only activated keyboard will be shown.
        /// </summary>
        private void ActivateKeyboard(Type_SYM kb)
        {
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + ", ActivateKeyboard() " + kb);
            #endif

            KMInstance.ActivateKeyboard<Type_SYM> (KeyboardLanguage.Symbol, kb, true);
            curKeyboard = kb;
        }

        private void DeactivateKeyboard(Type_SYM kb)
        {
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + ", DeactivateKeyboard() " + kb);
            #endif

            KMInstance.ActivateKeyboard<Type_SYM> (KeyboardLanguage.Symbol, kb, false);
        }

        public override void ActivateCurrentKeyboard()
        {
            if (Locale_Keyboards == null)
            {
                #if UNITY_EDITOR
                Debug.Log(LOG_TAG + ", OnShiftKeyboard() no keyboards registered!!");
                #endif
                return;
            }

            ActivateKeyboard (curKeyboard);
        }

        #region Methods for keyboard buttons
        public override void OnShiftKeyboard()
        {
            if (Locale_Keyboards == null)
            {
                #if UNITY_EDITOR
                Debug.Log(LOG_TAG + ", OnShiftKeyboard() no keyboards registered!!");
                #endif
                return;
            }

            bool _has_active_keyboard = false;
            // No need to record instances of all keyboard, just looping to check all registered keyboard.
            for (int _ui = 0; _ui < Locale_Keyboards.Count; _ui++)
            {
                if (KMInstance.IsKeyboardActive<Type_SYM> (KeyboardLanguage.Symbol, Locale_Keyboards [_ui].Type))
                {
                    int _next = (_ui + 1) % Locale_Keyboards.Count;
                    DeactivateKeyboard(Locale_Keyboards [_ui].Type);
                    ActivateKeyboard(Locale_Keyboards [_next].Type);
                    _has_active_keyboard = true;
                    break;
                }
            }

            if (!_has_active_keyboard)
            {
                ActivateKeyboard (Locale_Keyboards [0].Type);
            }
        }
        #endregion
    }
}