using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AirForce
{
    public class ProcedureMain : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.ShowEntitySuccess, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.ShowEntityFailure, OnShowEntityFailure);

            GameEntry.Entity.ShowMyAircraft(new MyAircraftData(GameEntry.Entity.GenerateSerialId(), 10000)
            {
                Name = "My Aircraft",
                Camp = CampType.Player,
            });
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.ShowEntitySuccess, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.ShowEntityFailure, OnShowEntityFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
        }

        private void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }
    }
}
