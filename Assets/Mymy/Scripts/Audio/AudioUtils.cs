using System.Collections;
using System.Linq;
using UnityEngine;

public static class AudioUtils
{
    //REMARKS: If cannot start coroutine add MonoBehaviour to Func params

    public static IEnumerator FadeInAudioToOne(AudioSource audioSourceToFadeIn, float duration)
    {
        yield return FadeInAudioToOne(new AudioSource[] { audioSourceToFadeIn }, duration);
    }

    public static IEnumerator FadeInAudioToOne(AudioSource[] audioSourcesToFadeIn, float duration)
    {
        float[] oneVolumes = audioSourcesToFadeIn.Select(x => 1f).ToArray();
        yield return FadeAudioToTargetVolumes(audioSourcesToFadeIn, oneVolumes, duration);
    }

    public static IEnumerator FadeOutAudioToZero(AudioSource audioSourceToFadeOut, float duration)
    {
        yield return FadeOutAudioToZero(new AudioSource[] { audioSourceToFadeOut }, duration);
    }

    public static IEnumerator FadeOutAudioToZero(AudioSource[] audioSourcesToFadeOut, float duration)
    {
        float[] zeroVolumes = audioSourcesToFadeOut.Select(x => 0f).ToArray();
        yield return FadeAudioToTargetVolumes(audioSourcesToFadeOut, zeroVolumes, duration);
    }

    public static IEnumerator FadeAudioToTargetVolume(AudioSource[] audioSrcsToFade, float endVol, float duration) 
    {
        float[] endVolumes = audioSrcsToFade.Select(x => endVol).ToArray();
        yield return FadeAudioToTargetVolumes(audioSrcsToFade, endVolumes, duration);
    }

    public static IEnumerator FadeAudioToTargetVolumes(AudioSource[] audioSrcs, float[] endVols, float duration)
    {
        if (audioSrcs != null && audioSrcs.Length > 0)
        {
            float[] iniVolumes = audioSrcs.Select(x => x.volume).ToArray();

            // Execute this loop once per frame until the timer exceeds the duration.
            float timer = 0f;
            while (timer <= duration)
            {
                int i = 0;
                foreach (AudioSource audioSrc in audioSrcs)
                {
                    // Set the volume based on the normalised time.
                    audioSrc.volume = Mathf.Lerp(iniVolumes[i], endVols[i], timer / duration);
                    i++;
                }
                // Increment the timer by the time between frames and return next frame.
                timer += Time.deltaTime;
                yield return null;
            }
        }

    }
}
