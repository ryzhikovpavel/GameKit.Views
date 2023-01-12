using System;

namespace GameKit.Views.Animators
{
    public interface IViewAnimator
    {
        bool IsPlaying { get; }
        void PlayShow(Action onComplete);
        void PlayHide(Action onComplete);
    }
}