using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class U
    {
        public enum WebcamPreference
        {
            Any,
            Front,
            Back
        }

        public static WebCamDevice? GetWebcam(WebcamPreference preference, bool forcePreference)
        {
            var webcams = WebCamTexture.devices;

            if (webcams.Length == 0) return null;

            if (preference == WebcamPreference.Any)
                return webcams[0];
            else
            {
                bool front = preference == WebcamPreference.Front;

                foreach (var webcam in webcams)
                    if (webcam.isFrontFacing == front)
                        return webcam;

                if (forcePreference) return null;
                else return webcams[0];
            }
        }
    }
}
