using System.Threading.Tasks;
using GameKit.Views.Components;

namespace GameKit.Views
{
    public interface IViewLoader
    {
        public bool IsLoading { get; }
        public TView Load<TView>() where TView : ViewComponent;
        public Task<TView> LoadAsync<TView>() where TView : ViewComponent;
    }
}