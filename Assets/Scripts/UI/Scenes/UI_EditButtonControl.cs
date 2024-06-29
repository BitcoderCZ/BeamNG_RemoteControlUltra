using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.UI;
using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.Scenes
{
    public class UI_EditButtonControl : UIScene
    {
        protected override bool CloseWhenEscape => true;

        [SerializeField] private TMP_InputField input_name = null!;
        [SerializeField] private TMP_Dropdown dropdown_slot = null!;

        public override void Init()
        {
            if (Value is not ButtonComponent btn) throw new InvalidDataException("Value must be ButtonComponent");

            input_name.text = btn.Name;

            dropdown_slot.options = Enumerable.Range(0, ControlsLayout.MaxSlotsPerControlType).Select(slot => new TMP_Dropdown.OptionData("Slot " + slot)).ToList();
            dropdown_slot.value = btn.Slot;
        }

        public override void Close()
        {
            if (Value is not ButtonComponent btn) throw new InvalidDataException("Value must be ButtonComponent");

            btn.Name = input_name.text;
            CustomLayoutManager.Ins.SwapSlots(LayoutObjectType.Button, btn.Slot, dropdown_slot.value);

            Value = null;
            base.Close();
        }
    }
}
