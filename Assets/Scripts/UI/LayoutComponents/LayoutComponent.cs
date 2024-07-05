using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.Models;
using BeamNG.RemoteControlUltra.UI.CustomComponents;
using BeamNG.RemoteControlUltra.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class LayoutComponent : MonoBehaviour, IPointerDownHandler
    {
        public abstract LayoutObjectType Type { get; }
        public abstract string TypeName { get; }
        public abstract Vector2 MinSize { get; }

        //public float Value { get; protected set; }

        public string Name
        {
            get => textMesh_name.text;
            set => textMesh_name.text = value;
        }
        private int slot;
        public int Slot
        {
            get => slot;
            set
            {
                slot = value;
                textMesh_slot.text = slot.ToString();
            }
        }

        private TransformAction transformAction = TransformAction.None;
        private int transformInput;
        private Vector2 grabOffset;

        private Corner? resizeCorner;

        protected RectTransform RT = null!;
        protected bool EditMode => CustomLayoutManager.Ins != null;
        protected bool CanBeEdited => UIManager.Ins.Count == 0 && EditMode;

        [SerializeField] private TextMeshProUGUI textMesh_name = null!;
        [SerializeField] private TextMeshProUGUI textMesh_slot = null!;
        [SerializeField] private List<ButtonPlus> cornerResizeButtons = null!;

        private void Start()
        {
            RT = GetComponent<RectTransform>();

            for (int i = 0; i < 4; i++)
            {
                int localI = i;
                cornerResizeButtons[i].OnDown += eventData =>
                {
                    if (!CanBeEdited) return;

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
            if (!CanBeEdited) return;

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

        public abstract void Edit();

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanBeEdited) return;

            if (transformAction != TransformAction.None) return;

            transform.SetAsLastSibling();

            transformAction = TransformAction.Move;

            eventData.Use();

            InputP.Update();
            TouchP touchP = InputP.GetClosestTouch(eventData.position)!.Value;
            transformInput = touchP.Id;
            grabOffset = RT.position.XY() - touchP.Touch.position;
        }

        public void StartResize(Corner corner, int inputId, Vector2 _grabOffset)
        {
            if (!CanBeEdited) return;

            if (resizeCorner != null || transformAction != TransformAction.None) return;

            transform.SetAsLastSibling();

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

        public void StopTransform()
        {
            switch (transformAction)
            {
                case TransformAction.Resize:
                    StopResize();
                    break;
            }

            transformAction = TransformAction.None;
        }

        private void OnDestroy()
        {
            InputP.OnTouchUp -= onInputUp;
        }

        private void onInputUp(TouchP touch)
        {
            if (touch.Id != transformInput) return;

            StopTransform();
        }

        enum TransformAction
        {
            None,
            Resize,
            Move
        }
    }
}
