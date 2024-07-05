using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.Scenes
{
    public class UI_LayoutMenu : UIScene
    {
        protected override bool CloseWhenEscape => true;

        [SerializeField] private TMP_Dropdown dropdown_accelometerAxis = null!;

        public override void Init()
        {
            dropdown_accelometerAxis.options =
                Enumerable.Concat(
                    new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("None") },
                    Enumerable.Range(0, ControlsLayout.MaxSlotsPerControlType).Select(slot => new TMP_Dropdown.OptionData("Slot " + slot))
                ).ToList();

            dropdown_accelometerAxis.value = CustomLayoutManager.Ins.AccelometerAxisSlot + 1;
            dropdown_accelometerAxis.onValueChanged.AddListener(newValue =>
            {
                CustomLayoutManager.Ins.AccelometerAxisSlot = newValue - 1;
            });
        }

        public void Exit()
        {
            CustomLayoutManager.Ins.AccelometerAxisSlot = dropdown_accelometerAxis.value - 1;

            CustomLayoutManager.Ins.Write();
            SceneManager.LoadScene("Settings", LoadSceneMode.Single);
        }

        public override void Close()
        {
            CustomLayoutManager.Ins.AccelometerAxisSlot = dropdown_accelometerAxis.value - 1;

            base.Close();
        }
    }
}
