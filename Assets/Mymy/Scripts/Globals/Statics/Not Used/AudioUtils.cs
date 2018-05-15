using System.Collections;
using System.Linq;
using UnityEngine;

public static class AudioUtils
{
    public static IEnumerator FadeOutAudio(AudioSource audioSourceToFadeOut, float duration)
    {
        yield return FadeOutAudio(new AudioSource[] { audioSourceToFadeOut }, duration);
    }

    public static IEnumerator FadeOutAudio(AudioSource[] audioSourcesToFadeOut, float duration)
    {
        if (audioSourcesToFadeOut != null && audioSourcesToFadeOut.Length > 0)
        {
            float[] audioSourceNormalVolumes = audioSourcesToFadeOut.Select(x => x.volume).ToArray();

            // Execute this loop once per frame until the timer exceeds the duration.
            float timer = 0f;
            while (timer <= duration)
            {
                // Set the volume based on the normalised time for each Audio Source To Fade.
                int i = 0;
                foreach (AudioSource audioSource in audioSourcesToFadeOut)
                {
                    audioSource.volume = Mathf.Lerp(audioSourceNormalVolumes[i++], 0f, timer / duration);
                }

                //print(audioSourcesToFadeOut[0].volume);

                // Increment the timer by the time between frames and return next frame.
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
