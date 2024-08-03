using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public float fadeDuration = 1f;
    private RawImage _imageReference;

    private void Awake()
    {
        _imageReference = this.GetComponent<RawImage>();
    }

    public void TriggerFadeOut()
    {
        StartCoroutine(FadeOut(fadeDuration));
    }
    
    public void TriggerFadeOut(float duration)
    {
        StartCoroutine(FadeOut(duration));
    }
    
    public void TriggerFadeIn()
    {
        StartCoroutine(FadeIn(fadeDuration));
    }
    
    public void TriggerFadeIn(float duration)
    {
        StartCoroutine(FadeIn(duration));
    }

    private IEnumerator FadeOut(float duration)
    {
        Color color = _imageReference.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(1, 0, elapsedTime / duration);
            _imageReference.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 0;
        _imageReference.color = color;
    }

    private IEnumerator FadeIn(float duration)
    {
        Color color = _imageReference.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(0, 1, elapsedTime / duration);
            _imageReference.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1;
        _imageReference.color = color;
    }
}
