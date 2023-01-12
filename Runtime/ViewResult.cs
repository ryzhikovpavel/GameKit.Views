using System.Threading.Tasks;
using GameKit.Views.Components;

namespace GameKit.Views
{
    public class ViewResult<T>
    {
        private T _result;
        private bool _isCompleted;

        public void Complete(T result)
        {
            _result = result;
            _isCompleted = true;
        }

        public async Task<T> Wait(ViewComponent view)
        {
            while (_isCompleted == false && view.IsDisplayed)
            {
                await Task.Yield();
            }
            return _result;
        }
    }
}