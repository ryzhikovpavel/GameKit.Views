using GameKit.Views.Components;

namespace GameKit.Views
{
    public interface IViewComponent
    {
        public bool IsDisplayed { get; }
        public void Hide();
    }
}