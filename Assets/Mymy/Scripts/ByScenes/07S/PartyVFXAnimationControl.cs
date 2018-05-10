using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scene07Party
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PartyVFXAnimationControl : MonoBehaviour {

        private static IDictionary<PartyOptionEnum, SphereAnimationPackage> PartyFXDictionary = new Dictionary<PartyOptionEnum, SphereAnimationPackage>(new PartyEnumComparer());

        [Serializable]
        public class SphereAnimationPackage
        {
            [SerializeField]
            private PartyOptionEnum PartyOption;
            [SerializeField]
            private string resourceFormatPath;
            [SerializeField]
            [Range(0, 360)]
            public float EffectRotation;
            [SerializeField]
            public float FrameRate;
            [SerializeField]
            private int numberOfFrames;
            [SerializeField]
            public bool IsRepeat;

            public Texture2D[] Frames { get; private set; }
            public bool IsFinishedPlaying
            {
                get { return m_IsFinishedPlaying; }
                set { m_IsFinishedPlaying = value; }
            }
            private bool m_IsFinishedPlaying = true;
            public float AnimationStartTime { get; set; }

            public void InitializeSphereAnimationPackage()
            {
                if (string.IsNullOrEmpty(resourceFormatPath))
                {
                    resourceFormatPath = defaultResourceFormatPath;
                }

                Frames = new Texture2D[numberOfFrames];
                
                for (int i = 0; i < numberOfFrames; i++)
                {
                    //Folder Frame Index Starts with <00001>
                    string texturePath = string.Format(resourceFormatPath, i+1);
                    Frames[i] = Resources.Load<Texture2D>(texturePath);
                }

                PartyFXDictionary.Add(PartyOption, this);
            }
        }


        [SerializeField]
        private SphereAnimationPackage[] m_SphereVFXAnimations = new SphereAnimationPackage[3];
        [SerializeField]
        private MeshRenderer m_SphereMeshRenderer;

        private const string defaultResourceFormatPath = "s7-02a-once/B_{0:d5}";

        private void Awake()
        {
            if (!m_SphereMeshRenderer)
            {
                m_SphereMeshRenderer = GetComponent<MeshRenderer>();
            }

            
        }

        private void Start()
        {
            foreach (SphereAnimationPackage VFXPackage in m_SphereVFXAnimations)
            {
                VFXPackage.InitializeSphereAnimationPackage();
            }
            m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);
            SetAnimationFrame(m_SphereVFXAnimations[0], 0);
        }

        public IEnumerator PlayPartyVFX(PartyOptionEnum FXType)
        {
            yield return StartCoroutine(PlayFXAnim(PartyFXDictionary[FXType]));          
        }

        private IEnumerator PlayFXAnim(SphereAnimationPackage sphereAnim)
        {
            if (sphereAnim.IsFinishedPlaying)
            {
                int currentFrame = 0;
                float frameLength = 1.0f / sphereAnim.FrameRate;
                Quaternion FXRotation = Quaternion.Euler(0, sphereAnim.EffectRotation, 0);
                m_SphereMeshRenderer.transform.rotation = FXRotation;
                m_SphereMeshRenderer.material.SetFloat("_Transparency", 1);

                if (sphereAnim.IsRepeat)
                {
                    //sphereAnim.AnimationStartTime = Time.time;
                    sphereAnim.IsFinishedPlaying = false;

                    while (!sphereAnim.IsFinishedPlaying)
                    {
                        if (currentFrame == 0) continue;
                        SetAnimationFrame(sphereAnim, currentFrame % sphereAnim.Frames.Length);
                        currentFrame++;
                        yield return new WaitForSeconds(frameLength);
                    }
                    sphereAnim.IsFinishedPlaying = true;
                    //currentFrame = (int)(((Time.time - sphereAnim.AnimationStartTime) * sphereAnim.FrameRate) % sphereAnim.Frames.Length);
                    //if (currentFrame >= sphereAnim.Frames.Length)
                    //{
                    //    currentFrame = sphereAnim.Frames.Length;
                    //}

                }
                else
                {
                    sphereAnim.IsFinishedPlaying = false;
                    for (; currentFrame < sphereAnim.Frames.Length; currentFrame++)
                    {
                        if (currentFrame == 0) continue;
                        SetAnimationFrame(sphereAnim, currentFrame);
                        yield return new WaitForSeconds(frameLength);
                    }

                    sphereAnim.IsFinishedPlaying = true;
                    m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);

                    //currentFrame = (int)(Time.time * frameRate);
                    //if (currentFrame >= frames.Length)
                    //{
                    //    isFinishedPlaying = true;
                    //    animationSphere.SetActive(false);
                    //}
                    //else
                    //{
                    //    SetAnimationFrame(currentFrame);
                    //}

                    //print(currentFrame);
                }
            }

            //yield return null;
        }

        public void StopAnimation(SphereAnimationPackage sphereAnim)
        {
            sphereAnim.IsFinishedPlaying = true;
        }

        public void StopAnimation(SphereAnimationPackage sphereAnim, int frameToShowAfterStop)
        {
            StopAnimation(sphereAnim);
            if (frameToShowAfterStop < sphereAnim.Frames.Length)
            {
                SetAnimationFrame(sphereAnim, frameToShowAfterStop);
            }
        }

        private void SetAnimationFrame(SphereAnimationPackage sphereAnim, int i)
        {
            m_SphereMeshRenderer.material.SetTexture("_MainTex", sphereAnim.Frames[i]);
        }

        public IEnumerator HandlePartyOptionOnSelected(PartySelection partyOption)
        {
            yield return null;
        }
    }
}
