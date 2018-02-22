using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIME
{
    public class CKeyboard<T>
    {
        public T Type;
        public GameObject Keyboard = null;

        public CKeyboard(T type, GameObject go)
        {
            Type = type;
            Keyboard = go;
        }

        ~CKeyboard()
        {
            Keyboard = null;
        }

        public static bool ValidateKeyboard(CKeyboard<T> kb)
        {
            if (kb.Keyboard == null)
                return false;
            return true;
        }
    }

    public abstract class AbstractKeyboard<TEnum>
        where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        protected AbstractKeyboard(){}
        public abstract void RegisterKeyboard(TEnum type, GameObject keyboard);
        public abstract GameObject GetKeyboard (TEnum type);
        public abstract List<CKeyboard<TEnum>> GetKeyboardList ();
        public abstract void ActivateKeyboard (TEnum type, bool active);
        public abstract bool IsKeyboardActive (TEnum type);
    }
}