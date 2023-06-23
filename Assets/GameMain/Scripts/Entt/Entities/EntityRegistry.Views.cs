/*
  作者：LTH
  文件描述：
  文件名：EntityRegistry_Views
  创建时间：2023/06/23 23:06:SS
*/

namespace Entt.Entities
{
    public partial class EntityRegistry<TEntityKey>
    {
        public void DiscardView<TView>() where TView : IEntityView<TEntityKey>
        {
            views.Remove(typeof(TView));
        }
    }
}