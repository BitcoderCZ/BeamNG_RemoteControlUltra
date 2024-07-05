using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.Utils;
using UnityEngine.EventSystems;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    public abstract class ButtonComponent : LayoutComponent
    {
        public override LayoutObjectType Type => LayoutObjectType.Button;

        private int holdInput = -1;

        public virtual void Load(int slot, ControlsLayout.Button btn)
        {
            Name = btn.Name;
            Slot = slot;
        }

        protected override void UpdateValue()
        {
        }

        public override void Edit()
        {
            StopTransform();

            UIManager.Ins.OpenUI("EditButtonControl", value: this);
        }

        protected override void OnInputUp(TouchP touch)
        {
            if (EditMode)
            {
                base.OnInputUp(touch);
                return;
            }

            if (touch.Id != holdInput) return;

            Value = 0f;
            holdInput = -1;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (CanBeEdited)
            {
                base.OnPointerDown(eventData);
                return;
            }

            if (holdInput != -1) return;

            eventData.Use();

            InputP.Update();
            TouchP touchP = InputP.GetClosestTouch(eventData.position)!.Value;
            holdInput = touchP.Id;

            Value = 1f;
        }
    }
}
