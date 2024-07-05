using BeamNG.RemoteControlLib;
using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Utils;
using BeamNG.RemoteControlUltra.Utils.Converters;
using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace BeamNG.RemoteControlUltra.Managers
{
    public class AppManager : Singleton<AppManager>
    {
        protected override bool destroyPrevious => false;
        protected override bool dontDestroyOnLoad => true;

        public GameInstance? GameInstance { get; private set; }

        private ConcurrentQueue<Action> mainThreadQueue = new();

        private void Start()
        {
            var cams = WebCamTexture.devices;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new Vector2Converter(),
                    JsonSubtypesConverterBuilder
                        .Of(typeof(ControlsLayout.LayoutObject), "Type")
                        .RegisterSubtype(typeof(ControlsLayout.Button), LayoutObjectType.Button)
                        .RegisterSubtype(typeof(ControlsLayout.Axis), LayoutObjectType.Axis)
                        .Build(),
                },
            };

            if (!Save.Ins.SeenLegalNotice)
            {
                UIManager.Ins.OpenPopup("Legal Notice!", "This app is an independent creation and is not affiliated, associated, authorized, endorsed by, or in any way officially connected with BeamNG GmbH, the creators of BeamNG.drive. All product and company names are trademarks™ or registered® trademarks of their respective holders. Use of them does not imply any affiliation with or endorsement by them.\n\nBeamNG.drive and the BeamNG.drive logo are trademarks of BeamNG GmbH.", UIManager.PopupButtons.Ok, callback: result =>
                {
                    Save.Ins.SeenLegalNotice = true;
                    Save.Write();
                });
            }
        }

        private void Update()
        {
            InputP.Update();

            for (int _ = 0; _ < 200; _++)
                try
                {
                    if (mainThreadQueue.Count == 0) break;

                    if (mainThreadQueue.TryDequeue(out Action action))
                        action();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
        }

        public void Connect(string securityCode)
            => StartCoroutine(connect(securityCode));
        private IEnumerator connect(string securityCode)
        {
            yield return null;
            SceneManager.LoadScene("Connecting", LoadSceneMode.Single);
            yield return null;
            UIManager.Ins.OpenUI("Text", value: "Connecting...");

            var task = GameInstance.Find(securityCode, SystemInfo.deviceModel, Save.Ins.Settings.GetControlsRequest());

            DateTime timeOut = DateTime.UtcNow.AddSeconds(30.0);
            while (!task.IsCompleted && DateTime.UtcNow < timeOut)
                yield return null;

            ApiResponse<GameInstance.GameFindStatus, GameInstance> res;
            if (!task.IsCompleted || (res = task.Result).Status == GameInstance.GameFindStatus.Timeout)
            {
                UIManager.Ins.OpenPopup("Error", "Couldn't find game instance: Timed out", UIManager.PopupButtons.Ok, callback: result =>
                {
                    SceneManager.LoadScene(0, LoadSceneMode.Single);
                });
                yield break;
            }

            if (res is ErrorApiResponse<GameInstance.GameFindStatus, GameInstance> error)
            {
                UIManager.Ins.OpenPopup("Error", error.ErrorMsg, UIManager.PopupButtons.Ok, callback: result =>
                {
                    SceneManager.LoadScene(0, LoadSceneMode.Single);
                });
                yield break;
            }

            GameInstance = ((OkApiResponse<GameInstance.GameFindStatus, GameInstance>)res).Result;
            GameInstance.Start();

            yield return null;

            SceneManager.LoadScene("Controller", LoadSceneMode.Single);
        }

        public void Reconnect()
            => StartCoroutine(reconnect());
        private IEnumerator reconnect()
        {
            if (GameInstance is null) yield break;

            GameInstance.Stop(); // needs to be stopped before SendConnectRequest()

            yield return null;
            SceneManager.LoadScene("Connecting", LoadSceneMode.Single);
            yield return null;
            UIManager.Ins.OpenUI("Text", value: "Connecting...");

            var task = GameInstance.SendConnectRequest(SystemInfo.deviceModel, Save.Ins.Settings.GetControlsRequest());

            DateTime timeOut = DateTime.UtcNow.AddSeconds(30.0);
            while (!task.IsCompleted && DateTime.UtcNow < timeOut)
                yield return null;

            ApiResponse<GameInstance.GameFindStatus, object?> resp;

            if (!task.IsCompleted)
            {
                GameInstance = null;
                UIManager.Ins.OpenPopup("Error", "Couldn't reconnect: Timed out", UIManager.PopupButtons.Ok, callback: result =>
                {
                    SceneManager.LoadScene(0, LoadSceneMode.Single);
                });
            }
            else if ((resp = task.Result) is ErrorApiResponse<GameInstance.GameFindStatus, object?> error)
            {
                GameInstance = null;
                UIManager.Ins.OpenPopup("Error", error.ErrorMsg, UIManager.PopupButtons.Ok, callback: result =>
                {
                    SceneManager.LoadScene(0, LoadSceneMode.Single);
                });
            }
            else
            {
                GameInstance.Start();

                UIManager.Ins.OpenPopup("Info", $"Reconnected to: {GameInstance.GameEndPoint}", UIManager.PopupButtons.Ok, callback: result =>
                {
                    SceneManager.LoadScene("Controller", LoadSceneMode.Single);
                });
            }
        }

        public void Disconnect()
        {
            GameInstance?.Dispose();
            GameInstance = null;
        }

        public void RunOnMainThread(Action action)
           => mainThreadQueue.Enqueue(action);

        public void RunNextFrame(Action action)
            => StartCoroutine(runNextFrame(action));
        private IEnumerator runNextFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        private void OnDestroy()
        {
            GameInstance?.Stop();
        }
    }
}