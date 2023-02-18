using System;
using UnityEngine;

namespace GameKit.Views.Components
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaRect : MonoBehaviour
    {
        [Flags]
        private enum Direction
        {
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8
        }
        
        [SerializeField] private Direction direction;
        [SerializeField] private bool scaleFitHeight;
        private RectTransform _rectTransform;
        private Rect _safeArea;
        private bool _ready;

        private void Awake()
        {
            _rectTransform = (RectTransform) transform;
        }

        private void OnEnable()
        {
            _ready = false;
        }

        private void Sync(Rect safeArea)
        {
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            if (direction.HasFlag(Direction.Left))
                anchorMin.x /= Screen.width;
            else
                anchorMin.x = 0;

            if (direction.HasFlag(Direction.Right))
                anchorMax.x /= Screen.width;
            else
                anchorMax.x = 1;

            if (direction.HasFlag(Direction.Top))
                anchorMax.y /= Screen.height;
            else
                anchorMax.y = 1;

            if (direction.HasFlag(Direction.Bottom))
                anchorMin.y /= Screen.height;
            else
                anchorMin.y = 0;

            if (scaleFitHeight == false)
            {
                _rectTransform.anchorMin = anchorMin;
                _rectTransform.anchorMax = anchorMax;
            }
            else
            {
                float s = anchorMax.y - anchorMin.y;
                _rectTransform.localScale = new Vector3(s, s, 1);
                if (anchorMin.y < (1 - anchorMax.y))
                {
                    anchorMax.y += anchorMin.y;
                    anchorMin.y = 0;
                }
                else
                {
                    anchorMin.y -= (1 - anchorMax.y);
                    anchorMax.y = 1;
                }

                _rectTransform.anchorMax = anchorMax;
                _rectTransform.anchorMin = anchorMin;
            }
        }

        private void Update()
        {
            if (_safeArea == Screen.safeArea && _ready) return;
            
            _safeArea = Screen.safeArea;
            if (float.IsNaN(_safeArea.x) || float.IsInfinity(_safeArea.x) || float.IsNaN(_safeArea.y) ||
                float.IsInfinity(_safeArea.y))
            {
                _ready = false;
                return;
            }
            
            _ready = true;
            Sync(_safeArea);
        }

        private void Reset()
        {
            if (transform is RectTransform rt)
            {
                rt.anchorMax = new Vector2(1, 1);
                rt.anchorMin = new Vector2(0, 0);
                rt.offsetMin = new Vector2(0, 0);
                rt.offsetMax = new Vector2(0, 0);
            }

            var canvas = GetComponent<Canvas>();
            if (canvas != null && canvas.isRootCanvas)
            {
                Debug.LogError($"{nameof(SafeAreaRect)} dont work with Root Canvas");
            }
        }
    }
}