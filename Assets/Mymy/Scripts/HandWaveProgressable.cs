using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandWaveProgressable : MonoBehaviour
{
    public event Action OnProgressComplete;


    // 0 <= value to return <= 1
    public float Progress
    {
        get
        {
            return m_ProgressBar.fillAmount;
        }       
    }


    private const float MinValue = 0f;
    private const float MaxValue = 1f;


    [SerializeField]
    private Image m_ProgressBar;
    [SerializeField]
    private int m_NumOfStepsToCompletion;

    private float m_ProgressPerStep;


    private void Start()
    {
        Reset();
        m_ProgressPerStep = (MaxValue - MinValue) / m_NumOfStepsToCompletion;
    }

    public void Reset()
    {
        m_ProgressBar.fillAmount = MinValue;
    }

    public void StepIt()
    {
        m_ProgressBar.fillAmount += m_ProgressPerStep;
        if (m_ProgressBar.fillAmount >= MaxValue)
        {
            if (OnProgressComplete != null)
            {
                OnProgressComplete();
            }
        }
    }
}
