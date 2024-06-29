using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.CustomComponents
{
    [ExecuteAlways]
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DropdownEnabler : MonoBehaviour
    {
        [SerializeField] private List<int> interactableValues = null!;

        [SerializeField] private List<Selectable> onEnabled = null!;
        [SerializeField] private List<Selectable> onDisabled = null!;

        private TMP_Dropdown dropdown = null!;

        private void Start()
        {
            dropdown = GetComponent<TMP_Dropdown>();

            dropdown.onValueChanged.AddListener(updateState);

            updateState(dropdown.value);
        }

        private void updateState(int value)
        {
            bool interactable = interactableValues.Contains(value);

            for (int i = 0; i < onEnabled.Count; i++)
                onEnabled[i].interactable = interactable;

            for (int i = 0; i < onDisabled.Count; i++)
                onDisabled[i].interactable = !interactable;
        }
    }
}
