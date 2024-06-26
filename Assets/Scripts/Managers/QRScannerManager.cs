using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using ZXing;

#nullable enable
namespace BeamNG.RemoteControlUltra.Managers
{
    public class QRScannerManager : Singleton<QRScannerManager>
    {
        protected override bool destroyPrevious => true;
        protected override bool dontDestroyOnLoad => false;

        [SerializeField] private RawImage camImage = null!;
        [SerializeField] private RectTransform camContainer = null!;

        private bool initialized = false;

        private WebCamTexture camTexture = null!;
        private RectTransform camRect = null!;

        private IBarcodeReader barCodeReader = new BarcodeReader()
        {
            AutoRotate = true,
        };
        private Texture2D? snap;
        private DateTime nextUpdateTime;
        private TimeSpan updateTimeOut = TimeSpan.FromSeconds(0.5);

        private string? data;

        private void Start()
        {
#if UNITY_ANDROID
            if (Application.isEditor)
                init();
            else
                requestPermission();
#else
            init();
#endif
           
        }
        private void requestPermission()
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += permissionName => init();
            callbacks.PermissionDenied += permissionDenied;
            callbacks.PermissionRequestDismissed += permissionDenied;
            Permission.RequestUserPermission(Permission.Camera, callbacks);

            void permissionDenied(string permissionName)
            {
                Debug.LogWarning("Permission denied");
                if (Permission.ShouldShowRequestPermissionRationale(Permission.Camera))
                {
                    UIManager.Ins.OpenPopup("Warning", "Camera permission is required to scan the QR code", UIManager.PopupButtons.Ok, callback: result =>
                    {
                        requestPermission();
                    });
                }
                else
                {
                    UIManager.Ins.OpenPopup("Error", "Couldn't get camera permission", UIManager.PopupButtons.Ok, callback: result =>
                    {
                        Application.Quit();
                    });
                }
            }
        }

        private void init()
        {
            WebCamDevice? webCam = U.GetWebcam(U.WebcamPreference.Back, false);
            if (webCam is null)
            {
                UIManager.Ins.OpenPopup("Error", "Couldn't find a camera", UIManager.PopupButtons.Ok, callback: result =>
                {
                    Application.Quit();
                });
                return;
            }

            camTexture = new WebCamTexture(webCam.Value.name);
            camTexture.Play();
            //if (webCam.Value.isAutoFocusPointSupported) camTexture.autoFocusPoint = new Vector2(0.5f, 0.5f);

            camImage.texture = camTexture;
            camRect = (RectTransform)camImage.transform;

            initialized = true;
        }

        private void Update()
        {
            if (
                !initialized || 
                UIManager.Ins.Count != 0 ||
                camTexture.width == 16 || camTexture.height == 16
            )
                return;

            // make sure it isn't bigger that the screen
            float camAspect = Math.Max((float)camTexture.width / (float)camTexture.height, (float)Screen.width / (float)Screen.height);
            if (camTexture.videoRotationAngle == 90 || camTexture.videoRotationAngle == 270)
            {
                camRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, camContainer.rect.width * camAspect);
                camRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, camContainer.rect.width);
            }
            else
            {
                camRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, camContainer.rect.width);
                camRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, camContainer.rect.width / camAspect);
            }

            camImage.transform.localEulerAngles = new Vector3(0f, 0f, -camTexture.videoRotationAngle);
            camImage.transform.localScale = new Vector3(camTexture.videoVerticallyMirrored ? -1f : 1f, 1f, 1f);

            if (snap is null)
                snap = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, false);

            if (data is not null || nextUpdateTime > DateTime.UtcNow) return;
            nextUpdateTime = DateTime.UtcNow.Add(updateTimeOut);

            snap.SetPixels32(camTexture.GetPixels32());

            try
            {
                var res = barCodeReader.Decode(snap.GetRawTextureData(), snap.width, snap.height, RGBLuminanceSource.BitmapFormat.ARGB32);
                if (res != null)
                {
                    if (tryGetSecurityCode(res.Text, out string? securityCode))
                    {
                        data = securityCode;
                        Debug.Log(data);
                        AppManager.Ins.Connect(securityCode);
                    }
                    else
                        UIManager.Ins.OpenPopup("Error", "Invalid QR code", UIManager.PopupButtons.Ok);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error decoding QR code: {ex}");
            }
        }

        private bool tryGetSecurityCode(string data, [NotNullWhen(true)] out string? securityCode)
        {
            securityCode = null;

            int index = data.IndexOf('#');
            if (index > -1 && index < data.Length - 1)
            {
                securityCode = data.Substring(index + 1);
                return true;
            }
            else
                return false;
        }

        private void OnDestroy()
        {
            camTexture.Stop();
        }
    }
}
