using GameFramework.ObjectPool;
using UnityEngine;

namespace StarForce
{
    public class HPBarItemObject : ObjectBase
    {
        public HPBarItemObject(object target)
            : base(target)
        {

        }

        protected override void Release(bool isShutdown)
        {
            HPBarItem hpBarItem = (HPBarItem)Target;
            if (hpBarItem == null)
            {
                return;
            }

            Object.Destroy(hpBarItem.gameObject);
        }
    }
}
