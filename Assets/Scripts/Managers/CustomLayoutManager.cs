using BeamNG.RemoteControlLib;
using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Managers
{
    public class CustomLayoutManager : Singleton<CustomLayoutManager>
    {
        protected override bool destroyPrevious => false;
        protected override bool dontDestroyOnLoad => false;

        private AndroidJavaObject? plugin;

        void Start()
        {
#if PLATFORM_ANDROID && !UNITY_EDITOR
            plugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
            plugin.Call("setSamplingPeriod", 10 * 1000); // refresh sensor 10 mSec each
            plugin.Call("startSensorListening", "rotationvector");

            Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
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
        }
    }
}
