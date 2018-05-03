using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (MeshRenderer))]
public class PartyVFXAnimationControl : MonoBehaviour {

    [SerializeField]
    private MeshRenderer m_SphereMeshRenderer;

    private void Awake()
    {
        if (!m_SphereMeshRenderer)
        {
            m_SphereMeshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void Start()
    {
        
    }

    private IEnumerator ChangeColor()
    {
        while (true)
        {
            m_SphereMeshRenderer.material.SetFloat("_Transparency", Random.value * 0.5f);
            yield return new WaitForSeconds(2);
        }
    }

    private IEnumerator PlayFXAnim()
    {
        yield return null;
    }
}
