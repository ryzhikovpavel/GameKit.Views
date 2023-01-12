using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameKit.Implementation;
using GameKit.Views.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Views.Core
{
    public class ViewContainer: IViewContainer
    {
        private readonly Dictionary<Type, IPool<ViewComponent>> _views = new();
        private readonly IViewLoader _loader;
        
        public bool IsLoading => _loader.IsLoading;

        public ViewContainer(IViewLoader loader)
        {
            _loader = loader;
        }

        public TView Create<TView>() where TView : ViewComponent
        {
            if (_views.TryGetValue(typeof(TView), out var pool)) return pool.Pull() as TView;
            _views[typeof(TView)] = pool = CreatePool(_loader.Load<TView>());
            return pool.Pull() as TView;
        }

        public void Load<TView>() where TView : ViewComponent
        {
            LoadAsync<TView>().ContinueWith(CheckError);
        }

        public async Task LoadAsync<TView>() where TView: ViewComponent
        {
            if (_views.ContainsKey(typeof(TView))) return;
            var prefab = await _loader.LoadAsync<TView>();
            _views[typeof(TView)] = CreatePool(prefab);
        }

        private void ResetView(ViewComponent view)
        {
            view.OnRelease();
        }

        private IPool<ViewComponent> CreatePool(ViewComponent prefab)
        {
            return Pool.Build(() => InstantiateView(prefab.gameObject), ResetView);
        }

        private ViewComponent InstantiateView(GameObject prefab)
        {
            if (prefab == null)
                throw new Exception($"Prefab not found");

            prefab.SetActive(false);
            GameObject obj = Object.Instantiate(prefab);
            
#if UNITY_EDITOR
            prefab.SetActive(true);
#endif
            ViewComponent view = obj.GetComponent<ViewComponent>();
            view.name = view.GetType().Name;
            view.Initialize();

            return view;
        }

        private void CheckError(Task task)
        {
            if (task.IsFaulted)
                Debug.LogError(task.Exception);
        }
    }
}