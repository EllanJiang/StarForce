using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace StarForce
{
    public class ProcedureVerifyResources : ProcedureBase
    {
        private bool m_VerifyResourcesComplete = false;

        public override bool UseNativeDialog
        {
            get
            {
                return true;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(ResourceVerifyStartEventArgs.EventId, OnResourceVerifyStart);
            GameEntry.Event.Subscribe(ResourceVerifySuccessEventArgs.EventId, OnResourceVerifySuccess);
            GameEntry.Event.Subscribe(ResourceVerifyFailureEventArgs.EventId, OnResourceVerifyFailure);

            m_VerifyResourcesComplete = false;
            GameEntry.Resource.VerifyResources(OnVerifyResourcesComplete);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(ResourceVerifyStartEventArgs.EventId, OnResourceVerifyStart);
            GameEntry.Event.Unsubscribe(ResourceVerifySuccessEventArgs.EventId, OnResourceVerifySuccess);
            GameEntry.Event.Unsubscribe(ResourceVerifyFailureEventArgs.EventId, OnResourceVerifyFailure);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_VerifyResourcesComplete)
            {
                return;
            }

            ChangeState<ProcedureCheckResources>(procedureOwner);
        }

        private void OnVerifyResourcesComplete(bool result)
        {
            m_VerifyResourcesComplete = true;
            Log.Info("Verify resources complete, result is '{0}'.", result);
        }

        private void OnResourceVerifyStart(object sender, GameEventArgs e)
        {
            ResourceVerifyStartEventArgs ne = (ResourceVerifyStartEventArgs)e;
            Log.Info("Start verify resources, verify resource count '{0}', verify resource total length '{1}'.", ne.Count, ne.TotalLength);
        }

        private void OnResourceVerifySuccess(object sender, GameEventArgs e)
        {
            ResourceVerifySuccessEventArgs ne = (ResourceVerifySuccessEventArgs)e;
            Log.Info("Verify resource '{0}' success.", ne.Name);
        }

        private void OnResourceVerifyFailure(object sender, GameEventArgs e)
        {
            ResourceVerifyFailureEventArgs ne = (ResourceVerifyFailureEventArgs)e;
            Log.Warning("Verify resource '{0}' failure.", ne.Name);
        }
    }
}
