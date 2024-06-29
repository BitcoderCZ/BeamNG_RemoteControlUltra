using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using System;

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
