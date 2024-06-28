using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.CustomComponents
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleEnabler : MonoBehaviour
    {
        [SerializeField] private List<Selectable> onEnabled = null!;
        [SerializeField] private List<Selectable> onDisabled = null!;

        private Toggle toggle = null!;

        private void Start()
        {
            toggle = GetComponent<Toggle>();

            toggle.onValueChanged.AddListener(updateState);

            updateState(toggle.isOn);
        }

        private void updateState(bool value)
        {
            for (int i = 0; i < onEnabled.Count; i++)
                onEnabled[i].interactable = value;

            for (int i = 0; i < onDisabled.Count; i++)
                onDisabled[i].interactable = !value;
        }
    }
}
