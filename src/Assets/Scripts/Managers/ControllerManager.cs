using BeamNG.RemoteControlLib;
using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

#if PLATFORM_ANDROID && !UNITY_EDITOR
using System.Linq;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace BeamNG.RemoteControlUltra.Managers
{
    public class ControllerManager : Singleton<ControllerManager>
    {
        protected override bool destroyPrevious => true;
        protected override bool dontDestroyOnLoad => false;

        private AndroidJavaObject? plugin;

        private int orientationhandler;

        private float angle;

        private List<float> rollingAverage = new List<float>();
        private const int MAX_SAMPLE_SIZE = 5;
        private float gravity;

        private float sensitivitySetting = 0.65f;

        private List<ButtonComponent> buttons = new();
        private List<AxisComponent> axes = new();

        [SerializeField] private RectTransform componentContainer = null!;

        [SerializeField] private Transform uiContainer = null!;
        [SerializeField] private GameObject throttlePressesIndicator = null!;
        [SerializeField] private GameObject brakesPressesIndicator = null!;

        void Start()
        {
#if PLATFORM_ANDROID && !UNITY_EDITOR
            plugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
            plugin.Call("setSamplingPeriod", 10 * 1000); // refresh sensor 10 mSec each
            plugin.Call("startSensorListening", "accelerometer");
            plugin.Call("startSensorListening", "rotationvector");

            Screen.orientation = ScreenOrientation.LandscapeLeft;
            orientationhandler = 1;
#endif

            if (Save.Ins.Settings.CustomLayout)
                loadLayout(Save.Ins.Settings.CurrentLayout);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && UIManager.Ins.Count == 0)
            {
                UIManager.Ins.OpenPopup("Confirm", "Disconnect and go to menu?", UIManager.PopupButtons.YesNo, callback: result =>
                {
                    if (result == UIManager.PopupResult.Yes)
                    {
                        AppManager.Ins.Disconnect();
                        SceneManager.LoadScene(0, LoadSceneMode.Single);
                    }
                });
                return;
            }

#if PLATFORM_ANDROID && !UNITY_EDITOR
            if (plugin != null)
            {
                float[]? sensorValue = plugin.Call<float[]>("getSensorValues", "rotationvector");
                if (sensorValue != null && sensorValue.Length >= 2)
                {
                    float val = sensorValue[1];

                    if (val < -0.25)
                    {
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                        orientationhandler = 1;
                    }
                    else if (val > 0.25)
                    {
                        Screen.orientation = ScreenOrientation.LandscapeRight;
                        orientationhandler = -1;
                    }
                }

                sensorValue = plugin.Call<float[]>("getSensorValues", "accelerometer");
                if (sensorValue != null && sensorValue.Length >= 3)
                {
                    angle = MathF.Asin(
                        -sensorValue[1] / MathF.Sqrt(
                            sensorValue[0] * sensorValue[0] +
                            sensorValue[1] * sensorValue[1] +
                            sensorValue[2] * sensorValue[2]
                        )
                    ) * 180f / MathF.PI;

                    rollingAverage = roll(rollingAverage, sensorValue[1]);
                    gravity = rollingAverage.Average(); // -10 to 10
                }
            }
#endif

            if (AppManager.Ins.GameInstance is not null)
            {
                GameInstance game = AppManager.Ins.GameInstance;

                if (Save.Ins.Settings.CustomLayout)
                {
                    var layout = Save.Ins.Settings.CurrentLayout;

                    if (layout.AccelometerAxis > -1)
                        game.Controls!.Axes[layout.AccelometerAxis] = MathF.Min(MathF.Max((angle * sensitivitySetting * orientationhandler) / 75, -0.5f), 0.5f) + 0.5f;

                    for (int i = 0; i < buttons.Count; i++)
                    {
                        var button = buttons[i];
                        game.Controls!.Buttons[button.Slot] = button.Value;
                    }

                    for (int i = 0; i < axes.Count; i++)
                    {
                        var axis = axes[i];
                        game.Controls!.Axes[axis.Slot] = axis.Invert ? (1f - axis.Value) : axis.Value;
                    }

                    throttlePressesIndicator.SetActive(false);
                    brakesPressesIndicator.SetActive(false);
                }
                else
                {
                    game.Controls!.Axes[0] = MathF.Min(MathF.Max((angle * sensitivitySetting * orientationhandler) / 75, -0.5f), 0.5f) + 0.5f;
                    game.Controls.Buttons[0] = 0f;
                    game.Controls.Buttons[1] = 0f;

                    float midddle = Screen.width / 2f;

                    foreach (var touch in Input.touches)
                    {
                        if (touch.phase == TouchPhase.Canceled) continue;

                        if (touch.position.x > midddle)
                            game.Controls.Buttons[1] = 1f;
                        else
                            game.Controls.Buttons[0] = 1f;
                    }

                    throttlePressesIndicator.SetActive(game.Controls.Buttons[1] >= 0.5f);
                    brakesPressesIndicator.SetActive(game.Controls.Buttons[0] >= 0.5f);
                }
            }

            float uiAngle = gravity * 7.9f * orientationhandler;

            uiContainer.transform.localEulerAngles = new Vector3(0f, 0f, uiAngle);
        }

        public void Reconnect()
            => AppManager.Ins.Reconnect();

        //rolling list of the last {MAX_SAMPLE_SIZE} Sensorevents
        private List<float> roll(List<float> list, float newMember)
        {
            if (list.Count >= MAX_SAMPLE_SIZE)
                list.RemoveAt(0);

            list.Add(newMember);
            return list;
        }

        private void loadLayout(ControlsLayout layout)
        {
            var children = componentContainer.GetChildren();

            for (int i = 0; i < children.Length; i++)
                Destroy(children[i].gameObject);

            buttons.Clear();
            axes.Clear();

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
        }

        void OnDestroy()
        {
#if UNITY_ANDROID
            if (plugin != null)
            {
                plugin.Call("terminate");
                plugin = null;
            }
#endif
        }
    }
}
