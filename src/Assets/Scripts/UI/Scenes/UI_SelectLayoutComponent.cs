using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.Scenes
{
    public class UI_SelectLayoutComponent : UIScene
    {
        protected override bool CloseWhenEscape => true;

        [SerializeField] private Transform buttonsContainer = null!;
        [SerializeField] private Transform axesContainer = null!;

        public override void Init()
        {
            foreach (var prefab in Prefabs.ButtonControls
                .OrderBy(prefab => prefab.GetComponent<LayoutComponent>().TypeName)
            )
                init(Instantiate(Prefabs.Button, buttonsContainer), prefab);

            foreach (var prefab in Prefabs.AxisControls
                .OrderBy(prefab => prefab.GetComponent<LayoutComponent>().TypeName)
            )
                init(Instantiate(Prefabs.Button, axesContainer), prefab);

            void init(GameObject go, GameObject prefab)
            {
                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Value = prefab;
                    Close();
                });

                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prefab.GetComponent<LayoutComponent>().TypeName;
            }
        }
    }
}
