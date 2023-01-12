using System;
using System.Collections;
using UnityEngine;

namespace GameKit.Views.Animators
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasFadeViewAnimator : MonoBehaviour, IViewAnimator
    {
        [Range(0f, 5f), SerializeField] private float showDuration = 0.2f;
        [Range(0f, 5f), SerializeField] private float hideDuration = 0.2f;
        private CanvasGroup _canvas;
        private Coroutine _process;
        
        public void PlayShow() => PlayShow(null);
        public void PlayHide() => PlayHide(null);
        public bool IsPlaying { get; set; }

        public void PlayShow(Action onComplete)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            if (_process != null) StopCoroutine(_process);
            _process = StartCoroutine(Fade(1, showDuration, onComplete));
        }
        public void PlayHide(Action onComplete)
        {
            StopAllCoroutines();
            if (gameObject.activeSelf)
                _process = StartCoroutine(Fade(0, hideDuration, onComplete));
            else
            {
                onComplete?.Invoke();
            }
        }

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroup>();
            _canvas.alpha = 0;
        }

        private void OnDisable()
        {
            IsPlaying = false;
        }

        private IEnumerator Fade(int target, float duration, Action onComplete)
        {
            int direction = target == 0 ? -1 : 1;
            IsPlaying = true;
            float value = _canvas.alpha;

            while (duration > 0.01f && ((target == 0 && value > 0) || (target == 1 && value < 1)))
            {
                if (_canvas == null) yield break;
                value += direction * Time.unscaledDeltaTime / duration;
                _canvas.alpha = value;
                yield return null;
            }
            _canvas.alpha = target;
            _process = null;
            IsPlaying = false;
            onComplete?.Invoke();
        }
    }
}