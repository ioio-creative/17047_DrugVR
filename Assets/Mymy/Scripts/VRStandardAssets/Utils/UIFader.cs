using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    // This class is used to fade in and out groups of UI
    // elements.  It contains a variety of functions for
    // fading in different ways.
    public class UIFader : MonoBehaviour
    {
        public event Action OnFadeInComplete;                   // This event is triggered when the UI elements have finished fading in.
        public event Action OnFadeOutComplete;                  // This event is triggered when the UI elements have finished fading out.

        public event Action OnFadeInReachThreshold;                    // This event is triggered when the UI elements have reached certain alpha value when fading in.
        private bool m_FadeInThresholdTriggered;                       // If OnFadeInReachThreshold is triggered
        [SerializeField]
        public float m_FadeInAlphaThreshold = 1f;    // OnFadeInReachThreshold is triggered when this value is reached.

        public event Action OnFadeOutReachThreshold;                     // This event is triggered when the UI elements have reached certain alpha value when fading out.
        private bool m_FadeOutThresholdTriggered;                        // If OnFadeOutReachThreshold is triggered
        [SerializeField]
        public float m_FadeOutAlphaThreshold = 0f;     // OnFadeOutReachThreshold is triggered when this value is reached.

        [SerializeField]
        public float m_FadeSpeed = 1f;        // The amount the alpha of the UI elements changes per second.
        [SerializeField]
        private CanvasGroup[] m_UiGroupsToFade;  // All the groups of UI elements that will fade in and out.

        [SerializeField]
        private GameObject[] m_NonUiGroupsContainers;
        private Renderer[] m_NonUiGroupsToFade;                 // All the groups of non-UIs elements that will fade in and out.        
        private Color m_FadeColor;                              // The colour the non-UI elements' material fades out to.


        private bool m_Fading;                                  // Whether the UI elements are currently fading in or out.


        public bool Visible { get; private set; }               // Whether the UI elements are currently visible.


        public void Awake()
        {
            if (m_NonUiGroupsContainers.Length > 0)
            {
                List<Renderer> nonUiGroups = new List<Renderer>();

                foreach (GameObject nonUiGroupsContainer in m_NonUiGroupsContainers)
                {
                    foreach (Transform child in nonUiGroupsContainer.transform)
                    {
                        nonUiGroups.Add(child.GetComponent<Renderer>());
                    }
                }

                m_NonUiGroupsToFade = nonUiGroups.ToArray();

                if (m_NonUiGroupsToFade.Length > 0)
                {
                    m_FadeColor = m_NonUiGroupsToFade[0].material.color;
                }
            }
        }


        public void Start()
        {
            SetInvisible();
        }


        public IEnumerator WaitForFadeIn()
        {
            // Keep coming back each frame whilst the groups are currently fading.
            while (m_Fading)
            {
                yield return null;
            }

            // Return once the FadeIn coroutine has finished.
            yield return StartCoroutine(FadeIn());
        }


        public IEnumerator InteruptAndFadeIn()
        {
            // Stop all fading that is currently happening.
            StopAllCoroutines();

            // Return once the FadeIn coroutine has finished.
            yield return StartCoroutine(FadeIn());
        }


        public IEnumerator CheckAndFadeIn()
        {
            // If not already fading return once the FadeIn coroutine has finished.
            if (!m_Fading)
                yield return StartCoroutine(FadeIn());
        }


        public IEnumerator FadeIn()
        {
            // Fading has now started.
            m_Fading = true;
            m_FadeInThresholdTriggered = false;

            // Fading needs to continue until all the groups have finishing fading in so we need to base that on the lowest alpha.
            float lowestAlpha;

            do
            {
                // Assume the lowest alpha has faded in already.
                lowestAlpha = 1f;

                // Go through all the groups...
                for (int i = 0; i < m_UiGroupsToFade.Length; i++)
                {
                    // ... and increment their alpha based on the fade speed.
                    //m_UiGroupsToFade[i].alpha += m_FadeSpeed * Time.deltaTime;
                    m_UiGroupsToFade[i].alpha += m_FadeSpeed * Time.fixedDeltaTime;

                    // Also we need to check what the lowest alpha is.
                    if (m_UiGroupsToFade[i].alpha < lowestAlpha)
                        lowestAlpha = m_UiGroupsToFade[i].alpha;
                }

                // https://forum.unity3d.com/threads/fade-in-and-fade-out-of-a-gameobject.4723/
                // Go through all the non-UI elements...
                if (m_NonUiGroupsToFade != null)
                {
                    foreach (Renderer renderer in m_NonUiGroupsToFade)
                    {
                        // ... and increment their alpha based on the fade speed.
                        //float currentAlpha = renderer.material.color.a + m_FadeSpeed * Time.deltaTime;
                        float currentAlpha = renderer.material.color.a + m_FadeSpeed * Time.fixedDeltaTime;
                        renderer.material.color = renderer.material.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, currentAlpha);

                        // Also we need to check what the lowest alpha is.
                        if (currentAlpha < lowestAlpha)
                            lowestAlpha = currentAlpha;
                    }
                }

                if (!m_FadeInThresholdTriggered && m_FadeInAlphaThreshold < 1f && lowestAlpha > m_FadeInAlphaThreshold)
                {
                    m_FadeInThresholdTriggered = true;

                    if (OnFadeInReachThreshold != null)
                        OnFadeInReachThreshold();
                }

                // Wait until next frame.
                yield return null;
            }
            // Continue doing this until the lowest alpha is one or greater.
            while (lowestAlpha < 1f);

            // If there is anything subscribed to OnFadeInComplete, call it.
            if (OnFadeInComplete != null)
                OnFadeInComplete();

            // Fading has now finished.
            m_Fading = false;

            // Since everthing has faded in now, it is visible.
            Visible = true;
        }


        // The following functions are identical to the previous ones but fade the CanvasGroups out instead.
        public IEnumerator WaitForFadeOut()
        {
            while (m_Fading)
            {
                yield return null;
            }

            yield return StartCoroutine(FadeOut());
        }


        public IEnumerator InteruptAndFadeOut()
        {
            StopAllCoroutines();
            yield return StartCoroutine(FadeOut());
        }


        public IEnumerator CheckAndFadeOut()
        {
            if (!m_Fading)
                yield return StartCoroutine(FadeOut());
        }


        public IEnumerator FadeOut()
        {
            m_Fading = true;
            m_FadeOutThresholdTriggered = false;

            float highestAlpha;

            do
            {
                highestAlpha = 0f;

                for (int i = 0; i < m_UiGroupsToFade.Length; i++)
                {
                    //m_UiGroupsToFade[i].alpha -= m_FadeSpeed * Time.deltaTime;
                    m_UiGroupsToFade[i].alpha -= m_FadeSpeed * Time.fixedDeltaTime;

                    if (m_UiGroupsToFade[i].alpha > highestAlpha)
                        highestAlpha = m_UiGroupsToFade[i].alpha;
                }

                if (m_NonUiGroupsToFade != null)
                {
                    foreach (Renderer renderer in m_NonUiGroupsToFade)
                    {
                        //float currentAlpha = renderer.material.color.a - m_FadeSpeed * Time.deltaTime;
                        float currentAlpha = renderer.material.color.a - m_FadeSpeed * Time.fixedDeltaTime;
                        renderer.material.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, currentAlpha);

                        if (currentAlpha > highestAlpha)
                            highestAlpha = currentAlpha;
                    }
                }

                if (!m_FadeOutThresholdTriggered && m_FadeOutAlphaThreshold > 0f && highestAlpha < m_FadeOutAlphaThreshold)
                {
                    m_FadeOutThresholdTriggered = true;

                    if (OnFadeOutReachThreshold != null)
                        OnFadeOutReachThreshold();
                }

                yield return null;
            }
            while (highestAlpha > 0f);

            if (OnFadeOutComplete != null)
                OnFadeOutComplete();

            m_Fading = false;

            Visible = false;
        }


        // These functions are used if fades are required to be instant.
        public void SetVisible()
        {
            for (int i = 0; i < m_UiGroupsToFade.Length; i++)
            {
                m_UiGroupsToFade[i].alpha = 1f;
            }

            if (m_NonUiGroupsToFade != null)
            {
                foreach (Renderer renderer in m_NonUiGroupsToFade)
                {
                    renderer.material.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 1f);
                }
            }

            Visible = true;
        }


        public void SetInvisible()
        {
            for (int i = 0; i < m_UiGroupsToFade.Length; i++)
            {
                m_UiGroupsToFade[i].alpha = 0f;
            }

            if (m_NonUiGroupsToFade != null)
            {
                foreach (Renderer renderer in m_NonUiGroupsToFade)
                {
                    renderer.material.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 0f);
                }
            }

            Visible = false;
        }
    }
}
