using System.IO;
using System.Threading.Tasks;
using GameKit.Views.Components;
using UnityEngine;

namespace GameKit.Views.Core
{
    public class ViewLoaderFromResources : IViewLoader
    {
        private readonly string _folder;
        private int _loadProcessCount;

        public bool IsLoading => _loadProcessCount > 0;

        public ViewLoaderFromResources(string viewPrefabResourceFolder)
        {
            _folder = viewPrefabResourceFolder;
        }

        public TView Load<TView>() where TView : ViewComponent
        {
            var viewType = typeof(TView);
            string path =  Path.Combine(_folder, viewType.Name);
            return Resources.Load<TView>(path);
        }

        public async Task<TView> LoadAsync<TView>() where TView : ViewComponent
        {
            _loadProcessCount++;
            var viewType = typeof(TView);
            string path =  Path.Combine(_folder, viewType.Name);
            var operation = Resources.LoadAsync<TView>(path);
            while (operation.isDone == false) await Task.Yield();
            _loadProcessCount--;
            return operation.asset as TView;
        }

        public void Unload<TView>(TView view) where TView : ViewComponent
        {
            Resources.UnloadAsset(view.gameObject);
        }
    }
}