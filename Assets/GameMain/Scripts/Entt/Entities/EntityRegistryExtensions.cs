/*
* 文件名：EntityRegistryExtensions
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/28 14:37:22
* 修改记录：
*/

using System.Diagnostics.CodeAnalysis;

namespace Entt.Entities
{
    /// <summary>
    /// Entity管理器拓展
    /// </summary>
    public static partial class EntityRegistryExtensions
    {
        /// <summary>
        /// 如果该Entity身上没有挂载该Component，那么把该Component附加到Entity身上并返回该Component
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="entity"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static TComponent GetOrCreateComponent<TEntityKey, TComponent>(this IEntityViewControl<TEntityKey> registry, TEntityKey entity)
            where TEntityKey : IEntityKey
        {
            if (!registry.GetComponent<TComponent>(entity, out var c))
            {
                c = registry.AssignComponent<TComponent>(entity);
            }

            return c;
        }
        
        /// <summary>
        /// 如果该Entity身上没有挂载该Component，那么把该Component附加到Entity身上并返回该Component
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TComponent"></typeparam>
        public static void GetOrCreateComponent<TEntityKey, TComponent>(this IEntityViewControl<TEntityKey> registry, TEntityKey entity, [MaybeNullWhen(false)] out TComponent c) 
            where TEntityKey : IEntityKey
        {
            if (!registry.GetComponent(entity, out c))
            {
                c = registry.AssignComponent<TComponent>(entity);
            }
        }
    }
}