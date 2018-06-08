using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class DishSFXControl : MonoBehaviour
{
    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip[] m_DishSFXClips;
    private int i;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PickableConfinedToPlane.OnAnyPickableObjectPicked += HandlePlateStateChanged;
        PickableConfinedToPlane.OnAnyPickableObjectDestroyed += HandlePlateStateChanged;
    }

    private void OnDisable()
    {
        PickableConfinedToPlane.OnAnyPickableObjectPicked -= HandlePlateStateChanged;
        PickableConfinedToPlane.OnAnyPickableObjectDestroyed -= HandlePlateStateChanged;
    }

    private void PlayAudioClip(AudioClip aClip)
    {
        m_AudioSource.clip = aClip;
        if (m_AudioSource.clip != null)
        {
            m_AudioSource.Play();
        }
    }

    private void HandlePlateStateChanged()
    {
        PlayAudioClip(m_DishSFXClips[(i++%m_DishSFXClips.Length)]);
    }
}
