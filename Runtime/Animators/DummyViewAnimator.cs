using System;

namespace GameKit.Views.Animators
{
    public class DummyViewAnimator : IViewAnimator
    {
        public bool IsPlaying => false;

        public void PlayShow(Action onComplete)
        {
            onComplete?.Invoke();
        }

        public void PlayHide(Action onComplete)
        {
            onComplete?.Invoke();
        }
    }
}