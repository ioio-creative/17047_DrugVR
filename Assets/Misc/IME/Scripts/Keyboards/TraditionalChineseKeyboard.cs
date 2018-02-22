using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIME
{
    public enum Type_CHT
    {
        PhoneticKeyboard = 0,
    };

    public sealed class TraditionalChineseKeyboard : AbstractKeyboard<Type_CHT>
    {
        private List<CKeyboard<Type_CHT>> KeyboardList = null;

        #region Constructor and Singleton
        public TraditionalChineseKeyboard()
        {
            KeyboardList = new List<CKeyboard<Type_CHT>> ();
        }

        ~TraditionalChineseKeyboard()
        {
            KeyboardList = null;
        }

        private static TraditionalChineseKeyboard instance = null;
        public static TraditionalChineseKeyboard Instance
        {
            get {
                if (instance == null)
                    instance = new TraditionalChineseKeyboard ();
                return instance;
            }
        }
        #endregion

        #region Override functions
        public override void RegisterKeyboard(Type_CHT type, GameObject keyboard)
        {
            CKeyboard<Type_CHT> _kb = new CKeyboard<Type_CHT> (type, keyboard);
            KeyboardList.Add (_kb);
        }

        public override GameObject GetKeyboard(Type_CHT type)
        {
            foreach (CKeyboard<Type_CHT> _kb in KeyboardList)
            {
                if (_kb.Type == type)
                    return _kb.Keyboard;
            }

            return null;
        }

        public override List<CKeyboard<Type_CHT>> GetKeyboardList()
        {
            return KeyboardList.Count > 0 ? KeyboardList : null;
        }

        public override void ActivateKeyboard(Type_CHT type, bool active)
        {
            GameObject _go = GetKeyboard (type);
            if (_go != null)
                _go.SetActive (active);
        }

        public override bool IsKeyboardActive(Type_CHT type)
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
            foreach (CKeyboard<Type_CHT> _kb in KeyboardList)
            {
                _kb.Keyboard.SetActive (false);
            }
        }
    }
}