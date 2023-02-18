using System;
using System.Collections;
using UnityEngine;

namespace GameKit.Views.Animators
{
    [RequireComponent(typeof(Animator))]
    internal class MecanimViewAnimator : MonoBehaviour, IViewAnimator
    {
        private const string AnimationDefaultName = "default";
        private enum Type
        {
            Animation,
            Trigger
        }

        [SerializeField] private Type animationType = Type.Animation;
        [SerializeField] private string animationShowName;
        [SerializeField] private string animationHideName;
        private Animator animator;
        private Coroutine routine;

        public bool IsPlaying => routine != null;

        public void PlayShow(Action onComplete)
        {
            Play(animationShowName);
            if (routine != null)
            {
                Debug.LogWarning(
                    $"{gameObject.name} replace animation on Show. Completion events from past animation will not be dispatched");
                StopCoroutine(routine);
            }

            routine = StartCoroutine(WaitCompleteThenEvent(onComplete));
        }

        public void PlayHide(Action onComplete)
        {
            Play(animationHideName);
            if (routine != null)
            {
                Debug.LogWarning(
                    $"{gameObject.name} replace animation on Hide. Completion events from past animation will not be dispatched");
                StopCoroutine(routine);
            }

            routine = StartCoroutine(WaitCompleteThenEvent(onComplete));
        }

        private void Play(string value)
        {
            switch (animationType)
            {
                case Type.Animation:
                    animator.Play(value);
                    break;
                case Type.Trigger:
                    animator.SetTrigger(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator WaitCompleteThenEvent(Action onComplete)
        {
            yield return null;
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length);
            routine = null;
            yield return null;
            onComplete?.Invoke();
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            if (animationType == Type.Trigger)
            {
                animator.ResetTrigger(animationShowName);
                animator.ResetTrigger(animationHideName);
            }
        }
        
#if UNITY_EDITOR
        [ContextMenu("CreateAnimator")]
        private void CreateAnimator()
        {
            // Creates the controller
            var path = UnityEditor.EditorUtility.SaveFilePanel("Create animator controller", Application.dataPath, name, "controller");
            if (string.IsNullOrEmpty(path)) return;
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path.Remove(0, Application.dataPath.Length - 6));
            
            var animationDefault = new AnimationClip() { name = AnimationDefaultName };
            var animationShow = new AnimationClip() { name = animationShowName };
            var animationHide = new AnimationClip() { name = animationHideName };
            
            UnityEditor.AssetDatabase.AddObjectToAsset( animationDefault, controller );
            UnityEditor.AssetDatabase.AddObjectToAsset( animationShow, controller );
            UnityEditor.AssetDatabase.AddObjectToAsset( animationHide, controller );
            
            controller.AddMotion(animationDefault);
            controller.AddMotion(animationShow);
            controller.AddMotion(animationHide);

            GetComponent<Animator>().runtimeAnimatorController = controller;
            
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        private void Reset()
        {
            animationShowName = "show";
            animationHideName = "hide";
            if (GetComponent<CanvasGroup>() == false)
                gameObject.AddComponent<CanvasGroup>();
        }
#endif
    }
}