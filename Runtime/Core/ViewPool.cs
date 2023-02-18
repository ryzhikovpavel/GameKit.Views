using System;
using System.Collections.Generic;
using GameKit.Views.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Views.Core
{
    public class ViewPool
    {
        private readonly List<ViewComponent> _instances;
        private readonly GameObject _prefab;

        public ViewPool(ViewComponent prefab)
        {
            _instances = new List<ViewComponent>(1);
            _prefab = prefab.gameObject;
        }

        public ViewComponent Pull()
        {
            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                var view = _instances[i];
                if (view.gameObject == null)
                {
                    _instances.RemoveAt(i);
                    continue;
                }
                
                if (view.IsPulled) continue;
                return view;
            }

            var newView = InstantiateView();
            _instances.Add(newView);
            return newView;
        }
        
        private ViewComponent InstantiateView()
        {
            if (_prefab == null)
                throw new Exception($"Prefab not found");

            _prefab.SetActive(false);
            GameObject obj = Object.Instantiate(_prefab);
            
#if UNITY_EDITOR
            _prefab.SetActive(true);
#endif
            
            ViewComponent view = obj.GetComponent<ViewComponent>();
            view.name = view.GetType().Name;
            view.Initialize();

            return view;
        }
    }
}