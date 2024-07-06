using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.UI.CustomComponents;
using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    public class VerticalSliderAxisComponent : AxisComponent
    {
        public override string TypeName => "Vertical Slider";
        public override Vector2 MinSize => new Vector2(120, 240);

        private int handleInput = -1;
        private float handleGrabOffset;

        [SerializeField] private RectTransform handleContainer = null!;
        [SerializeField] private RectTransform handle = null!;
        [SerializeField] private ButtonPlus btn_handle = null!;

        protected override void Init()
        {
            handle.GetComponent<Image>().raycastTarget = !EditMode;

            btn_handle.OnDown += eventData =>
            {
                if (CanBeEdited) return;

                eventData.Use();

                InputP.Update();
                TouchP touchP = InputP.GetClosestTouch(eventData.position)!.Value;
                handleInput = touchP.Id;
                handleGrabOffset = handle.position.y - touchP.Touch.position.y;
            };
        }

        protected override void Update()
        {
            if (EditMode)
            {
                Value = DefaultValue;
                SetVisualFromValue();
            }

            base.Update();
        }

        protected override void CheckSize(ref Vector2 size)
        {
            size.x = MathF.Min(size.x, size.y / 2f);
            base.CheckSize(ref size);
        }

        protected override void UpdateValue()
        {
            if (handleInput == -1)
            {
                if (SnapToDefaultWhenNotInUse && Value != DefaultValue)
                {
                    if (MathF.Abs(Value - DefaultValue) < SnapToDefaultSpeed)
                        Value = DefaultValue;
                    else if (Value < DefaultValue)
                        Value += SnapToDefaultSpeed;
                    else
                        Value -= SnapToDefaultSpeed;

                    SetVisualFromValue();
                }
            }
            else
            {
                var touchP = InputP.GetTouchById(handleInput);

                float localTouchPos = touchP.Touch.position.y - handleContainer.position.y;

                float max = handleContainer.rect.height - handle.rect.height;
                float pos = Math.Clamp(localTouchPos + handleGrabOffset, 0f, max);

                handle.anchoredPosition = new Vector2(0f, pos);

                Value = pos / max;
            }
        }

        protected override void OnInputUp(TouchP touch)
        {
            if (EditMode)
            {
                base.OnInputUp(touch);
                return;
            }

            if (touch.Id != handleInput) return;

            handleInput = -1;
        }

        public override void SetVisualFromValue()
        {
            float max = handleContainer.rect.height - handle.rect.width; // handle.rect.height should be equal to handle.rect.width, but that might take a frame to update, so we use width here

            handle.anchoredPosition = new Vector2(0f, Value * max);
        }
    }
}
