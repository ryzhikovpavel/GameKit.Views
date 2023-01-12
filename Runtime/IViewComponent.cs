using System.Threading.Tasks;
using GameKit.Views.Components;

namespace GameKit.Views
{
    public interface IViewComponent
    {
        public bool IsDisplayed { get; }
        public void Hide();
    }
    
    public interface IViewResult<T>: IViewComponent
    {
        public T Result { get; set; }

        public void CompleteWith(T result)
        {
            Result = result;
            Hide();
        }

        public async Task<T> WaitResult()
        {
            while (IsDisplayed)
            {
                await Task.Yield();
            }
            return Result;
        }
    }
}