using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrugVR_Scribbler
{

    public enum DrugVR_ENUM
    {
        Sc01,
        Sc02
    }


    public static class Scribbler
    {
        public static IDictionary<DrugVR_ENUM, string> SceneDictionary = new Dictionary<DrugVR_ENUM, string>()
        {
            { DrugVR_ENUM.Sc01, "Scene01"},
            { DrugVR_ENUM.Sc02, "Scene02"}
        };

        public static bool side01 = true;
        public static bool side02 = true;
        public static bool side03 = true;
        public static bool side04 = true;
        public static bool side05 = true;

    }
}
