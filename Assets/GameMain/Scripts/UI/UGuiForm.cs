using GameFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public abstract class UGuiForm : UIFormLogic
    {
        public const int DepthFactor = 100;

        private Canvas m_CachedCanvas = null;

        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return m_CachedCanvas.sortingOrder;
            }
        }

        protected void Close()
        {
            GameEntry.UI.CloseUIForm(this);
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            OriginalDepth = m_CachedCanvas.sortingOrder;

            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
        }

        protected internal override void OnClose(object userData)
        {
            base.OnClose(userData);
        }

        protected internal override void OnPause()
        {
            base.OnPause();
        }

        protected internal override void OnResume()
        {
            base.OnResume();
        }

        protected internal override void OnCover()
        {
            base.OnCover();
        }

        protected internal override void OnReveal()
        {
            base.OnReveal();
        }

        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].sortingOrder += deltaDepth;
            }
        }
    }
}
