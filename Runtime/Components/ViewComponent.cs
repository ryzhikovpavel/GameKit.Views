using GameKit.Views.Animators;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit.Views.Components
{
    [PublicAPI]
    public abstract class ViewComponent : MonoBehaviour
    {
        public IViewAnimator Animator { get; private set; }
        public abstract bool Interactable { get; set; }
        public virtual bool IsDisplayed => this != null && gameObject.activeSelf;

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
            OnHide();
            Interactable = false;
            Animator.PlayHide(gameObject.HideObject);
        }

        protected virtual void OnDisplayed()
        {
            Interactable = true;
        }

        protected virtual void OnHide() { }
        public virtual void OnRelease() { }
    }
}