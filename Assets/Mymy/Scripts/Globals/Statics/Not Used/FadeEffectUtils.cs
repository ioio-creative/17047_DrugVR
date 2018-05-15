using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FadeEffectUtils
{
    public static IEnumerator FadeIn(Renderer renderer, float duration)
    {
        yield return BeginFade(new Renderer[] { renderer }, 1f, duration);
    }

    public static IEnumerator FadeIn(Renderer[] renderers, float duration)
    {
        yield return BeginFade(renderers, 1f, duration);
    }

    public static IEnumerator FadeOut(Renderer renderer, float duration)
    {
        yield return BeginFade(new Renderer[] { renderer }, 0f, duration);
    }

    public static IEnumerator FadeOut(Renderer[] renderers, float duration)
    {
        yield return BeginFade(renderers, 0f, duration);
    }

    private static IEnumerator BeginFade(Renderer[] renderers, float finalAlpha, float duration)
    {
        Color[] initialColors = renderers.Select(x => x.material.color).ToArray();
        Color[] finalColors = initialColors.Select(x => new Color(x.r, x.g, x.b, finalAlpha)).ToArray();

        // Execute this loop once per frame until the timer exceeds the duration.
        float timer = 0f;
        while (timer <= duration)
        {
            // Set the colour based on the normalised time.
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = Color.Lerp(initialColors[i], finalColors[i], timer / duration);
                Debug.Log(renderers[i].material.color);
            }

            // Increment the timer by the time between frames and return next frame.
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
