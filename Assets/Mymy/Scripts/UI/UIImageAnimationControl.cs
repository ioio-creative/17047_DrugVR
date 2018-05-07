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

    private void Awake()
    {
        image = GetComponent<Image>();
        animationSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Color _color = image.color;
        image.sprite = animationSpriteRenderer.sprite;
        image.color = _color;
    }
}
