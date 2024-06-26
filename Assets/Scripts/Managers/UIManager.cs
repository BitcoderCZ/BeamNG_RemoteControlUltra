using BeamNG.RemoteControlUltra.Collections;
using BeamNG.RemoteControlUltra.UI;
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
    public class UIManager : Singleton<UIManager>
    {
        protected override bool destroyPrevious => true;
        protected override bool dontDestroyOnLoad => false;

        [HideInInspector] public float Count => activeUI.Count;
        [HideInInspector] public UIScene? TopUI { get => activeUI.Count > 0 ? activeUI.Peek()?.UIScene : null; }

        private StackList<ActiveUI> activeUI = new StackList<ActiveUI>();

        private new void Awake()
        {
            base.Awake();
            if (destroyed)
                return;
            if (Application.isPlaying)
                for (int i = 0; i < transform.childCount; i++)
                    Destroy(transform.GetChild(i).gameObject);
        }

        public T? GetUI<T>() where T : UIScene
        {
            for (int i = activeUI.Count - 1; i >= 0; i--)
            {
                ActiveUI ui = activeUI[i];
                if (ui.UIScene is T scene)
                    return scene;
            }

            return null;
        }

        public void OpenUI(string name, Action<object?>? onClose = null, object? value = null)
#pragma warning disable CS0618 // Type or member is obsolete
            => SetUI(getUIByName(name), false, onClose, value);
#pragma warning restore CS0618
        [Obsolete("Use OpenUI instead")]
        public void SetUI(string name, bool closeOther, Action<object?>? onClose = null, object? value = null)
            => SetUI(getUIByName(name), closeOther, onClose, value);
        [Obsolete("Use OpenUI instead")]
        public void SetUI(GameObject ui, bool closeOther, Action<object?>? onClose = null, object? value = null)
        {
            if (closeOther) CloseUI(true);
            else TopUI?.OnNewUIOpen();

            GameObject go = Instantiate(ui, transform);
            GameObjectUtils.SetParentAndAlign(go, gameObject);
            ActiveUI _activeUI = new ActiveUI(go.GetComponent<UIScene>(), onClose);
            _activeUI.UIScene.Value = value;
            _activeUI.UIScene.Init();
            activeUI.Push(_activeUI);
        }

        public void OpenPopup(string title, string message, PopupButtons buttons, bool closeOnEscape = true, Action<PopupResult>? callback = null)
            => OpenPopup(new PopupOptions(title, message, buttons, closeOnEscape), callback);
        public void OpenPopup(PopupOptions options, Action<PopupResult>? callback = null)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            OpenUI("Popup", result => callback?.Invoke((PopupResult)result!), options);
        }

        private GameObject getUIByName(string uiName)
        {
            GameObject? go = Prefabs.UIScenes.FirstOrDefault(prefab => prefab.name == uiName);

            if (go is null) throw new Exception($"Couldn't fint ui scene \"{uiName}\"");
            else return go;
        }

        public void CloseUI(bool closeAll = true)
        {
            if (activeUI.Count < 1)
                return;

            activeUI.Pop()!.Close();

            if (closeAll)
                while (activeUI.Count > 0)
                    activeUI.Pop()!.Close();
        }

        internal class ActiveUI
        {
            public UIScene UIScene;
            public Action<object?>? OnClose;

            public ActiveUI(UIScene _UIScene, Action<object?>? _onClose)
            {
                UIScene = _UIScene;
                OnClose = _onClose;
            }

            public void Close()
            {
                OnClose?.Invoke(UIScene.Value);
                Destroy(UIScene.gameObject);
            }
        }

        public class PopupOptions
        {
            private string? title;
            public string Title
            {
                get => title ?? string.Empty;
                set => title = value;
            }

            private string? message;
            public string Message
            {
                get => message ?? string.Empty;
                set => message = value;
            }

            public PopupButtons Buttons;

            public bool CloseOnEscape;

            public PopupOptions(string _title, string _message, PopupButtons _buttons, bool _closeOnEscape)
            {
                Title = _title;
                Message = _message;
                Buttons = _buttons;
                CloseOnEscape = _closeOnEscape;
            }
        }

        public enum PopupButtons
        {
            None,
            Ok,
            OkCancel,
            YesNo,
        }

        public enum PopupResult
        {
            /// <summary>
            /// User pressed close or escape/back button
            /// </summary>
            Close,
            Ok,
            Cancel,
            Yes,
            No,
        }
    }
}
