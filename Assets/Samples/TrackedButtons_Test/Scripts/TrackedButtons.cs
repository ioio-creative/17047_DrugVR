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
using System;
using WaveVR_Log;

public class TrackedButtons : WaveVR_TrackedButtons
{
    public const string TAG = "TrackedButtons";
	public AudioClip sound;
    public GameObject controlled_object;

	private bool changeScale = false;
	private AudioSource source;

	private void PrintEvent(ClickedEventArgs e)
	{
		Log.d(TAG, "PrintEvent: " + Environment.NewLine + 
			"Device: " + e.device + Environment.NewLine + 
			"Flag: " + e.flags + Environment.NewLine + 
            "X: " + e.axis.x + Environment.NewLine + 
            "Y: " + e.axis.y);
	}

	private void MenuButtonClickedHandler (object sender, ClickedEventArgs e)
	{
		Log.d(TAG, "MenuButtonClickedHandler");
        controlled_object.SetActive(false);
	}

	private void MenuBottonUnClickedHandler(object sender, ClickedEventArgs e)
	{
		Log.d(TAG, "MenuBottonUnClickedHandler");
        controlled_object.SetActive (true);
	}

	private void PadTouchedHandler(object sender, ClickedEventArgs e)
	{
        float xangle = 360 * e.axis.x, yangle = 360 * e.axis.y;
		xangle = xangle > 0 ? xangle : -xangle;
		yangle = yangle > 0 ? yangle : -yangle;
		Log.d(TAG, "PadTouchedHandler, xangle: " + xangle + ", yangle: " + yangle);
//		PrintEvent (e);
        controlled_object.transform.Rotate (xangle*(10*Time.deltaTime), 0, 0);
        controlled_object.transform.Rotate (0, yangle*(10*Time.deltaTime), 0);
	}

	private void PadClickedHandler(object sender, ClickedEventArgs e)
	{
        Log.d(TAG, "PadClickedHandler");
		source.PlayOneShot (sound, 1f);
		if (!changeScale)
            controlled_object.transform.localScale += new Vector3 (0.3F, 0.3F, 0.3F);
		else
            controlled_object.transform.localScale -= new Vector3(0.3F, 0.3F, 0.3F);

		changeScale = !changeScale;
	}

	void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		Log.d(TAG, "OnEnable");
        controlled_object.SetActive (true);
		MenuButtonClicked += new ClickedEventHandler (MenuButtonClickedHandler);
		MenuButtonUnclicked += new ClickedEventHandler (MenuBottonUnClickedHandler);
		PadTouched += new ClickedEventHandler (PadTouchedHandler);
		PadClicked += new ClickedEventHandler (PadClickedHandler);
	}
}
