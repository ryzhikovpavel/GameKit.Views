using GameKit.Views.Animators;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit.Views.Components
{
    [PublicAPI]
    public abstract class ViewComponent : MonoBehaviour, IViewComponent
    {
        public IViewAnimator Animator { get; private set; }
        public abstract bool Interactable { get; set; }
        public virtual bool IsDisplayed => this != null && gameObject.activeSelf;

        internal bool IsPulled;
        
        internal virtual void Initialize()
        {
            Animator = GetComponent<IViewAnimator>() ?? new DummyViewAnimator();
        }

        protected virtual void Activate()
        {
            gameObject.SetActive(true);
            Interactable = false;
            Animator.PlayShow(OnDisplayed);
        }

        public void Hide()
        {
            if (IsDisplayed == false) return;
            OnHide();
            Interactable = false;
            Animator.PlayHide(gameObject.HideObject);
        }

        public void Release()
        {
            if (this == null) return;
            if (IsDisplayed)
            {
                OnHide();
                Interactable = false;
                Animator.PlayHide(PushToPool);
                return;
            }

            PushToPool();
        }

        private void PushToPool()
        {
            IsPulled = false;
            OnRelease();
        }

        protected virtual void OnDisplayed()
        {
            Interactable = true;
        }

        protected virtual void OnHide() { }
        protected virtual void OnRelease() { }
    }
}