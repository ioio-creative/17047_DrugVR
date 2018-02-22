using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIME
{
    public class Locale
    {
        private static string LOG_TAG = "UnityIME.Locale";
        protected KeyboardManager KMInstance = null;

        protected InputFieldManager Parent = null;
        protected bool isPassword = false;
        protected MyInputField myInputField = null;

        public Locale(InputFieldManager ifm, bool ispw)
        {
            KMInstance = KeyboardManager.Instance;

            Parent = ifm;
            if (Parent != null)
                myInputField = Parent.GetComponent<MyInputField> ();
            isPassword = ispw;
        }

        ~Locale()
        {
            KMInstance = null;

            Parent = null;
            myInputField = null;
        }

        private void FocusOnInputField()
        {
            if (myInputField != null)
            {
                //EventSystem.current.SetSelectedGameObject(_if.gameObject, null);
                myInputField.ActivateInputField ();

                #if UNITY_ANDROID
                var keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
                keyboard.active = false;
                #endif
            }
        }

        #region Methods for keyboard buttons
        public virtual void OnInputText(string s)
        {
            if (myInputField == null)
            {
                #if UNITY_EDITOR
                Debug.Log (LOG_TAG + ", OnInputText(), no MyInputField in GameObject");
                #endif
                //Log.e (LOG_TAG, "OnInputText(), no MyInputField in GameObject");
                return;
            }

            myInputField.text = myInputField.text + (isPassword ? "*" : s);
            if (isPassword)
                Parent.storedPasswd = Parent.storedPasswd + s;

            FocusOnInputField ();
        }

        public void OnBackspaceText()
        {
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + ", OnBackspaceText()");
            #endif
            //Log.d (LOG_TAG, "OnBackspaceText()");

            if (myInputField == null)
            {
                #if UNITY_EDITOR
                Debug.Log (LOG_TAG + ", OnBackspaceText(), no MyInputField in GameObject");
                #endif
                //Log.e (LOG_TAG, "OnBackspaceText(), no MyInputField in GameObject");
                return;
            }

            if (myInputField.text.Length > 0)
                myInputField.text = myInputField.text.Remove (myInputField.text.Length - 1, 1);
            if (isPassword)
            {
                if (Parent.storedPasswd.Length > 0)
                    Parent.storedPasswd = Parent.storedPasswd.Remove (Parent.storedPasswd.Length - 1, 1);
            }

            FocusOnInputField ();
        }

        public void OnClearText()
        {
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + ", OnClearText()");
            #endif
            //Log.d (LOG_TAG, "OnClearText()");

            if (myInputField == null)
            {
                #if UNITY_EDITOR
                Debug.Log (LOG_TAG + ", OnClearText(), no MyInputField in GameObject");
                #endif
                //Log.e (LOG_TAG, "OnClearText(), no MyInputField in GameObject");
                return;
            }

            myInputField.text = "";
            if (isPassword)
                Parent.storedPasswd = "";

            FocusOnInputField ();
        }

        public void OnSubmitText()
        {
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + ", OnSubmitText()");
            #endif
            //Log.d (LOG_TAG, "OnSubmitText()");

            if (myInputField == null)
            {
                #if UNITY_EDITOR
                Debug.Log (LOG_TAG + ", OnSubmitText(), no MyInputField in GameObject");
                #endif
                //Log.e (LOG_TAG, "OnSubmitText(), no MyInputField in GameObject");
                return;
            }

            Parent.UpdateTargetText (isPassword ? Parent.storedPasswd : myInputField.text);
            OnClearText ();
        }
        /// <summary>
        /// Switch to / back from symbol keyboard
        /// Empty function for override.
        /// </summary>
        public void OnAlterKeyboard()
        {
            if (Parent != null)
            {
                // switch to symbol keyboard when "Change" button is clicked
                Parent.AlterKeyboard ();
            }
        }

        public void OnCloseKeyboard()
        {
            KMInstance.DeactivateAllKeyboards ();
        }

        /// <summary>
        /// Switch to / back from capital keyboard.
        /// Empty function for override.
        /// </summary>
        public virtual void OnShiftKeyboard (){}
        #endregion

        /// <summary>
        /// Activates the current keyboard.
        /// Empty function for override.
        /// </summary>
        public virtual void ActivateCurrentKeyboard (){}
        /// <summary>
        /// Setups the keyboards of English.
        /// Empty function for override.
        /// Called when:
        /// 1. InputField initialization
        /// 2. switch InputField
        /// </summary>
        public virtual void SetupKeyboards(){}
    }
}