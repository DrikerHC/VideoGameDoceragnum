using System.Collections;
using UnityEngine;
using Utils;

namespace SceneController
{
    internal class LoadingOverlay : Singleton<LoadingOverlay>
    {

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeInTime = .5f;
        [SerializeField] private float fadeOutTime = .5f;

        public IEnumerator FadeInBlack()
        {
            yield return FadeTo(1, fadeInTime);
        }
        public IEnumerator FadeOutBlack()
        {
            yield return FadeTo(0, fadeOutTime);
        }

        private IEnumerator FadeTo(float targetAlpha, float time)
        {
            var startAlpha = canvasGroup.alpha;
            var elapsedTime = 0f;
            while (elapsedTime < time)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime/time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;

        }

    }
}