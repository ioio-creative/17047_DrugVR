using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc7A_PartyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_FaderToStartContainers;

    private UIFader[] m_FadersToStart;


    private void Awake()
    {
        List<UIFader> faders = new List<UIFader>();
        foreach (GameObject faderToStartContainer in m_FaderToStartContainers)
        {
            // check if the game object contains UIFader component
            UIFader fader = faderToStartContainer.GetComponent<UIFader>();

            if (fader)
            {
                faders.Add(fader);
            }
            // if the game object does not contain UIFader component,
            // chect the game object's children
            else
            {
                faders.AddRange(faderToStartContainer.GetComponentsInChildren<UIFader>());
            }
        }

        m_FadersToStart = faders.ToArray();
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
}
