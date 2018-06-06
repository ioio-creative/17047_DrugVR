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
        public float FadeSpeed { get { return m_FadeSpeed; } }
        public float FadeInAlphaThreshold { get { return m_FadeInAlphaThreshold; } }
        public float FadeOutAlphaThreshold { get { return m_FadeOutAlphaThreshold; } }
        public bool Fading { get { return m_Fading; } }


        public event Action OnFadeInComplete;                   // This event is triggered when the UI elements have finished fading in.
        public event Action OnFadeOutComplete;                  // This event is triggered when the UI elements have finished fading out.

        public event Action OnFadeInReachThreshold;                    // This event is triggered when the UI elements have reached certain alpha value when fading in.
        private bool m_FadeInThresholdTriggered;                       // If OnFadeInReachThreshold is triggered
        [SerializeField]
        private float m_FadeInAlphaThreshold = 1f;    // OnFadeInReachThreshold is triggered when this value is reached.

        public event Action OnFadeOutReachThreshold;                     // This event is triggered when the UI elements have reached certain alpha value when fading out.
        private bool m_FadeOutThresholdTriggered;                        // If OnFadeOutReachThreshold is triggered
        [SerializeField]
        private float m_FadeOutAlphaThreshold = 0f;     // OnFadeOutReachThreshold is triggered when this value is reached.

        [SerializeField]
        private bool m_IsSetVisibleWhenAwake = false;

        [SerializeField]
        private float m_FadeSpeed = 1f;        // The amount the alpha of the UI elements changes per second.        
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
                    nonUiGroups.AddRange(GetComponentsInChildren<Renderer>());
                }

                m_NonUiGroupsToFade = nonUiGroups.ToArray();

                if (m_NonUiGroupsToFade.Length > 0)
                {
                    m_FadeColor = m_NonUiGroupsToFade[0].material.color;
                }
            }

            // !!! Important !!! 
            // better to call SetInvisible() in Awake() rather than in Start()
            // because I may have called StartCoroutine(UIFader.InteruptAndFadeIn()) elsewhere,
            // say in the Start() of another game object, in that case InteruptAndFadeIn() will be run
            // earlier than Start()
            if (!m_IsSetVisibleWhenAwake)
            {
                SetInvisible();
            }
            else
            {
                SetVisible();
            }
        }

        // Keep coming back each frame whilst the groups are currently fading.
        public IEnumerator WaitForFadeIn()
        {            
            while (m_Fading)
            {
                yield return null;
            }

            //yield return new WaitWhile(() => m_Fading);
            //yield return new WaitUntil(() => !m_Fading);

            // Return once the FadeIn coroutine has finished.
            yield return StartCoroutine(FadeIn());
        }

        // Stop all fading that is currently happening.
        public IEnumerator InterruptAndFadeIn()
        {           
            StopAllCoroutines();

            // Return once the FadeIn coroutine has finished.
            yield return StartCoroutine(FadeIn());
        }

        // If fading already happening then no return
        public IEnumerator CheckAndFadeIn()
        {
            // If not already fading return once the FadeIn coroutine has finished.
            if (!m_Fading)
                yield return StartCoroutine(FadeIn());
        }

        //Actual Fade In happens here
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


        public IEnumerator InterruptAndFadeOut()
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

                        Debug.Log(renderer.material.color);
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


        public void SetIsBlockRaycast(bool isBlockRaycast)
        {
            foreach (CanvasGroup uiGroup in m_UiGroupsToFade)
            {
                uiGroup.blocksRaycasts = isBlockRaycast;
            }
        }


        /* UIFader states */

        public bool IsFadingOut()
        {
            return Fading && Visible;
        }

        public bool IsFadingIn()
        {
            return Fading && !Visible;
        }

        public bool IsCompletelyFadedOut()
        {
            return !Fading && !Visible;
        }

        public bool IsCompletelyFadedIn()
        {
            return !Fading && Visible;
        }
        
        /* end of UIFader states */
    }
}
