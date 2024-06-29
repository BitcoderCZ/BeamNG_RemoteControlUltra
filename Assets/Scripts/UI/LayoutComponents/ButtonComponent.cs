using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    public abstract class ButtonComponent : LayoutComponent
    {
        public override LayoutObjectType Type => LayoutObjectType.Button;

        public virtual void Load(int slot, ControlsLayout.Button btn)
        {
            Name = btn.Name;
            Slot = slot;
        }

        public override void Edit()
        {
            StopTransform();

            UIManager.Ins.OpenUI("EditButtonControl", value: this);
        }
    }
}
