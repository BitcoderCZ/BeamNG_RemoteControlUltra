using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using System;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    public abstract class AxisComponent : LayoutComponent
    {
        public override LayoutObjectType Type => LayoutObjectType.Axis;

        public float DefaultValue { get; set; } = 0.5f;
        public bool Invert { get; set; } = true;
        public bool SnapToDefaultWhenNotInUse { get; set; } = true;
        public float SnapToDefaultSpeed { get; set; } = 0.05f;

        public virtual void Load(int slot, ControlsLayout.Axis axis)
        {
            Name = axis.Name;
            Slot = slot;

            DefaultValue = axis.DefaultValue;
            Invert = axis.Invert;
            SnapToDefaultWhenNotInUse = axis.SnapToDefaultWhenNotInUse;
            SnapToDefaultSpeed = axis.SnapToDefaultSpeed;

            Value = DefaultValue;
            SetVisualFromValue();
        }

        public override void Edit()
        {
            StopTransform();

            UIManager.Ins.OpenUI("EditAxisControl", value: this);
        }

        public abstract void SetVisualFromValue();
    }
}
