using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    public abstract class AxisComponent : LayoutComponent
    {
        public override LayoutObjectType Type => LayoutObjectType.Axis;

        public virtual void Load(int slot, ControlsLayout.Axis axis)
        {
            Name = axis.Name;
            Slot = slot;
        }

        public override void Edit()
        {
            throw new NotImplementedException();

            StopTransform();

            UIManager.Ins.OpenUI("EditAxisControl", value: this);
        }
    }
}
