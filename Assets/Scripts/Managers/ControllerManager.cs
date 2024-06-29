
using BeamNG.RemoteControlLib;
using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        }

        private void Update()
        {
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
