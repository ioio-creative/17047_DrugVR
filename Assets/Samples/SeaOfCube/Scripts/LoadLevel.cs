// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour {
    public int ReloadSceneEveryNSecond = 20;
    void Start()
    {
        if (ReloadSceneEveryNSecond > 0)
        {
            StartCoroutine("updateCountDown");
            Invoke("nextScene", ReloadSceneEveryNSecond);
        }
    }

    IEnumerator updateCountDown()
    {
        var oneSecond = new WaitForSeconds(1);
        Text [] texts = FindObjectsOfType<UnityEngine.UI.Text>();
        Text text = null;
        foreach (Text t in texts) {
            if (t.name == "ReloadSceneCountDown")
            {
                text = t;
                break;
            }
        }
        if (text == null)
            yield break;

        for (int i = 0; i < ReloadSceneEveryNSecond; i++)
        {
            yield return oneSecond;
            var left = ReloadSceneEveryNSecond - i;
            text.text = "Load next level after " + left + "s.";
        }
    }

    void nextScene()
    {
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }
}
