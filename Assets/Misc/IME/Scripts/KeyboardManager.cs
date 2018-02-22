using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace UnityIME
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    public enum KeyboardLanguage
    {
        English = 0,
        TraditionalChinese = 1,
        Symbol = 2
    };

    public class KeyboardManager
    {
        private static string LOG_TAG = "UnityIME.KeyboardManager";

        private Hashtable KeyboardsTable = new Hashtable();

        #region Constructor and Singleton
        private static KeyboardManager instance = null;
        public static KeyboardManager Instance
        {
            get {
                if (instance == null)
                    instance = new KeyboardManager();
                return instance;
            }
        }

        private KeyboardManager()
        {
            KeyboardsTable.Add (KeyboardLanguage.English, EnglishKeyboard.Instance);
            KeyboardsTable.Add (KeyboardLanguage.TraditionalChinese, TraditionalChineseKeyboard.Instance);
            KeyboardsTable.Add (KeyboardLanguage.Symbol, SymbolKeyboard.Instance);
        }

        ~KeyboardManager()
        {
            KeyboardsTable.Clear ();
        }
        #endregion

        public void RegisterKeyboard<TEnum>(KeyboardLanguage lang, TEnum type, GameObject keyboard)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("TEnum must be an enumerated type");
            #if UNITY_EDITOR
            Debug.Log(LOG_TAG + ", RegisterKeyboard() " + lang + ", " + type + ", " + keyboard.name);
            #endif

            AbstractKeyboard<TEnum> _akb = (AbstractKeyboard<TEnum>)KeyboardsTable [lang];
            _akb.RegisterKeyboard (type, keyboard);
        }

        public bool IsLocaleRegistered(KeyboardLanguage lang)
        {
            bool _registered = false;
            switch (lang)
            {
            case KeyboardLanguage.English:
                if (EnglishKeyboard.Instance.HasKeyboardRegistered ())
                    _registered = true;
                break;
            case KeyboardLanguage.TraditionalChinese:
                if (TraditionalChineseKeyboard.Instance.HasKeyboardRegistered ())
                    _registered = true;
                break;
            case KeyboardLanguage.Symbol:
                if (SymbolKeyboard.Instance.HasKeyboardRegistered ())
                    _registered = true;
                break;
            }
            return _registered;
        }

        public GameObject GetKeyboard<TEnum>(KeyboardLanguage lang, TEnum type)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("TEnum must be an enumerated type");

            AbstractKeyboard<TEnum> _akb = (AbstractKeyboard<TEnum>)KeyboardsTable [lang];
            return _akb.GetKeyboard (type);
        }

        public List<CKeyboard<TEnum>> GetLocaleKeyboards<TEnum>(KeyboardLanguage lang)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("TEnum must be an enumerated type");

            AbstractKeyboard<TEnum> _akb = (AbstractKeyboard<TEnum>)KeyboardsTable [lang];
            return _akb.GetKeyboardList ();
        }

        public bool IsKeyboardActive<TEnum> (KeyboardLanguage lang, TEnum type)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("TEnum must be an enumerated type");

            AbstractKeyboard<TEnum> _akb = (AbstractKeyboard<TEnum>)KeyboardsTable [lang];
            return _akb.IsKeyboardActive(type);
        }

        public void ActivateKeyboard<TEnum> (KeyboardLanguage lang, TEnum type, bool active)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("TEnum must be an enumerated type");
            #if UNITY_EDITOR
            Debug.Log(LOG_TAG + ", ActivateKeyboard() " + (active ? "enable " : "disable ") + type);
            #endif

            AbstractKeyboard<TEnum> _akb = (AbstractKeyboard<TEnum>)KeyboardsTable [lang];
            if (_akb != null)
            {
                _akb.ActivateKeyboard (type, active);
            }
        }

        public void DeactivateAllKeyboards()
        {
            EnglishKeyboard.Instance.DeactivateAllKeyboard ();
            TraditionalChineseKeyboard.Instance.DeactivateAllKeyboard ();
            SymbolKeyboard.Instance.DeactivateAllKeyboard ();
        }
    }
}