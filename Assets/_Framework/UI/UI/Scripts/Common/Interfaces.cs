
namespace Ironcow.UI
{
    public interface IUIList<T>
    {
        public T AddItem();
        public void SetList();
        public void ClearList();
    }

    public interface CanvasOption
    {
        public void SetParent();
    }
}