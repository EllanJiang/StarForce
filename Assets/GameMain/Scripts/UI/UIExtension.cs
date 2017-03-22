using GameFramework;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;

namespace AirForce
{
    public static class UIExtension
    {
        public static bool HasUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroup = "Default")
        {
            return uiComponent.HasUIForm((int)uiFormId, uiGroup);
        }

        public static UIForm GetUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroup = "Default")
        {
            return uiComponent.GetUIForm((int)uiFormId, uiGroup);
        }

        public static UIForm[] GetUIForms(this UIComponent uiComponent, UIFormId uiFormId, string uiGroup = "Default")
        {
            return uiComponent.GetUIForms((int)uiFormId, uiGroup);
        }

        public static void OpenUIForm(this UIComponent uiComponent, UIFormId uiFormId, object userData = null)
        {
            uiComponent.OpenUIForm((int)uiFormId, userData);
        }

        public static void OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
        {
            IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
                return;
            }

            uiComponent.OpenUIForm(uiFormId, AssetUtility.GetUIFormAsset(drUIForm.AssetName), drUIForm.UIGroupName, drUIForm.PauseCoveredUIForm, userData);
        }

        public static void OpenDialog(this UIComponent uiComponent, UIDialogParams dialogParams)
        {
            if (((ProcedureBase)GameEntry.Procedure.CurrentProcedure).UseNativeDialog)
            {
                OpenNativeDialog(dialogParams);
            }
            else
            {
                uiComponent.OpenUIForm(UIFormId.Dialog, dialogParams);
            }
        }

        private static void OpenNativeDialog(UIDialogParams dialogParams)
        {
            throw new System.NotImplementedException("OpenNativeDialog");
        }

        private interface INativeCaller
        {
            void OpenDialog(int mode, string title, string message,
                string confirmText, int confirmCallbackId,
                string cancelText, int cancelCallbackId,
                string otherText, int otherCallbackId,
                string userData);
        }
    }
}
