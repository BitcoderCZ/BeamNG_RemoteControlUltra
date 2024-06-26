using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BeamNG.RemoteControlUltra.Managers.UIManager;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.Scenes
{
    public class UI_Popup : UIScene
    {
        private bool closeWhenEscape;
        protected override bool CloseWhenEscape => closeWhenEscape;

        [SerializeField] private TextMeshProUGUI textMesh_Title = null!;
        [SerializeField] private TextMeshProUGUI textMesh_Message = null!;
        [SerializeField] private Transform buttonsContainer = null!;

        public override void Init()
        {
            if (Value is not PopupOptions options) throw new InvalidDataException("Value must be PopupOptions");

            textMesh_Title.text = options.Title;
            textMesh_Message.text = options.Message;

            List<(string name, PopupResult result)> buttons = options.Buttons switch
            {
                PopupButtons.None => new List<(string, PopupResult)>(),
                PopupButtons.Ok => new List<(string, PopupResult)>() { ("OK", PopupResult.Ok) },
                PopupButtons.OkCancel => new List<(string, PopupResult)>() { ("Cancel", PopupResult.Cancel), ("OK", PopupResult.Ok) },
                PopupButtons.YesNo => new List<(string, PopupResult)>() { ("No", PopupResult.No), ("Yes", PopupResult.Yes) },
                _ => throw new InvalidDataException($"Unsupported buttons: '{options.Buttons}'"),
            };

            if (buttons.Count == 0)
                Destroy(buttonsContainer.gameObject);
            else
            {
                foreach (var (name, result) in buttons)
                {
                    GameObject buttonObj = Instantiate(Prefabs.Button, buttonsContainer);
                    buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Value = result;
                        Close();
                    });
                    buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
                }
            }

            closeWhenEscape = options.CloseOnEscape;

            Value = null;
        }

        public override void Close()
        {
            if (Value is null) Value = PopupResult.Close;

            base.Close();
        }
    }
}
