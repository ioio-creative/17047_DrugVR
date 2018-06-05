using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(SpriteRenderer))]
public class UIImageAnimationControl : MonoBehaviour
{
    private Image image;
    private SpriteRenderer animationSpriteRenderer;
    private Animator anim;
    [SerializeField]
    private bool playAnimation;

    private void Awake()
    {
        image = GetComponent<Image>();
        animationSpriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playAnimation)
        {
            Color _color = image.color;
            image.sprite = animationSpriteRenderer.sprite;
            image.color = _color; 
        }
    }

    public void ActivateUIAnimation()
    {
        playAnimation = true;
        SetAnimBool();
    }

    public void DeactivateUIAnimation()
    {
        playAnimation = false;
        SetAnimBool();
    }

    private void SetAnimBool()
    {
        anim.SetBool("Activated", playAnimation);
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
}
