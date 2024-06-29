using BeamNG.RemoteControlUltra.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI
{
    public abstract class UIScene : MonoBehaviour
    {
        public virtual object? Value { get; set; }
        protected virtual bool WorksNotOnTop => false;
        protected abstract bool CloseWhenEscape { get; }
        public bool IsOnTop { get => UIManager.Ins.TopUI == this; }

        public virtual void Init() { }

        // max 1 param to show in inspector
        [Obsolete("Use OpenUI instead")] public virtual void SetUI(GameObject ui) => ExecIfOnTop(() => UIManager.Ins.SetUI(ui, true));
        [Obsolete("Use OpenUI instead")] public virtual void SetUI(string uiName) => ExecIfOnTop(() => UIManager.Ins.SetUI(uiName, true));
        public virtual void OpenUI(string uiName) => ExecIfOnTop(() => UIManager.Ins.OpenUI(uiName));

        protected virtual void Update()
        {
            // on android the back button triggers escape
            if (CloseWhenEscape && Input.GetKeyDown(KeyCode.Escape) && (WorksNotOnTop || IsOnTop))
                Close();
        }

        protected virtual void ExecIfOnTop(Action action)
        {
            if (WorksNotOnTop || IsOnTop)
                action();
        }

        public virtual void OnNewUIOpen() { }

        public virtual void Close()
        {
            UIManager.Ins.CloseUI(false);
            //AppManager.Ins.RunNextFrame(() => UIManager.Ins.CloseUI(false));
        }
    }
}
