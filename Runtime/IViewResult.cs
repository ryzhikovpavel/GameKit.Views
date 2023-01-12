namespace GameKit.Views
{
    public interface IViewResult<T>: IViewComponent
    {
        public T Result { get; set; }

        public void CompleteWith(T result)
        {
            Result = result;
            Hide();
        }
    }
}