using System.IO;
using TMPro;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.Scenes
{
    public class UI_Text : UIScene
    {
        protected override bool CloseWhenEscape => false;

        [SerializeField] private TextMeshProUGUI textMesh = null!;

        public override void Init()
        {
            if (Value is not string text) throw new InvalidDataException($"{nameof(Value)} must be a string");

            textMesh.text = text;
        }
    }
}
