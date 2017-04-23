using UnityEngine;
using UnityEngine.UI;

namespace StarForce
{
    public class MenuForm : UGuiForm
    {
        [SerializeField]
        private Text m_Title = null;

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_Title.text = GameEntry.Localization.GetString("Game.Name");
        }
    }
}
