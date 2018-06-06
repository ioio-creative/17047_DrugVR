using DrugVR_Scribe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasinTrigger : MonoBehaviour
{
    public event Action OnProgressTrigger;

    [SerializeField]
    private const int m_PlatesTotal = 15;
    [SerializeField]
    [Range(1, m_PlatesTotal)]
    private int m_TriggerThresholdPlates = 3;
    [SerializeField]
    [Range(0f, 10f)]
    private float m_TriggerDelaySeconds = 2f;
    private int m_PlateCounter = 0;

    [SerializeField]
    private AudioSource m_SinkAudio;
    [SerializeField]
    private AudioClip m_DishInSinkClip;
    [SerializeField]
    private AudioClip m_DishWashCompleteClip;

    private void Awake()
    {
        if (m_SinkAudio == null)
        {
            m_SinkAudio = GetComponent<AudioSource>();
        }  
        m_SinkAudio.clip = m_DishInSinkClip ? m_DishInSinkClip : m_SinkAudio.clip;
    }

    private void OnTriggerEnter(Collider targetObj)
    {
        if (targetObj.gameObject.layer == 10)
        {
            m_SinkAudio.Play();
            
            //Destroy(targetObj.gameObject.GetComponentInParent<PickableConfinedToPlane>().ConfinedPlane.gameObject);
            Destroy(targetObj.gameObject.GetComponentInParent<Rigidbody>().transform.parent.gameObject);
            m_PlateCounter++;

            if (m_PlateCounter == m_TriggerThresholdPlates)
            {                
                StartCoroutine(WaitForSecondsAndPlayChefScoldingClip(m_TriggerDelaySeconds));
            }
        
            if (m_PlateCounter >= m_PlatesTotal)
            {
                Scribe.Side03 = true;
                StartCoroutine(DishWashCompleteRoutine());
            }
        }

    }

    private IEnumerator DishWashCompleteRoutine()
    {
        yield return new WaitWhile(() => m_SinkAudio.isPlaying);
        m_SinkAudio.clip = m_DishWashCompleteClip;
        m_SinkAudio.Play();
        Sc03AClient.GoToSceneOnChoice();
    }

    private IEnumerator WaitForSecondsAndPlayChefScoldingClip(float s)
    {
        yield return new WaitForSeconds(s);
        if (OnProgressTrigger != null)
        {
            OnProgressTrigger();
        }
    }
}
