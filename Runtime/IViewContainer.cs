using System.Threading.Tasks;
using GameKit.Views.Components;

namespace GameKit.Views
{
    public interface IViewContainer
    {
        public bool IsLoading { get; }
        
        public TView Create<TView>() where TView : ViewComponent;
        public void Load<TView>() where TView : ViewComponent;
        public Task LoadAsync<TView>() where TView : ViewComponent;
    }
}