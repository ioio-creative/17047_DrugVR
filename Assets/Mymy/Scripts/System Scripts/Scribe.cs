using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

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
        Sc03X,
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
        Sc08,
        Sc09,
        Sc10,
        Summary
    }

    //This Static class records all the choices made by player, as well as storing scene names as string
    public static class Scribe
    {
        public static IDictionary<DrugVR_SceneENUM, Scroll> SceneDictionary = new Dictionary<DrugVR_SceneENUM, Scroll>();

        //Scene 1 Side Taking
        public static bool side01 = true;
        //Scene 2 Side Taking
        public static bool side02 = true;
        //Scene 3 Side Taking
        public static bool side03 = true;
        //Scene 4 Side Taking
        public static bool side04 = true;
        //Scene 6 Side Taking
        public static bool side05 = true;
        //Scene 7 Side Taking
        public static bool side06 = true;

        static Scribe()
        {
            TextAsset sceneNameTXT = Resources.Load<TextAsset>("SceneNames");

            //string filePath = Path.Combine(Application.persistentDataPath, "Mymy/Scripts/System Scripts/SceneNames.txt").ToString();
            //Debug.Log(filePath);
            string[] stringScenesParams = sceneNameTXT.text.Split(Environment.NewLine.ToCharArray(),StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();//File.ReadAllLines(filePath).Skip(1).ToArray();
            Debug.Log(stringScenesParams[1]);
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

        public string SkyContentPath = null;

        public float SkyShaderDefaultRotation = 0;

        public bool HMDRotationEnabled = true;

        public bool ControllerEnabled = true;

        public bool ControllerRotEnabled = true;

        public Scroll(string paramPresplit)
        {
            string[] paramArray = paramPresplit.Split(',');

            SceneName = paramArray[0];
            try
            {
                SceneSky = (SkyboxType)Enum.Parse(typeof(SkyboxType), paramArray[1]);
                SkyContentPath = paramArray[2];
                SkyShaderDefaultRotation = float.Parse(paramArray[3]);
                HMDRotationEnabled = ParseZeroAndOne(paramArray[4]);
                ControllerEnabled = ParseZeroAndOne(paramArray[5]);
                ControllerRotEnabled = ParseZeroAndOne(paramArray[6]);
            }
            catch (Exception)
            {

            }
        }

        bool ParseZeroAndOne(string returnValue)
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
                throw new FormatException("The string is not a recognized as a valid boolean value.");
            }
        }
    }
}