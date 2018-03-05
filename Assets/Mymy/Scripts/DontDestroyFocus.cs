using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyFocus : MonoBehaviour
{
    public static DontDestroyFocus instance = null;
    //make sure that we only have a single instance of the Focus Prefab
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

}
