/*
  作者：LTH
  文件描述：
  文件名：SurrogateContainer
  创建时间：2023/07/16 17:07:SS
*/

using System.Runtime.Serialization;

namespace Entt.Serialization.Xml
{
    /// <summary>
    ///   Surrogate providers are not allowed to use built-in objects like string
    ///   or int as surrogate value. Why? Only the authors of the DataContractSurrogateCaller
    ///   class would know. This little wrapper allows you to serialize primitive objects
    ///   without having to declare the same boilerplate code over and over again.
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    [DataContract(Namespace = "urn:SurrogateKey")]
    public readonly struct SurrogateContainer<TContent>
    {
        [DataMember(Order = 0, Name = "C")]
        public readonly TContent Content;

        public SurrogateContainer(TContent content)
        {
            Content = content;
        }

        public static implicit operator TContent(SurrogateContainer<TContent> p)
        {
            return p.Content;
        }
           
        public static implicit operator SurrogateContainer<TContent>(TContent p)
        {
            return new SurrogateContainer<TContent>(p);
        }
    }
}