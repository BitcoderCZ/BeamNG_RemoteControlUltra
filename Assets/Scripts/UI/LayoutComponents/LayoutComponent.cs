using BeamNG.RemoteControlUltra.Models;
using BeamNG.RemoteControlUltra.UI.CustomComponents;
using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class LayoutComponent : MonoBehaviour, IPointerDownHandler
    {
        public abstract Vector2 MinSize { get; }

        private TransformAction transformAction = TransformAction.None;
        private int transformInput;
        private Vector2 grabOffset;

        private Corner? resizeCorner;

        protected RectTransform RT = null!;

        [SerializeField] private List<ButtonPlus> cornerResizeButtons = null!;

        private void Start()
        {
            RT = GetComponent<RectTransform>();

            for (int i = 0; i < 4; i++)
            {
                int localI = i;
                cornerResizeButtons[i].OnDown += eventData =>
                {
                    eventData.Use();

                    InputP.Update();
                    TouchP touchP = InputP.GetClosestTouch(eventData.position)!.Value;
                    RectTransform rt = cornerResizeButtons[localI].GetComponent<RectTransform>();
                    StartResize((Corner)localI, touchP.Id, (rt.position.XY() - rt.anchoredPosition) - touchP.Touch.position);
                };
            }

            InputP.OnTouchUp += onInputUp;
        }

        private void Update()
        {
            InputP.Update();

            if (transformAction == TransformAction.Resize && resizeCorner != null)
            {
                Vector2 inputPos = InputP.GetTouchById(transformInput).Touch.position + grabOffset;
                RT.sizeDelta = Vector2.Max((RT.anchoredPosition - inputPos) * resizeCorner.Value.NormalizationMultiplier(), MinSize);
            }
            else if (transformAction == TransformAction.Move)
            {
                RT.position = InputP.GetTouchById(transformInput).Touch.position + grabOffset;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (transformAction != TransformAction.None) return;

            transformAction = TransformAction.Move;

            eventData.Use();

            InputP.Update();
            TouchP touchP = InputP.GetClosestTouch(eventData.position)!.Value;
            transformInput = touchP.Id;
            grabOffset = RT.position.XY() - touchP.Touch.position;
        }

        public void StartResize(Corner corner, int inputId, Vector2 _grabOffset)
        {
            if (resizeCorner != null || transformAction != TransformAction.None) return;

            transformAction = TransformAction.Resize;

            resizeCorner = corner;
            transformInput = inputId;
            grabOffset = _grabOffset;

            RT.SetPivot(corner.Oposite().GetPivot());
        }

        public void StopResize()
        {
            resizeCorner = null;
            RT.SetPivot(new Vector2(0.5f, 0.5f));

            transformAction = TransformAction.None;
        }

        private void OnDestroy()
        {
            InputP.OnTouchUp -= onInputUp;
        }

        private void onInputUp(TouchP touch)
        {
            if (touch.Id != transformInput) return;

            switch (transformAction)
            {
                case TransformAction.Resize:
                    StopResize();
                    break;
                case TransformAction.Move:
                    transformAction = TransformAction.None;
                    break;
            }
        }

        enum TransformAction
        {
            None,
            Resize,
            Move
        }
    }
}
