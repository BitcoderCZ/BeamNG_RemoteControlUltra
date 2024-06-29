using BeamNG.RemoteControlUltra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown_speedUnit = null!;
        [SerializeField] private TMP_Dropdown dropdown_controlsType = null!;

        private void Start()
        {
            // init
            var speedUnityValues = Enum.GetValues(typeof(SpeedUnit));
            dropdown_speedUnit.options.Clear();
            foreach (var speedUnit in speedUnityValues)
                dropdown_speedUnit.options.Add(new TMP_Dropdown.OptionData(SpeedUnitE.GetDisplayText((SpeedUnit)speedUnit)));

            dropdown_speedUnit.value = Array.IndexOf(speedUnityValues, Save.Ins.Settings.SpeedUnit);

            dropdown_controlsType.value = Save.Ins.Settings.CustomLayout ? 1 : 0;

            // listeners
            dropdown_speedUnit.onValueChanged.AddListener(value =>
            {
                Save.Ins.Settings.SpeedUnit = (SpeedUnit)speedUnityValues.GetValue(value);
                Save.Write(); // save
            });
            dropdown_controlsType.onValueChanged.AddListener(value =>
            {
                Save.Ins.Settings.CustomLayout = value == 1;
                Save.Write(); // save
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }
    }
}
