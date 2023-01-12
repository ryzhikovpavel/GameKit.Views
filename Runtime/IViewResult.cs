namespace GameKit.Views
{
    public interface IViewResult<T>: IViewComponent
    {
        public T Result { get; set; }
    }
}