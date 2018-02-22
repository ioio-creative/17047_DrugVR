using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIME
{
    public enum Type_ENG
    {
        AlphabetKeyboard = 0,
        AlphabetKeyboardCapital = 1,
    };

    public sealed class EnglishKeyboard : AbstractKeyboard<Type_ENG>
    {
        private List<CKeyboard<Type_ENG>> KeyboardList = null;

        #region Constructor and Singleton
        public EnglishKeyboard()
        {
            KeyboardList = new List<CKeyboard<Type_ENG>> ();
        }

        ~EnglishKeyboard()
        {
            KeyboardList = null;
        }

        private static EnglishKeyboard instance = null;
        public static EnglishKeyboard Instance
        {
            get {
                if (instance == null)
                    instance = new EnglishKeyboard ();
                return instance;
            }
        }
        #endregion

        #region Override functions
        public override void RegisterKeyboard(Type_ENG type, GameObject keyboard)
        {
            CKeyboard<Type_ENG> _kb = new CKeyboard<Type_ENG> (type, keyboard);
            KeyboardList.Add (_kb);
        }

        public override GameObject GetKeyboard(Type_ENG type)
        {
            foreach (CKeyboard<Type_ENG> _kb in KeyboardList)
            {
                if (_kb.Type == type)
                    return _kb.Keyboard;
            }

            return null;
        }

        public override List<CKeyboard<Type_ENG>> GetKeyboardList()
        {
            return KeyboardList.Count > 0 ? KeyboardList : null;
        }

        public override void ActivateKeyboard(Type_ENG type, bool active)
        {
            GameObject _go = GetKeyboard (type);
            if (_go != null)
                _go.SetActive (active);
        }

        public override bool IsKeyboardActive(Type_ENG type)
        {
            GameObject _go = GetKeyboard (type);
            return _go == null ? false : _go.activeSelf;
        }
        #endregion

        public bool HasKeyboardRegistered ()
        {
            return KeyboardList.Count > 0 ? true : false;
        }

        public void DeactivateAllKeyboard()
        {
            foreach (CKeyboard<Type_ENG> _kb in KeyboardList)
            {
                _kb.Keyboard.SetActive (false);
            }
        }
    }
}