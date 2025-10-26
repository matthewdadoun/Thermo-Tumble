using System;
using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [Header("References")]
    
    // The canvas group to use to control the alpha
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool useUnscaledTime = true;

    // 🔹 Event triggered when any fade finishes
    public event Action OnFadeComplete;

    private Coroutine _fadeRoutine;

    private void Awake()
    {
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Begin fade in from black
        FadeInFromBlack(3f);

        // Set the fade controller instance
        GameManager.Instance.FadeControllerInstance = this;
    }

    public void FadeTo(float targetAlpha, float duration, Action onComplete = null)
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        _fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    public void FadeOutToBlack(float duration = 1f)
        => FadeTo(1f, duration);

    public void FadeInFromBlack(float duration = 1f)
        => FadeTo(0f, duration);

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;
        canvasGroup.blocksRaycasts = (targetAlpha > 0f || startAlpha > 0f);

        while (t < duration)
        {
            // Calculate the progress
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            
            // Store the progress
            float p = Mathf.Clamp01(t / duration);
            
            // Lerp the alpha
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curve.Evaluate(p));
            
            // yield return null
            yield return null;
        }

        // Set the target alpha
        SetAlpha(targetAlpha);
        
        // null out fade routine
        _fadeRoutine = null;
        
        OnFadeComplete?.Invoke();
    }

    private void SetAlpha(float a)
    {
        canvasGroup.alpha = a;
        canvasGroup.blocksRaycasts = a > 0.001f;
    }
}