using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WaveVR_Log;
using wvr;

public class detectInputText : MonoBehaviour
{

    private InputField myInputField;
    private static string LOG_TAG = "detectInputText";
    private bool initialized = false;
    private bool showKeyboard_ = false;
    private WaveVR_IMEManager pmInstance = null;
    private string inputContent = null;
    private WaveVR_IMEManager.IMEParameter mParameter = null;
    public static int CONTROLLER_BUTTON_SYSTEM = 0;
    public static int CONTROLLER_BUTTON_MENU = 1;
    public static int CONTROLLER_BUTTON_GRIP = 2;
    public static int CONTROLLER_BUTTON_VOLUME_UP = 7;
    public static int CONTROLLER_BUTTON_VOLUME_DOWN = 8;
    public static int CONTROLLER_BUTTON_TOUCH_PAD = 16;
    public static int CONTROLLER_BUTTON_TRIGGER = 17;

    public static int SERVICE_STATE_UN_INIT = 0x00;
    public static int SERVICE_STATE_NORMAL = 0x01;
    public static int SERVICE_STATE_BUSY = 0x02;
    public static int SERVICE_STATE_ERROR = 0x04;

    public void inputDoneCallback(WaveVR_IMEManager.InputResult results)
    {
        Log.d(LOG_TAG, "inputDoneCallback:" + results.InputContent);
        inputContent = results.InputContent;
        showKeyboard_ = false;

        Log.i(LOG_TAG, "inputDoneCallback() state = " + getKeyboardState());
        //Note: directly update input field text will exception
        // use LastUpdate to update Input field text
        //updateInputField(inputContent);
    }
    public void updateInputField(string str)
    {
        Log.d(LOG_TAG, "UpdateTargetText:" + str);
        if (myInputField != null && str != null)
        {
            myInputField.textComponent.text = str;
            myInputField.placeholder.GetComponent<Text>().text = "";
            //ChangeInputFieldText(myInputField, str.ToString());
        }
    }

    public InputField getInputField(string name)
    {
        InputField inputObj = GameObject.Find(name).GetComponent<InputField>();
        return inputObj;
    }
    private void FocusOnInputField()
    {
#if UNITY_ANDROID
        var keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
        keyboard.active = false;
#endif
        if (myInputField != null)
        {
            myInputField.ActivateInputField();
        }

    }

    private void initParameter()
    {
        int MODE_FLAG_FIX_MOTION = 0x02;
        //int MODE_FLAG_AUTO_FIT_CAMERA = 0x04;
        int id = 0;
        int type = MODE_FLAG_FIX_MOTION;
        int mode = 2;

        string exist = "";
        int cursor = 0;
        int selectStart = 0;
        int selectEnd = 0;
        double[] pos = new double[] { 0, 0, -1.05 };
        double[] rot = new double[] { 1.0, 0.0, 0.0, 0.0 };
        int width = 800;
        int height = 800;
        int shadow = 100;
        string locale = "en_US";
        string title = "IMETest Title";
        int extraInt = 0;
        string extraString = "";
        int buttonId = 16;//System:0 , Menu:1 , Grip:2, Touchpad:16, Trigger:17
        mParameter = new WaveVR_IMEManager.IMEParameter(id, type, mode, exist, cursor, selectStart, selectEnd, pos,
                             rot, width, height, shadow, locale, title, extraInt, extraString, buttonId);
        //IMEParameter(int id, int type, int mode, double[] pos, double[] rot, int width, int height,
        //int shadow, string locale, string title, int buttonId)
    }
    private void InitializeKeyboards()
    {
        pmInstance = WaveVR_IMEManager.instance;
        initialized = pmInstance.isInitialized();
        showKeyboard_ = false;
        initParameter();
        if (initialized)
            Log.d(LOG_TAG, "InitializeKeyboards: done");
        else
            Log.d(LOG_TAG, "InitializeKeyboards: failed");

    }
    private void hideKeyboard()
    {
        if (showKeyboard_ && initialized)
        {
            Log.i(LOG_TAG, "hideKeyboard: done");
            pmInstance.hideKeyboard();
            showKeyboard_ = false;
            Log.i(LOG_TAG, "hideKeyboard() state = " + getKeyboardState());

        }
    }
    private void showKeyboard(WaveVR_IMEManager.IMEParameter parameter)
    {
        if (!showKeyboard_ && initialized)
        {

            //Log.i(LOG_TAG, "showKeyboard: done");
            pmInstance.showKeyboard(parameter, inputDoneCallback);
            showKeyboard_ = true;
            Log.i(LOG_TAG, "showKeyboard() state = " + getKeyboardState());
        }
    }
    public int getKeyboardState()
    {
        int state = pmInstance.getKeyboardState();
        Log.i(LOG_TAG, "showKeyboard() state = " + state);
        return state;
    }

    public void enterTextField(Image target)
    {
        Debug.Log("enterTextField");

        Color mColor;
        ColorUtility.TryParseHtmlString("#00ABFF39", out mColor);
        target.color = mColor;

    }


    public void exitTextField(Image target)
    {
        Debug.Log("enterTextField");
        target.color = Color.white;
    }

   

    public void showKeyboardEng()
    {
        Log.i(LOG_TAG, "showKeyboardEng click");
        if (!initialized)
            InitializeKeyboards();
        WaveVR_IMEManager.IMEParameter parameter = mParameter;
        parameter.id = 0;
        myInputField = getInputField("NameInputField");
        if (myInputField != null)
        {
            myInputField.shouldHideMobileInput = false;
            //FocusOnInputField();
        }
        showKeyboard(parameter);

    }
    public void showKeyboardPassword()
    {
        Debug.Log("showKeyboardPassword click");
        if (!initialized)
            InitializeKeyboards();
        WaveVR_IMEManager.IMEParameter parameter = mParameter;
        parameter.id = 1;
        myInputField = getInputField("PasswordInputField");
        if (myInputField != null)
        {
            myInputField.shouldHideMobileInput = false;
            Log.d(LOG_TAG, "PasswordInputField.text = " + myInputField.textComponent.text);

            //myInputField.textComponent.text = "Enter Password....";
            //FocusOnInputField();
        }
        showKeyboard(parameter);

    }
    void LateUpdate()
    {
        if (inputContent != null)
            updateInputField(inputContent);
        inputContent = null;
    }

}
