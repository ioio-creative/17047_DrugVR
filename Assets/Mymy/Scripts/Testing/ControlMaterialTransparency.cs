using System.Collections;
using UnityEngine;

public class ControlMaterialTransparency : MonoBehaviour
{
    [SerializeField]
    private Renderer m_Renderer;
    
	private void Start()
    {
        StartCoroutine(ChangeColor());
	}
		
	private IEnumerator ChangeColor()
    {
        while (true)
        {
            m_Renderer.material.SetFloat("_Transparency", Random.value * 0.5f);
            yield return new WaitForSeconds(2);
        }
    }
}
