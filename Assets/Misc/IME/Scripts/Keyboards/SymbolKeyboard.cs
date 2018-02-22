using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIME
{
    public enum Type_SYM
    {
        PunctuationKeyboard = 0,
        PunctuationKeyboardShift = 1,
    };

    public sealed class SymbolKeyboard : AbstractKeyboard<Type_SYM>
    {
        private List<CKeyboard<Type_SYM>> KeyboardList = null;

        #region Constructor and Singleton
        public SymbolKeyboard()
        {
            KeyboardList = new List<CKeyboard<Type_SYM>> ();
        }

        ~SymbolKeyboard()
        {
            KeyboardList = null;
        }

        private static SymbolKeyboard instance = null;
        public static SymbolKeyboard Instance
        {
            get {
                if (instance == null)
                    instance = new SymbolKeyboard ();
                return instance;
            }
        }
        #endregion

        #region Override functions
        public override void RegisterKeyboard(Type_SYM type, GameObject keyboard)
        {
            CKeyboard<Type_SYM> _kb = new CKeyboard<Type_SYM> (type, keyboard);
            KeyboardList.Add (_kb);
        }

        public override GameObject GetKeyboard(Type_SYM type)
        {
            foreach (CKeyboard<Type_SYM> _kb in KeyboardList)
            {
                if (_kb.Type == type)
                    return _kb.Keyboard;
            }

            return null;
        }

        public override List<CKeyboard<Type_SYM>> GetKeyboardList()
        {
            return KeyboardList.Count > 0 ? KeyboardList : null;
        }

        public override void ActivateKeyboard(Type_SYM type, bool active)
        {
            GameObject _go = GetKeyboard (type);
            if (_go != null)
                _go.SetActive (active);
        }

        public override bool IsKeyboardActive(Type_SYM type)
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
            foreach (CKeyboard<Type_SYM> _kb in KeyboardList)
            {
                _kb.Keyboard.SetActive (false);
            }
        }
    }
}