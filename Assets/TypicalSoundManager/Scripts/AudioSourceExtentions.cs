using UnityEngine;
using System.Collections;
using System;

public static class AudioSourceExtentions  {

    public static IEnumerator FadeIn(this AudioSource source, float fadeTime, Action callback)
    {
        while (source.volume > 0f)
        {
            source.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }
        callback();
    }
}
