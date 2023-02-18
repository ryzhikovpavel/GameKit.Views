using System;
using GameKit.Views.Animators;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Views.Components
{
    [PublicAPI]
    [RequireComponent(typeof(Canvas))]
    public abstract class CanvasViewComponent : ViewComponent
    {
        private Canvas _canvas;
        private GameObject _block;

        public int SortingOrder
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder = value;
        }

        public override bool Interactable
        {
            get { return _block.activeSelf == false; }
            set
            {
                _block.SetActive(value == false);
                if (value)
                    _block.transform.SetAsFirstSibling();
                else
                    _block.transform.SetAsLastSibling();
            }
        } 
        
        internal override void Initialize()
        {
            base.Initialize();
            _canvas = GetComponent<Canvas>();
            InitializeBlockable();
        }

        private void InitializeBlockable()
        {
            _block = new GameObject("blockable");
            _block.transform.SetParent(transform, false);
            _block.layer = gameObject.layer;
            var img = _block.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            var rt = img.rectTransform;
            rt.anchorMax = new Vector2(1, 1);
            rt.anchorMin = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            rt.SetAsFirstSibling();
            img.gameObject.SetActive(false);
        }
        
        protected virtual void Reset()
        {
            var rt = (RectTransform)transform;
            rt.anchorMax = new Vector2(1, 1);
            rt.anchorMin = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            gameObject.name = GetType().Name;

            Canvas canvas = GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            AddScalerIfNotExist();
            AddRaycasterIfNotExist();
            gameObject.AddComponent<CanvasFadeViewAnimator>();

            var layer = SortingLayer.NameToID("UI");
            canvas.overrideSorting = SortingLayer.IsValid(layer);
            canvas.sortingLayerID = layer;

#if UNITY_EDITOR
            while (UnityEditorInternal.ComponentUtility.MoveComponentDown(this)) {}
#endif
        }

        private void AddScalerIfNotExist()
        {
            if (gameObject.GetComponent<CanvasScaler>() != null) return; 
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                case ScreenOrientation.PortraitUpsideDown:
                    scaler.referenceResolution = new Vector2(1024, 2048);
                    scaler.matchWidthOrHeight = 1;
                    break;
                case ScreenOrientation.LandscapeLeft:
                case ScreenOrientation.LandscapeRight:
                    scaler.referenceResolution = new Vector2(2048, 1024);
                    scaler.matchWidthOrHeight = 1;
                    break;
                case ScreenOrientation.AutoRotation:
                    scaler.referenceResolution = new Vector2(2048, 1024);
                    scaler.matchWidthOrHeight = 0.5f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddRaycasterIfNotExist()
        {
            if (gameObject.GetComponent<GraphicRaycaster>() != null) return;
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }
}