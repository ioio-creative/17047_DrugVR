using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DrugVR_Scribe
{

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
        Sc08,
        Sc09,
        Sc10,
        Recap
    }

    //This Static class records all the choices made by player, as well as storing scene names as string
    public static class Scribe
    {
        public static IDictionary<DrugVR_SceneENUM, string> SceneDictionary;// = new Dictionary<DrugVR_ENUM, string>()

        public static bool side01 = true;
        public static bool side02 = true;
        public static bool side03 = true;
        public static bool side04 = true;
        public static bool side05 = true;

        static Scribe()
        {
            
            string[] stringScenes = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "SceneNames.txt"));
            DrugVR_SceneENUM enumIndex = 0;
            foreach (string sceneStr in stringScenes)
            {
                SceneDictionary.Add(enumIndex++, sceneStr);
            }

        }
    }

    public class Scroll
    {
        public DrugVR_SceneENUM Scene { get; set; }
        public SkyboxType Skybox { get; set; }
        public bool EnableController;

    }

}