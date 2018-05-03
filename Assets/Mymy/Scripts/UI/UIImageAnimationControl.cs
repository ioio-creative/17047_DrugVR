using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(SpriteRenderer))]
public class UIImageAnimationControl : MonoBehaviour
{
    public GameObject animationSphere;
    public float frameRate;
    public string resourceFormatPath;
    public int numberOfFrames;
    public bool isRepeat;

    private Texture2D[] frames;
    private Image image;
    private SpriteRenderer animationSphereRenderer;
    private bool isFinishedPlaying = true;
    private float animationStartTime;

    private const string defaultResourceFormatPath = "blink_v2/Blink_transition_v2_{0:d5}";

    private void Awake()
    {
        //animationSphereRenderer = animationSphere.GetComponent<Renderer>();

        //if (string.IsNullOrEmpty(resourceFormatPath))
        //{
        //    resourceFormatPath = defaultResourceFormatPath;
        //}

        //frames = new Texture2D[numberOfFrames];
        //for (int i = 0; i < numberOfFrames; i++)
        //{
        //    string texturePath = string.Format(resourceFormatPath, i);
        //    frames[i] = (Texture2D)Resources.Load(texturePath, typeof(Texture2D));
        //}

        //SetAnimationFrame(0);

        image = GetComponent<Image>();
        animationSphereRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //if (!isFinishedPlaying)
        //{
        //    int currentFrame;

        //    if (isRepeat)
        //    {
        //        currentFrame = (int)(((Time.time - animationStartTime) * frameRate) % frames.Length);
        //        if (currentFrame >= frames.Length)
        //        {
        //            currentFrame = frames.Length;
        //        }

        //        SetAnimationFrame(currentFrame);
        //    }
        //    else
        //    {
        //        currentFrame = (int)(Time.time * frameRate);
        //        if (currentFrame >= frames.Length)
        //        {
        //            isFinishedPlaying = true;
        //            animationSphere.SetActive(false);
        //        }
        //        else
        //        {
        //            SetAnimationFrame(currentFrame);
        //        }

        //        //print(currentFrame);
        //    }
        //}

        image.sprite = animationSphereRenderer.sprite;
    }


    public void PlayAnimation()
    {
        isFinishedPlaying = false;
        animationStartTime = Time.time;
    }

    public void StopAnimation()
    {
        isFinishedPlaying = true;
    }

    public void StopAnimation(int frameToShowAfterStop)
    {
        StopAnimation();
        if (frameToShowAfterStop < frames.Length)
        {
            SetAnimationFrame(frameToShowAfterStop);
        }
    }

    private void SetAnimationFrame(int i)
    {
        animationSphereRenderer.material.mainTexture = frames[i];
    }
}
