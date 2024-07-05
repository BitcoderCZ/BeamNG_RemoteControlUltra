using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace BeamNG.RemoteControlUltra.Managers
{
    public class CustomLayoutManager : Singleton<CustomLayoutManager>
    {
        protected override bool destroyPrevious => false;
        protected override bool dontDestroyOnLoad => false;

        private AndroidJavaObject? plugin;

        private int accelometerAxisSlot = 0;
        public int AccelometerAxisSlot
        {
            get => accelometerAxisSlot;
            set
            {
                if (value < -1 || value >= ControlsLayout.MaxSlotsPerControlType)
                    throw new IndexOutOfRangeException(nameof(value));

                accelometerAxisSlot = value;
            }
        }

        private List<ButtonComponent> buttons = new();
        private List<AxisComponent> axes = new();

        [SerializeField] private RectTransform componentContainer = null!;

        void Start()
        {
#if PLATFORM_ANDROID && !UNITY_EDITOR
            plugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
            plugin.Call("setSamplingPeriod", 10 * 1000); // refresh sensor 10 mSec each
            plugin.Call("startSensorListening", "rotationvector");

            Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif

            Read();
        }

        private void Update()
        {
#if PLATFORM_ANDROID&& !UNITY_EDITOR
            if (plugin != null)
            {
                float[]? sensorValue = plugin.Call<float[]>("getSensorValues", "rotationvector");
                if (sensorValue != null && sensorValue.Length >= 2)
                {
                    float val = sensorValue[1];

                    if (val < -0.25)
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                    else if (val > 0.25)
                        Screen.orientation = ScreenOrientation.LandscapeRight;
                }
            }
#endif

            if (Input.GetKeyDown(KeyCode.Escape) && UIManager.Ins.Count == 0)
            {
                Write();
                SceneManager.LoadScene("Settings", LoadSceneMode.Single);
            }
        }

        public void Write()
        {
            Save.Ins.Settings.CurrentLayout = createLayout();
            Save.Write();
        }

        public void Read()
            => loadLayout(Save.Ins.Settings.CurrentLayout);

        public void AddElement()
        {
            UIManager.Ins.OpenUI("SelectLayoutComponent", res =>
            {
                if (res is not GameObject prefab) return;

                LayoutObjectType type = prefab.GetComponent<LayoutComponent>().Type;

                if (count(type) >= ControlsLayout.MaxCount(type))
                {
                    UIManager.Ins.OpenPopup("Error", $"Maximum amount of {(type == LayoutObjectType.Button ? "buttons" : "axes")} has already been placed", UIManager.PopupButtons.Ok);
                    return;
                }

                LayoutComponent component = Instantiate(prefab, (componentContainer.rect.size / 2f).XYN(), Quaternion.identity, componentContainer).GetComponent<LayoutComponent>();

                component.Slot = nextFreeSlot(component.Type);
                Debug.Log($"Used slot: {component.Slot}");

                if (component is ButtonComponent button)
                    buttons.Add(button);
                else if (component is AxisComponent axis)
                    axes.Add(axis);
                else
                    throw new InvalidDataException($"Invalid axis: '{component.GetType().Name}'");
            });
        }

        public void OpenMenu()
            => UIManager.Ins.OpenUI("LayoutMenu");

        public void SwapSlots(LayoutObjectType type, int a, int b)
        {
            if (type == LayoutObjectType.Button)
            {
                var aList = buttons.Where(btn => btn.Slot == a).ToList();
                var bList = buttons.Where(btn => btn.Slot == b).ToList();

                foreach (var item in aList)
                    item.Slot = b;
                foreach (var item in bList)
                    item.Slot = a;
            }
            else
            {
                var aList = axes.Where(btn => btn.Slot == a).ToList();
                var bList = axes.Where(btn => btn.Slot == b).ToList();

                foreach (var item in aList)
                    item.Slot = b;
                foreach (var item in bList)
                    item.Slot = a;

                if (accelometerAxisSlot == a)
                    accelometerAxisSlot = b;
                else if (accelometerAxisSlot == b)
                    accelometerAxisSlot = a;
            }
        }

        private int count(LayoutObjectType type)
        {
            if (type == LayoutObjectType.Button) return buttons.Count;
            else return axes.Count;
        }

        private int nextFreeSlot(LayoutObjectType type)
        {
            if (type == LayoutObjectType.Button)
            {
                for (int slot = 0; slot < ControlsLayout.MaxSlotsPerControlType; slot++)
                    if (buttons.Where(btn => btn.Slot == slot).Count() == 0)
                        return slot;
            }
            else
            {
                for (int slot = 1; slot < ControlsLayout.MaxSlotsPerControlType; slot++)
                    if (axes.Where(btn => btn.Slot == slot).Count() == 0)
                        return slot;
            }

            throw new Exception("No free slot");
        }

        private ControlsLayout createLayout()
        {
            // transform child index to order
            Dictionary<int, int> tciToO = new();
            var children = componentContainer.GetChildren();
            Array.Sort(children, (a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));

            int btnC = 0;
            int axisC = 0;
            for (int i = 0; i < children.Length; i++)
            {
                LayoutComponent component = children[i].GetComponent<LayoutComponent>();

                component.StopTransform(); // position would be wrong if resizing

                if (component.Type == LayoutObjectType.Button)
                    tciToO.Add(i, btnC++);
                else
                    tciToO.Add(i, axisC++);
            }

            return new ControlsLayout()
            {
                AccelometerAxis = accelometerAxisSlot,
                Buttons = buttons.ToDictionary(btn => btn.Slot, btn => new ControlsLayout.Button(btn.TypeName, btn.Name, tciToO[Array.IndexOf(children, btn.transform)], btn.transform.position, ((RectTransform)btn.transform).sizeDelta)),
                Axes = axes.ToDictionary(axis => axis.Slot, axis => new ControlsLayout.Axis(axis.TypeName, axis.Name, tciToO[Array.IndexOf(children, axis.transform)], axis.transform.position, ((RectTransform)axis.transform).sizeDelta)),
            };
        }

        private void loadLayout(ControlsLayout layout)
        {
            var children = componentContainer.GetChildren();

            for (int i = 0; i < children.Length; i++)
                Destroy(children[i].gameObject);

            buttons.Clear();
            axes.Clear();

            accelometerAxisSlot = layout.AccelometerAxis;

            foreach (var item in layout.Buttons.OrderBy(item => item.Value.Order))
            {
                ButtonComponent button = Instantiate(Prefabs.ButtonControls.FirstOrDefault(prefab => prefab.name == item.Value.TypeName), item.Value.Pos.XYN(), Quaternion.identity, componentContainer).GetComponent<ButtonComponent>();

                ((RectTransform)button.transform).sizeDelta = item.Value.Size;

                button.Load(item.Key, item.Value);

                buttons.Add(button);
            }

            foreach (var item in layout.Axes.OrderBy(item => item.Value.Order))
            {
                AxisComponent axis = Instantiate(Prefabs.AxisControls.FirstOrDefault(prefab => prefab.name == item.Value.TypeName), item.Value.Pos.XYN(), Quaternion.identity, componentContainer).GetComponent<AxisComponent>();

                ((RectTransform)axis.transform).sizeDelta = item.Value.Size;

                axis.Load(item.Key, item.Value);

                axes.Add(axis);
            }

            Debug.LogWarning("Loading axes not implemented");
        }
    }
}
