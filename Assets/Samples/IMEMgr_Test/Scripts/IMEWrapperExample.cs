using UnityEngine;
using UnityEngine.UI;
using WaveVR_Log;

public class IMEWrapperExample : MonoBehaviour {
	private InputField myInputField;
    private GameObject myButton;
    private static string LOG_TAG = "IMEWrapperExample";
	private WaveVR_IMEManagerWrapper mIMEWrapper;
	private string mInputContent = null;

    private bool mIsButton = false;

	void Start() {
		mIMEWrapper = WaveVR_IMEManagerWrapper.GetInstance();
		mIMEWrapper.SetCallback(InputDoneCallback);
	}

	private void InputDoneCallback(WaveVR_IMEManagerWrapper.InputResult results) {
		Log.d(LOG_TAG, "inputDoneCallback:" + results.GetContent());
		mInputContent = results.GetContent();

		// Note: directly update input field text will exception
		// use LastUpdate to update Input field text
	}

	private void UpdateInputField(string str) {
		Log.d(LOG_TAG, "UpdateTargetText:" + str);
		if (myInputField != null && str != null) {
			myInputField.textComponent.text = str;
			myInputField.placeholder.GetComponent<Text>().text = "";
		}
	}

    private void UpdateButton(string str)
    {
        Log.d(LOG_TAG, "UpdateButton:" + str);
        if (myButton != null && str != null)
        {
            myButton.GetComponentInChildren<Text>().text = str;
        }
    }

    private InputField GetInputField(string name) {
		InputField inputObj = GameObject.Find(name).GetComponent<InputField>();
		return inputObj;
	}
		
	public void ShowKeyboardEng() {
		Log.i(LOG_TAG, "showKeyboardEng");
		myInputField = GetInputField("NameInputField");
		if (myInputField != null) {
			myInputField.shouldHideMobileInput = false;
			Log.i(LOG_TAG, "NameInputField.text = "+ myInputField.textComponent.text);
			mIMEWrapper.SetText(myInputField.textComponent.text);
		}
		mIMEWrapper.SetTitle("Enter Username...");
		mIMEWrapper.SetLocale(WaveVR_IMEManagerWrapper.Locale.zh_CN);
		mIMEWrapper.SetAction (WaveVR_IMEManagerWrapper.Action.Enter);
		mIMEWrapper.Show();
	}

	public void ShowKeyboardPassword() {
		Log.i(LOG_TAG, "showKeyboardPassword");
		myInputField = GetInputField("PasswordInputField");
		if (myInputField != null) {
			myInputField.shouldHideMobileInput = false;
			Log.i(LOG_TAG, "PasswordInputField.text = "+ myInputField.textComponent.text);
			mIMEWrapper.SetText(myInputField.textComponent.text);
		}
		mIMEWrapper.SetTitle("Enter password...");
		mIMEWrapper.SetLocale(WaveVR_IMEManagerWrapper.Locale.en_US);
		mIMEWrapper.SetAction (WaveVR_IMEManagerWrapper.Action.Send);
		mIMEWrapper.Show();

	}

    public void ShowKeyboardEmpty()
    {
        Log.i(LOG_TAG, "ShowKeyboardEmpty");
        //Re-init all parameters
        mIMEWrapper.InitParameter();
        myInputField = GetInputField("EmptyInputField");
        if (myInputField != null)
        {
            myInputField.shouldHideMobileInput = false;
            Log.i(LOG_TAG, "EmptyInputField.text = " + myInputField.textComponent.text);
            mIMEWrapper.SetText(myInputField.textComponent.text);
        }
        mIMEWrapper.SetTitle("Enter text...");
        mIMEWrapper.SetAction(WaveVR_IMEManagerWrapper.Action.Send);
        mIMEWrapper.Show();

    }

    public void ShowKeyboardButton(GameObject target)
    {
        Log.i(LOG_TAG, "ShowKeyboardButton");
        //Re-init all parameters
        mIMEWrapper.InitParameter();
        mIsButton = true;
        myButton = target;
        if (target != null)
        {
            Log.i(LOG_TAG, "ShowKeyboardButton text = " + myButton.GetComponentInChildren<Text>().text);
            mIMEWrapper.SetText(myButton.GetComponentInChildren<Text>().text);
        }
        mIMEWrapper.SetTitle("ShowKeyboardButton...");
        mIMEWrapper.SetAction(WaveVR_IMEManagerWrapper.Action.Send);
        mIMEWrapper.Show();
    }

    void LateUpdate() {
		if (mInputContent != null) {
            if (mIsButton)
            {
                mIsButton = false;
                UpdateButton(mInputContent);
            }
            else
            {
                UpdateInputField(mInputContent);
            }
			
			mInputContent = null;
		}
	}
}
