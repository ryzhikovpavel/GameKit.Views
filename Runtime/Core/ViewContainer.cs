using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameKit.Views.Components;
using UnityEngine;

namespace GameKit.Views.Core
{
    public class ViewContainer: IViewContainer
    {
        private readonly Dictionary<Type, ViewPool> _views = new();
        private readonly IViewLoader _loader;
        
        public bool IsLoading => _loader.IsLoading;

        public ViewContainer(IViewLoader loader)
        {
            _loader = loader;
        }

        public TView Create<TView>() where TView : ViewComponent
        {
            if (Application.isPlaying == false) throw new Exception("Application is not running");
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
            if (_views.ContainsKey(typeof(TView))) return;
            _views[typeof(TView)] = CreatePool(prefab);
        }

        private ViewPool CreatePool(ViewComponent prefab)
        {
            return new ViewPool(prefab);
        }

        private void CheckError(Task task)
        {
            if (task.IsFaulted)
                Debug.LogError(task.Exception);
        }
    }
}