using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DrugVR_Scribe
{
    //Scene 
    public enum DrugVR_SceneENUM
    {
        Intro,
        Sc01,
        Sc01S,
        Sc01A,
        Sc01B,
        Sc02A,
        Sc02B,
        Sc02S,
        Sc03,
        Sc03A,
        Sc03B,
        Sc03S,
        Sc04,
        Sc04S,
        Sc04A,
        Sc04B,
        Sc05A,
        Sc05B,
        Sc06,
        Sc06A,
        Sc06B,
        Sc07,
        Sc07S,
        Sc07B,
        Sc08,
        Sc09,
        Sc10,
        Summary,
        Menu
    }

    public enum Ending
    {
        EndingA,
        EndingB,
        EndingC
    }

    //This Static class records all the choices made by player, as well as storing scene names as string
    public static class Scribe
    {
        //Event will fire whenever any bool Side is set
        public static event Action OnSideTaking;

        public class DrugVREnumComparer : IEqualityComparer<DrugVR_SceneENUM>
        {
            public bool Equals(DrugVR_SceneENUM x, DrugVR_SceneENUM y)
            {
                return x == y;
            }

            public int GetHashCode(DrugVR_SceneENUM x)
            {
                return (int)x;
            }
        }

        public static IDictionary<DrugVR_SceneENUM, Scroll> SceneDictionary =
            new Dictionary<DrugVR_SceneENUM, Scroll>(new DrugVREnumComparer());

        //Scene 1 Side Taking
        private static bool _Side01;
        public static bool Side01
        {
            get{ return _Side01; }
            set
            {
                _Side01 = value;
                if (OnSideTaking != null)
                {
                    OnSideTaking();
                }               
            }
        }
        //Scene 2 Side Taking
        private static bool _Side02;
        public static bool Side02
        {
            get { return _Side02; }
            set
            {
                _Side02 = value;
                if (OnSideTaking != null)
                {
                    OnSideTaking();
                }
            }
        }
        //Scene 3 Side Taking
        private static bool _Side03;
        public static bool Side03
        {
            get { return _Side03; }
            set
            {
                _Side03 = value;
                if (OnSideTaking != null)
                {
                    OnSideTaking();
                }
            }
        }
        //Scene 4 Side Taking
        private static bool _Side04;
        public static bool Side04
        {
            get { return _Side04; }
            set
            {
                _Side04 = value;
                if (OnSideTaking != null)
                {
                    OnSideTaking();
                }
            }
        }
        //Scene 6 Side Taking
        private static bool _Side05;
        public static bool Side05
        {
            get { return _Side05; }
            set
            {
                _Side05 = value;
                if (OnSideTaking != null)
                {
                    OnSideTaking();
                }
            }
        }
        //Scene 7 Side Taking
        private static bool _Side06;
        public static bool Side06
        {
            get { return _Side06; }
            set
            {
                _Side06 = value;
                if (OnSideTaking != null)
                {
                    OnSideTaking();
                }
            }
        }
        
        //Which ending to display for the player
        public static Ending EndingForPlayer
        {
            get
            {
                // both Side05 and Side06 are true
                if (Side05 && Side06)
                {
                    return Ending.EndingA;                    
                }
                // either one of Side05 or Side06 is true
                else if (Scribe.Side05 || Scribe.Side06)
                {
                    return Ending.EndingB;
                }
                // both Side05 and Side06 are false
                else
                {
                    return Ending.EndingC;
                }
            }
        }

        public static bool GetSideValueByName(string nameOfSide)
        {
            // use C# reflection here
            PropertyInfo propInfo = typeof(Scribe).GetProperty(nameOfSide);

            // null for getting value from static field
            return (bool)propInfo.GetValue(null, null);
        }

        static Scribe()
        {
            TextAsset sceneNameTXT = Resources.Load<TextAsset>("SceneNames");

            //string filePath = Path.Combine(Application.persistentDataPath, "Mymy/Scripts/System Scripts/SceneNames.txt").ToString();
            //Debug.Log(filePath);

            // use '\r' and '\n' (preferred to Environment.NewLine only) to cater for both Unix and non-Unix systems
            // because the txt file may be written in Windows (where Environment.NewLine = "\r\n")
            // and it may be read in Android (linux where Environment.NewLine = "\n", so there may be a trailing '\r' at the end of every line after string.Split())
            char[] newLineChars = (Environment.NewLine + "\r\n").ToCharArray().Distinct().ToArray();
            string[] stringScenesParams = sceneNameTXT.text.Split(newLineChars, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            //Debug.Log(stringScenesParams[1]);
            DrugVR_SceneENUM enumIndex = 0;

            foreach (string sceneParam in stringScenesParams)
            {
                SceneDictionary.Add(enumIndex++, new Scroll(sceneParam));
            }

        }

        public static IEnumerator LoadTxtDataWWW()
        {
            string sceneNameData;
            using (WWW sceneNameDataRaw = new WWW(Application.streamingAssetsPath + "/SceneNames.txt"))
            {
                yield return sceneNameDataRaw;
                sceneNameData = sceneNameDataRaw.text;
            }

            // use '\r' and '\n' (preferred to Environment.NewLine only) to cater for both Unix and non-Unix systems
            // because the txt file may be written in Windows (where Environment.NewLine = "\r\n")
            // and it may be read in Android (linux where Environment.NewLine = "\n", so there may be a trailing '\r' at the end of every line after string.Split())
            char[] newLineChars = (Environment.NewLine + "\r\n").ToCharArray().Distinct().ToArray();
            string[] stringScenesParams = sceneNameData.Split(newLineChars, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            //Debug.Log(stringScenesParams[1]);
            DrugVR_SceneENUM enumIndex = 0;

            foreach (string sceneParam in stringScenesParams)
            {
                SceneDictionary.Add(enumIndex++, new Scroll(sceneParam));
            }
        }
    }

    public class Scroll
    {
        public string SceneName;

        public SkyboxType SceneSky = SkyboxType.Null;

        public float SkyShaderDefaultRotation = 0;

        public string Video_ImgPath = null;

        public bool VideoAutoPlay = false;

        public Scroll(string paramPresplit)
        {
            string[] paramArray = paramPresplit.Split(',');

            SceneName = paramArray[0];
            try
            {
                SceneSky = (SkyboxType)Enum.Parse(typeof(SkyboxType), paramArray[1]);               
                SkyShaderDefaultRotation = float.Parse(paramArray[2]);
                Video_ImgPath = paramArray[3];                
                VideoAutoPlay = ParseZeroAndOne(paramArray[4]);
            }
            catch (Exception ex)
            {
                //GameManager.Instance.DebugLog(ex.ToString());
            }
        }

        public static bool ParseZeroAndOne(string returnValue)
        {            
            if (returnValue == "1")
            {
                return true;
            }
            else if (returnValue == "0")
            {
                return false;
            }
            else
            {
                GameManager.Instance.DebugLog(returnValue);
                throw new FormatException("The string is not a recognized as a valid boolean value.");
            }
        }
    }
}
 
 