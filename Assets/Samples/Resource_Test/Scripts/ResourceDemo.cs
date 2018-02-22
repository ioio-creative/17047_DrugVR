using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using wvr;
using WaveVR_Log;

public class ResourceDemo : MonoBehaviour {
    private static string LOG_TAG = "ResourceDemo";
    private WaveVR_Resource rw = null;

    // Use this for initialization
    void Start () {
        Log.d(LOG_TAG, "start()");
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            Debug.Log("ResourceDemo can't run on editor!");
            return;
        }
#endif
        rw = WaveVR_Resource.instance;

        Log.d(LOG_TAG, "system default language is " + rw.getSystemLanguage());
        Log.d(LOG_TAG, "system default country is " + rw.getSystemCountry());

        Log.d(LOG_TAG, "get string by system default language");

        string tmp = rw.getString("allow");
        Log.d(LOG_TAG, "get allow by system default language : " + tmp);

        tmp = rw.getString("demo");
        Log.d(LOG_TAG, "get demo by system default language : " + tmp);

        tmp = rw.getString("reject");
        Log.d(LOG_TAG, "get reject by system default language : " + tmp);

        tmp = rw.getString("test");
        Log.d(LOG_TAG, "get test by system default language : " + tmp);

        bool ret = rw.setPreferredLanguage("zh", "CN");
        Log.d(LOG_TAG, "set preferred lanuage to simplized chinese and get string = " + ret);

        tmp = rw.getString("allow");
        Log.d(LOG_TAG, "get allow by preferred lanuage(zhCN) : " + tmp);

        tmp = rw.getString("demo");
        Log.d(LOG_TAG, "get demo by preferred language(zhCN) : " + tmp);

        tmp = rw.getString("reject");
        Log.d(LOG_TAG, "get reject by preferred language(zhCN) : " + tmp);

        tmp = rw.getString("test");
        Log.d(LOG_TAG, "get test by preferred language(zhCN) : " + tmp);

        Log.d(LOG_TAG, "get string in dedicated language");
        tmp = rw.getStringByLanguage("demo", "zh", "TW");
        Log.d(LOG_TAG, "get demo by zhTW : " + tmp);
        tmp = rw.getStringByLanguage("demo", "en", "US");
        Log.d(LOG_TAG, "get demo by enUS : " + tmp);
        tmp = rw.getStringByLanguage("demo", "ja", "JP");
        Log.d(LOG_TAG, "get demo by jaJP : " + tmp);
        rw.getStringByLanguage("demo", "xx", "xx");
        Log.d(LOG_TAG, "get demo by xxxx: " + tmp);

        Log.d(LOG_TAG, "set back to default language ");
        rw.useSystemLanguage();

        tmp = rw.getString("allow");
        Log.d(LOG_TAG, "get allow from native : " + tmp);

        tmp = rw.getString("demo");
        Log.d(LOG_TAG, "get demo from native : " + tmp);

        tmp = rw.getString("reject");
        Log.d(LOG_TAG, "get reject from native : " + tmp);

        tmp = rw.getString("test");
        Log.d(LOG_TAG, "get test from native : " + tmp);
    }

    // Update is called once per frame
    void Update () {

    }

    public void selectTCLang() {
#if UNITY_EDITOR
        if (Application.isEditor) return;
#endif
        if (rw == null) {
             Log.w(LOG_TAG, "Failed to initial WaveVR Resource instance!");
        }

        GameObject at = GameObject.Find("AllowText");
        if (at != null) {
             Text allowText = at.GetComponent<Text>();
             allowText.text = rw.getStringByLanguage("allow", "zh", "TW");
        } else {
             Log.w(LOG_TAG, "Could not find allow text game object!");
        }

        GameObject dt = GameObject.Find("DemoText");
        if (at != null) {
             Text demoText = dt.GetComponent<Text>();
             demoText.text = rw.getStringByLanguage("demo", "zh", "TW");
        } else {
             Log.w(LOG_TAG, "Could not find demo text game object!");
        }

        GameObject rt = GameObject.Find("RejectText");
        if (rt != null) {
             Text rejectText = rt.GetComponent<Text>();
             rejectText.text = rw.getStringByLanguage("reject", "zh", "TW");
        } else {
             Log.w(LOG_TAG, "Could not find reject text game object!");
        }

        GameObject tt = GameObject.Find("TestText");
        if (tt != null) {
             Text testText = tt.GetComponent<Text>();
             testText.text = rw.getStringByLanguage("test", "zh", "TW");
        } else {
             Log.w(LOG_TAG, "Could not find test text game object!");
        }
    }

    public void selectENLang() {
#if UNITY_EDITOR
        if (Application.isEditor) return;
#endif
        if (rw == null) {
             Log.w(LOG_TAG, "Failed to initial WaveVR Resource instance!");
        }

        GameObject at = GameObject.Find("AllowText");
        if (at != null) {
             Text allowText = at.GetComponent<Text>();
             allowText.text = rw.getStringByLanguage("allow", "en", "US");
        } else {
             Log.w(LOG_TAG, "Could not find allow text game object!");
        }

        GameObject dt = GameObject.Find("DemoText");
        if (at != null) {
             Text demoText = dt.GetComponent<Text>();
             demoText.text = rw.getStringByLanguage("demo", "en", "US");
        } else {
             Log.w(LOG_TAG, "Could not find demo text game object!");
        }

        GameObject rt = GameObject.Find("RejectText");
        if (rt != null) {
             Text rejectText = rt.GetComponent<Text>();
             rejectText.text = rw.getStringByLanguage("reject", "en", "US");
        } else {
             Log.w(LOG_TAG, "Could not find reject text game object!");
        }

        GameObject tt = GameObject.Find("TestText");
        if (tt != null) {
             Text testText = tt.GetComponent<Text>();
             testText.text = rw.getStringByLanguage("test", "en", "US");
        } else {
             Log.w(LOG_TAG, "Could not find test text game object!");
        }
    }

    public void selectJPLang() {
#if UNITY_EDITOR
        if (Application.isEditor) return;
#endif
        if (rw == null) {
             Log.w(LOG_TAG, "Failed to initial WaveVR Resource instance!");
        }

        GameObject at = GameObject.Find("AllowText");
        if (at != null) {
             Text allowText = at.GetComponent<Text>();
             allowText.text = rw.getStringByLanguage("allow", "ja", "JP");
        } else {
             Log.w(LOG_TAG, "Could not find allow text game object!");
        }

        GameObject dt = GameObject.Find("DemoText");
        if (at != null) {
             Text demoText = dt.GetComponent<Text>();
             demoText.text = rw.getStringByLanguage("demo", "ja", "JP");
        } else {
             Log.w(LOG_TAG, "Could not find demo text game object!");
        }

        GameObject rt = GameObject.Find("RejectText");
        if (rt != null) {
             Text rejectText = rt.GetComponent<Text>();
             rejectText.text = rw.getStringByLanguage("reject", "ja", "JP");
        } else {
             Log.w(LOG_TAG, "Could not find reject text game object!");
        }

        GameObject tt = GameObject.Find("TestText");
        if (tt != null) {
             Text testText = tt.GetComponent<Text>();
             testText.text = rw.getStringByLanguage("test", "ja", "JP");
        } else {
             Log.w(LOG_TAG, "Could not find test text game object!");
        }
    }

    public void selectQuit()
    {
        Log.d(LOG_TAG, "Quit Game");
        Application.Quit();
    }
}
