using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrugVR_Scribe;

public class MoralMeterControl : MonoBehaviour
{
    [SerializeField]
    Image m_Pointer;
    [SerializeField]
    Image m_Red;
    [SerializeField]
    Image m_Green;

    //This value should be [-60, 60], eqv to rectTrans rot of m_Pointer
    //Green fill: [0, 60]
    //Red fill: [-60, 0]
    private float m_MoraleValue;

    private void OnEnable()
    {
        Scribe.OnSideTaking += HandleSideTaking;
    }

    private void OnDisable()
    {
        Scribe.OnSideTaking -= HandleSideTaking;
    }

    private void Start ()
    {
		
	}
	
	private void Update ()
    {
        RectTransform pointerRectTransform = m_Pointer.rectTransform;
        LimitPointerRotZ(pointerRectTransform, -60, 60);
        MeterFillAmount(pointerRectTransform.localEulerAngles.z);
    }


    private void MeterFillAmount(float pointerRotZ)
    {
        if (pointerRotZ < 180f)
        {
            m_Green.fillAmount = pointerRotZ / 360f;
            m_Red.fillAmount = 0f;
        }
        else
        {
            m_Red.fillAmount = Mathf.Abs(pointerRotZ - 360f) / 360f;
            m_Green.fillAmount = 0f;
        }
    }

    private void LimitPointerRotZ(RectTransform pointerRectTransform, float floor, float ceiling)
    {
        Vector3 clampedRotInEulerAngles = pointerRectTransform.localEulerAngles;
        if (clampedRotInEulerAngles.z < 180)
        {
            clampedRotInEulerAngles.z = Mathf.Clamp(clampedRotInEulerAngles.z, 0f, ceiling);           
        }
        else
        {
            clampedRotInEulerAngles.z = Mathf.Clamp(clampedRotInEulerAngles.z, 360f + floor, 360);
        }
        pointerRectTransform.localEulerAngles = clampedRotInEulerAngles;
    }

    private void HandleSideTaking()
    {
        //TODO
    }
}
