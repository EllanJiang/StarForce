using GameFramework;
using UnityEngine;

namespace AirForce
{
    public class HideByBoundary : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            GameObject gameObject = other.gameObject;
            Entity entity = gameObject.GetComponent<Entity>();
            if (entity == null)
            {
                Log.Warning("Unknown GameObject '{0}', you must use entity only.", gameObject.name);
                Destroy(gameObject);
                return;
            }

            GameEntry.Entity.HideEntity(entity.Entity);
        }
    }
}
