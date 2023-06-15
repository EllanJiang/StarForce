/*
* 文件名：TestAnimator
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/15 14:31:29
* 修改记录：
*/

using UnityEngine;

namespace Core
{
    public class TestAnimator:MonoBehaviour
    {
        public Animator Animator;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Animator.Play("Idle");
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                Animator.Play("Jump");
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Animator.Play("Run");
            }
            
        }
    }
}