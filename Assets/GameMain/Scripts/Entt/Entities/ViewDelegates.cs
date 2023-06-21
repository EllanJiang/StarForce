/*
* 文件名：ViewDelegates
* 文件描述：EntityView委托
* 作者：aronliang
* 创建时间：2023/06/21 17:38:31
* 修改记录：
*/

using System.Diagnostics.CodeAnalysis;

namespace Entt.Entities
{
    //从代码覆盖率结果中排除该类的代码
    [ExcludeFromCodeCoverage]
    public static partial class ViewDelegates
    {
        /// <summary>
        /// 定义委托：参数是EntityKey和实现Entity相关控制接口的类
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        public delegate void Apply<TEntityKey>(IEntityViewControl<TEntityKey> v, TEntityKey k) 
            where TEntityKey : IEntityKey;
        /// <summary>
        /// 定义委托：参数是EntityKey,实现Entity相关控制接口的类,以及上下文
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        public delegate void ApplyWithContext<TEntityKey, in TContext>(IEntityViewControl<TEntityKey> v, TContext context, TEntityKey k)
            where TEntityKey : IEntityKey;
    }
}