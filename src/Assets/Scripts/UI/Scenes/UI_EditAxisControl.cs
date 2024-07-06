using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.UI;
using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using MathUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.Scenes
{
    public class UI_EditAxisControl : UIScene
    {
        protected override bool CloseWhenEscape => true;

        [SerializeField] private TMP_InputField input_name = null!;

        [SerializeField] private TMP_Dropdown dropdown_slot = null!;

        [SerializeField] private TextMeshProUGUI text_defaultValue = null!;
        [SerializeField] private Slider slider_defaultValue = null!;

        [SerializeField] private Toggle toggle_invert = null!;

        [SerializeField] private Toggle toggle_defaultValueSnap = null!;

        [SerializeField] private TextMeshProUGUI text_snapSpeed = null!;
        [SerializeField] private Slider slider_snapSpeed = null!;

        public override void Init()
        {
            if (Value is not AxisComponent axis) throw new InvalidDataException("Value must be AxisComponent");

            input_name.text = axis.Name;

            dropdown_slot.options = Enumerable.Range(0, ControlsLayout.MaxSlotsPerControlType).Select(slot => new TMP_Dropdown.OptionData("Slot " + slot)).ToList();
            dropdown_slot.value = axis.Slot;

            slider_defaultValue.onValueChanged.AddListener(newValue =>
            {
                text_defaultValue.text = Maths.Round(newValue, 2).ToString();
            });
            slider_defaultValue.value = axis.DefaultValue;

            toggle_invert.isOn = axis.Invert;

            toggle_defaultValueSnap.isOn = axis.SnapToDefaultWhenNotInUse;

            slider_snapSpeed.onValueChanged.AddListener(newValue =>
            {
                text_snapSpeed.text = Maths.Round(newValue, 3).ToString();
            });
            slider_snapSpeed.value = axis.SnapToDefaultSpeed;
        }

        public void Delete()
        {
            CustomLayoutManager.Ins.RemoveElement((AxisComponent)Value!);

            Close();
        }

        public override void Close()
        {
            if (Value is not AxisComponent axis) throw new InvalidDataException("Value must be AxisComponent");

            axis.Name = input_name.text;
            CustomLayoutManager.Ins.SwapSlots(LayoutObjectType.Axis, axis.Slot, dropdown_slot.value);
            axis.DefaultValue = slider_defaultValue.value;
            axis.Invert = toggle_invert.isOn;
            axis.SnapToDefaultWhenNotInUse = toggle_defaultValueSnap.isOn;
            axis.SnapToDefaultSpeed = slider_snapSpeed.value;

            Value = null;
            base.Close();
        }
    }
}
