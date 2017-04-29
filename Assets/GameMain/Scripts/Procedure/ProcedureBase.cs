using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace StarForce
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        public abstract bool UseNativeDialog
        {
            get;
        }

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        protected void OnError(string format, params object[] args)
        {
            OnError(false, format, args);
        }

        protected void OnErrorWithNetworkCheck(string format, params object[] args)
        {
            OnError(true, format, args);
        }

        protected void RestartGame(object userData)
        {
            UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Restart);
        }

        protected void QuitGame(object userData)
        {
            UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit);
        }

        protected void GotoUpdateGame(object userData)
        {
            VersionInfo versionInfo = (VersionInfo)userData;
            if (versionInfo == null)
            {
                return;
            }

            Application.OpenURL(versionInfo.GameUpdateUrl);
        }

        private void OnError(bool checkNetwork, string format, params object[] args)
        {
            if (checkNetwork && Application.internetReachability == NetworkReachability.NotReachable)
            {
                GameEntry.UI.OpenDialog(new DialogParams
                {
                    Mode = 2,
                    Message = GameEntry.Localization.GetString("UI_TEXT_NETWORK_NOT_REACHABLE"),
                    ConfirmText = GameEntry.Localization.GetString("UI_TEXT_RETRY"),
                    CancelText = GameEntry.Localization.GetString("UI_TEXT_QUIT_GAME"),
                    OnClickConfirm = RestartGame,
                    OnClickCancel = QuitGame,
                });

                return;
            }

            Log.Error(string.Format(format, args));
        }
    }
}
