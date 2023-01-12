using System.Threading.Tasks;
using GameKit.Views.Components;

namespace GameKit.Views
{
    public class ViewResult<T>
    {
        private readonly ViewComponent _view;
        private T _result;
        private bool _isCompleted;


        public void Complete(T result)
        {
            _result = result;
            _isCompleted = true;
        }

        public async Task<T> Wait()
        {
            _isCompleted = false;
            while (_isCompleted == false && _view.IsDisplayed)
            {
                await Task.Yield();
            }
            return _result;
        }

        public ViewResult(ViewComponent view)
        {
            _view = view;
        }
    }
}