using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Scene07Party
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PartyVFXAnimationControl : MonoBehaviour {

        private static IDictionary<PartyOptionEnum, SphereAnimationPackage> PartyFXDictionary = new Dictionary<PartyOptionEnum, SphereAnimationPackage>(new PartyEnumComparer());

        [Serializable]
        public class SphereAnimationPackage
        {
            private PartyVFXAnimationControl AnimCtrlRef;

            [SerializeField]
            private PartyOptionEnum m_PartyOption;
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

            private bool m_IsReady = false;

            public Texture2D[] Frames { get; private set; }
            public bool IsFinishedPlaying
            {
                get { return m_IsFinishedPlaying; }
                set
                {
                    AnimCtrlRef.IsPlaying = !value;
                    m_IsFinishedPlaying = value;
                }
            }
            private bool m_IsFinishedPlaying = true;

            public IEnumerator InitializeSphereAnimationPackage(PartyVFXAnimationControl ctrl)
            {
                AnimCtrlRef = ctrl;

                if (string.IsNullOrEmpty(resourceFormatPath))
                {
                    resourceFormatPath = defaultResourceFormatPath;
                }

                Frames = new Texture2D[numberOfFrames];
                
                for (int i = 0; i < numberOfFrames; i++)
                {
                    //Folder Frame Index Starts with <00001>
                    string texturePath = GameManager.APP_IMGSEQUENCE_DATA_PATH + string.Format(resourceFormatPath, i+1);                 

                    // http://answers.unity3d.com/questions/432655/loading-texture-file-from-pngjpg-file-on-disk.html
                    Texture2D _imgTex = null;
                    byte[] fileData;
            
                    fileData = File.ReadAllBytes(texturePath);
                    _imgTex = new Texture2D(2, 2);
                    _imgTex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                    Frames[i] = _imgTex;
                    yield return null;
                }

                PartyFXDictionary.Add(m_PartyOption, this);
                
            }
        }

        private bool m_IsPlaying;
        public bool IsPlaying
        {
            get { return m_IsPlaying; }
            set { m_IsPlaying = value; }
        }

        [SerializeField]
        private SphereAnimationPackage[] m_SphereVFXAnimations = new SphereAnimationPackage[3];
        [SerializeField]
        private MeshRenderer m_SphereMeshRenderer;

        private const string defaultResourceFormatPath = "s7-02a-once/B_{0:d5}";

        public event Action OnFXPlay;
        public event Action OnFXEnd;

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
                StartCoroutine(VFXPackage.InitializeSphereAnimationPackage(this));
            }
            m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);
            SetAnimationFrame(m_SphereVFXAnimations[0], 0);
        }

        public void PlayPartyVFX(PartyOptionEnum FXType)
        {
            StartCoroutine(PlayFXAnim(PartyFXDictionary[FXType]));          
        }

        private IEnumerator PlayFXAnim(SphereAnimationPackage sphereAnim)
        {
            if (!m_IsPlaying)
            {
                if (OnFXPlay != null)
                {
                    OnFXPlay();
                }

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
                    m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);

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
                        if (sphereAnim.IsFinishedPlaying) break;
                        if (currentFrame == 0) continue;
                        SetAnimationFrame(sphereAnim, currentFrame);
                        yield return new WaitForSeconds(frameLength);
                    }
                    sphereAnim.IsFinishedPlaying = true;
                    m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);
                    if (OnFXEnd != null)
                    {
                        OnFXEnd();
                    }

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
        }

        public void StopAnimation(SphereAnimationPackage sphereAnim)
        {
            sphereAnim.IsFinishedPlaying = true;
            if (OnFXEnd != null)
            {
                OnFXEnd();
            }
        }

        public void StopAnimation(SphereAnimationPackage sphereAnim, int frameToShowAfterStop)
        {
            StopAnimation(sphereAnim);
            if (frameToShowAfterStop < sphereAnim.Frames.Length)
            {
                SetAnimationFrame(sphereAnim, frameToShowAfterStop);
            }
            m_SphereMeshRenderer.material.SetFloat("_Transparency", 1);
        }

        private void SetAnimationFrame(SphereAnimationPackage sphereAnim, int i)
        {
            m_SphereMeshRenderer.material.SetTexture("_MainTex", sphereAnim.Frames[i]);
        }

    }
}
