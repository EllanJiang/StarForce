/*
* 文件名：EntitySystemReferenceTest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/03 14:26:18
* 修改记录：
*/

using System;
using Entt.Entities.Systems;
using FluentAssertions;
using UnityEngine;

namespace Entt.Test
{
    public class EntitySystemReferenceTest:MonoBehaviour
    {
        private void Start()
        {
            WeirdTestClosure();
        }

        private void TestStandard()
        {
            Action action = TestStandard;
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#TestStandard");
        }

        private void TestClosure()
        {
            int a = 10;

            //闭包
            void Closure()
            {
                a += 1;
            }

            Action action = Closure;
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#TestClosure.Closure");
        }

        private void WeirdTestClosure()
        {
            int a = 10;
            Action Closure()
            {
                void Closure2()
                {
                    a += 1;
                }
                return Closure2;
            }

            Action action = Closure();
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#WeirdTestClosure.Closure2");
        }
    }
}