using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AirForce
{
    public abstract class GameBase
    {
        public abstract GameMode GameMode
        {
            get;
        }

        public virtual void Initialize()
        {
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.ShowEntitySuccess, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.ShowEntityFailure, OnShowEntityFailure);

            GameEntry.Entity.ShowMyAircraft(new MyAircraftData(GameEntry.Entity.GenerateSerialId(), 10000)
            {
                Name = "My Aircraft",
                Camp = CampType.Player,
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
            });
        }

        public virtual void Shutdown()
        {
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.ShowEntitySuccess, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.ShowEntityFailure, OnShowEntityFailure);
        }

        public virtual void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {

        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }
    }
}
