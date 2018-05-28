using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Video;

namespace Scene07Party
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PartyVFXAnimationControl : MonoBehaviour {

        //private static IDictionary<PartyOptionEnum, SphereAnimationSeqPackage> PartyFXDictionary = new Dictionary<PartyOptionEnum, SphereAnimationSeqPackage>(new PartyEnumComparer());
        //private static IDictionary<PartyOptionEnum, VideoClip> PartyFXDictionary = new Dictionary<PartyOptionEnum, VideoClip>(new PartyEnumComparer());
        private static IDictionary<PartyOptionEnum, object> PartyFXDictionary = new Dictionary<PartyOptionEnum, object>(new PartyEnumComparer());

        [Serializable]
        public class SphereAnimationSeqPackage
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
            public float FrameRate = 30;
            [SerializeField]
            private int numberOfFrames;

            public bool IsRepeat;

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
                PartyFXDictionary.Add(m_PartyOption, this);
                Debug.Log(m_PartyOption + " FX Loading");
                for (int i = 0; i < numberOfFrames; i++)
                {
                    //Folder Frame Index Starts with <00001>
                    //----------------------------------------
                    //Load File From Resources Folder
                    //----------------------------------------
                    Frames[i] = Resources.Load<Texture2D>(string.Format(resourceFormatPath, i + 1));
                    yield return null;
                    //------------------------------------------
                    //------------------------------------------

                    //----------------------------------------
                    //Load File From Persistant Datapath
                    //----------------------------------------

                    //string texturePath = GameManager.APP_IMGSEQUENCE_DATA_PATH + string.Format(resourceFormatPath, i+1);                 

                    //// http://answers.unity3d.com/questions/432655/loading-texture-file-from-pngjpg-file-on-disk.html
                    //Texture2D _imgTex = null;
                    //byte[] fileData;

                    //fileData = File.ReadAllBytes(texturePath);
                    //_imgTex = new Texture2D(2, 2);
                    //_imgTex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                    //Frames[i] = _imgTex;
                    //yield return null;
                    //------------------------------------------
                    //------------------------------------------

                    //----------------------------------------
                    //Load File From Streaming Assets
                    //----------------------------------------
                    ////https://www.cnblogs.com/murongxiaopifu/p/4199541.html
                    //string texturePath = Application.streamingAssetsPath + "/" + string.Format(resourceFormatPath, i + 1);
                    //using (WWW www = new WWW(texturePath))
                    //{ 
                    //    yield return www;
                    //    Frames[i] = www.texture; 
                    //}
                    //------------------------------------------
                    //------------------------------------------
                }

                Debug.Log(m_PartyOption + " FX Loading Completed");
            }
        }

        [Serializable]
        public class SphereVideoPackage
        {
            /*
                Reference of Alpha Video import in Unity:
                http://tsubakit1.hateblo.jp/entry/2017/03/20/134727 
            */

            private PartyVFXAnimationControl AnimCtrlRef;
            [SerializeField]
            private PartyOptionEnum m_PartyOption;
            public VideoClip FXVideoClip;
            [SerializeField]
            [Range(0, 360)]
            public float EffectRotation;
            public bool IsRepeat;
           
            private bool m_IsFinishedPlaying = true;

            public void InitializeSphereVideoPackage(PartyVFXAnimationControl partyVFXAnimationControl)
            {
                AnimCtrlRef = partyVFXAnimationControl;
                PartyFXDictionary.Add(m_PartyOption, this);
                Debug.Log(m_PartyOption + " FX Loading Completed");
            }
        }

        private bool m_IsPlaying;
        public bool IsPlaying
        {
            get { return m_IsPlaying; }
            set { m_IsPlaying = value; }
        }
        [SerializeField]
        private MeshRenderer m_SphereMeshRenderer;
        [SerializeField]
        private VideoPlayer m_FXVideoPlayer;

        [SerializeField]
        private SphereAnimationSeqPackage[] m_SphereImgSequences;
        private const string defaultResourceFormatPath = "s7-02a-once/B_{0:d5}";

        [SerializeField]
        private SphereVideoPackage[] m_SphereVideos;
        [SerializeField]
        private RenderTexture m_SphereVideoTexture;

        public event Action OnFXPlay;
        public event Action OnFXEnd;

        private void Awake()
        {
            if (!m_SphereMeshRenderer)
            {
                m_SphereMeshRenderer = GetComponent<MeshRenderer>();              
            }
            m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);

            if (!m_FXVideoPlayer)
            {
                m_FXVideoPlayer = GetComponent<VideoPlayer>();              
            }
            m_FXVideoPlayer.source = VideoSource.VideoClip;
            m_FXVideoPlayer.skipOnDrop = true;

            if (m_SphereVideoTexture != null)
            {
                GetComponent<MeshRenderer>().material.SetTexture("_MainTex", m_SphereVideoTexture);
            }
        }

        private void Start()
        {
            foreach (SphereAnimationSeqPackage VFXPackage in m_SphereImgSequences)
            {
                StartCoroutine(VFXPackage.InitializeSphereAnimationPackage(this));
            }

            foreach (SphereVideoPackage VFXPackage in m_SphereVideos)
            {
                VFXPackage.InitializeSphereVideoPackage(this);
            }        
        }

        public void PlayPartyVFX(PartyOptionEnum FXType)
        {          
            if (PartyFXDictionary[FXType] is SphereAnimationSeqPackage)
            {
                StartCoroutine(PlayFXAnimSeq(PartyFXDictionary[FXType] as SphereAnimationSeqPackage));
            }
            else if (PartyFXDictionary[FXType] is SphereVideoPackage)
            {
                StartCoroutine(PlayFXVideo(PartyFXDictionary[FXType] as SphereVideoPackage));
            }
        }

        private IEnumerator PlayFXAnimSeq(SphereAnimationSeqPackage sphereAnim)
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
        public void StopAnimationSeq(SphereAnimationSeqPackage sphereAnim)
        {
            sphereAnim.IsFinishedPlaying = true;
            if (OnFXEnd != null)
            {
                OnFXEnd();
            }
        }
        public void StopAnimationSeq(SphereAnimationSeqPackage sphereAnim, int frameToShowAfterStop)
        {
            StopAnimationSeq(sphereAnim);
            if (frameToShowAfterStop < sphereAnim.Frames.Length)
            {
                SetAnimationFrame(sphereAnim, frameToShowAfterStop);
            }
            m_SphereMeshRenderer.material.SetFloat("_Transparency", 1);
        }
        private void SetAnimationFrame(SphereAnimationSeqPackage sphereAnim, int i)
        {
            m_SphereMeshRenderer.material.SetTexture("_MainTex", sphereAnim.Frames[i]);
        }


        private IEnumerator PlayFXVideo(SphereVideoPackage sphereVideo)
        {
            if (!m_FXVideoPlayer.isPlaying)
            {
                m_FXVideoPlayer.clip = sphereVideo.FXVideoClip;
                m_FXVideoPlayer.Prepare();
                yield return m_FXVideoPlayer.isPrepared;

                Quaternion FXRotation = Quaternion.Euler(0, sphereVideo.EffectRotation, 0);
                m_SphereMeshRenderer.transform.rotation = FXRotation;
                
                m_FXVideoPlayer.loopPointReached += HandleFXVideoEnd;
                m_FXVideoPlayer.Play();
                m_SphereMeshRenderer.material.SetFloat("_Transparency", 1);
                if (OnFXPlay != null)
                {
                    OnFXPlay();
                } 
            }
        }
        public void StopFXVideo()
        {
            m_FXVideoPlayer.Stop();
            m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);
            if (OnFXEnd != null)
            {
                OnFXEnd();
            }
            m_FXVideoPlayer.loopPointReached -= HandleFXVideoEnd;
        }
        private void HandleFXVideoEnd(VideoPlayer source)
        {
            m_SphereMeshRenderer.material.SetFloat("_Transparency", 0);
            source.Stop();
            if (OnFXEnd != null)
            {
                OnFXEnd();
            }
            source.loopPointReached -= HandleFXVideoEnd;
        }
    }
}
