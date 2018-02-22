using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour {
    public bool backButtonLeavesApp = true;
    public int sleepTimeout = SleepTimeout.NeverSleep;

    // Use this for initialization
    void Start () {
        Input.backButtonLeavesApp = backButtonLeavesApp;
        Screen.sleepTimeout = sleepTimeout;
    }
}
