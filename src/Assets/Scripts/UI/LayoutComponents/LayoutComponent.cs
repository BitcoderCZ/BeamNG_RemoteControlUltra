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

        public float Value { get; protected set; }

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
        [SerializeField] private List<GameObject> removeWhenNotEditing = null!;

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

            if (!EditMode && removeWhenNotEditing is not null)
                for (int i = 0; i < removeWhenNotEditing.Count; i++)
                    Destroy(removeWhenNotEditing[i]);

            InputP.OnTouchUp += OnInputUp;

            Init();
        }

        protected virtual void Init() { }

        protected virtual void Update()
        {
            if (!CanBeEdited)
            {
                if (!EditMode)
                    UpdateValue();

                return;
            }

            InputP.Update();

            if (transformAction == TransformAction.Resize && resizeCorner != null)
            {
                Vector2 inputPos = InputP.GetTouchById(transformInput).Touch.position + grabOffset;
                Vector2 size = (RT.anchoredPosition - inputPos) * resizeCorner.Value.NormalizationMultiplier();
                CheckSize(ref size);
                RT.sizeDelta = size;
            }
            else if (transformAction == TransformAction.Move)
            {
                RT.position = InputP.GetTouchById(transformInput).Touch.position + grabOffset;
            }
        }

        protected virtual void CheckSize(ref Vector2 size)
        {
            size = Vector2.Max(size, MinSize);
        }

        protected abstract void UpdateValue();

        public abstract void Edit();

        public virtual void OnPointerDown(PointerEventData eventData)
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
            transformInput = -1;
        }

        private void OnDestroy()
        {
            InputP.OnTouchUp -= OnInputUp;
        }

        protected virtual void OnInputUp(TouchP touch)
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
