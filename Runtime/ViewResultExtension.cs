using System.Threading;
using System.Threading.Tasks;

namespace GameKit.Views
{
    public static class ViewResultExtension
    {
        public static async Task<T> WaitResult<T>(this IViewResult<T> view)
        {
            while (view.IsDisplayed)
            {
                await Task.Yield();
            }
            return view.Result;
        }
        
        public static async Task<T> WaitResult<T>(this IViewResult<T> view, CancellationToken token)
        {
            while (view.IsDisplayed)
            {
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }
            token.ThrowIfCancellationRequested();
            return view.Result;
        }
    }
}