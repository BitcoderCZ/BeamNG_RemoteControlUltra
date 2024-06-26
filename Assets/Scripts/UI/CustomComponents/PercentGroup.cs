using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.CustomComponents
{
    [ExecuteAlways]
    public class PercentGroup : MonoBehaviour
    {
        public float PadStart;
        public float PadEnd;

        public float Spacing;

        public bool Reverse;

        /// <summary>
        /// The minimum size auto-size elements will have, if > 0 elements could overflow
        /// </summary>
        public float MinAutoSize;

        public RectTransform.Axis Orientation;

        /// <summary>
        /// Percentage (0.0 to 1.0) of how big the item at that index is, if < 0 the size is determined automatically
        /// </summary>
        public List<float>? Percentages;

        [HideInInspector] public bool UpdateRequested;

        private RectTransform rt = null!;
        private Rect lastRect;
        private int lastChildCount;

        private void Start()
        {
            rt = (RectTransform)transform;
            UpdateRequested = true;
        }

        private void Update()
        {
            if (!UpdateRequested && !Application.isEditor && rt.rect == lastRect && transform.childCount == lastChildCount)
                return;

            UpdateRequested = false;

            lastRect = rt.rect;
            lastChildCount = transform.childCount;

            bool horizontal = Orientation == RectTransform.Axis.Horizontal;

            RectTransform[] items = transform.GetChildren().Select(child => (RectTransform)child).ToArray();

            float totalSize = horizontal ? rt.rect.width : rt.rect.height;
            float paddedSize = totalSize - (PadStart + PadEnd + Spacing * (items.Length - 1));

            float[] sizes = new float[items.Length];
            Array.Fill(sizes, -1f);

            // set the size of non auto items
            float sizeUsed = 0f;
            int nonAutoItems = 0;
            for (int i = 0; i < items.Length; i++)
            {
                float percentage = getDesiredPercentage(i);
                if (percentage >= 0f)
                {
                    float size = paddedSize * percentage;
                    items[i].SetSizeWithCurrentAnchors(Orientation, size);
                    sizes[i] = size;
                    sizeUsed += size + Spacing;
                    nonAutoItems++;
                }
            }

            int autoItemsCount = items.Length - nonAutoItems;

            if (autoItemsCount > 0)
            { // set size of auto-size items
                float sizeLeft = paddedSize - sizeUsed;
                sizeLeft = Math.Max(sizeLeft, 0f); // make sure it's >= 0

                float sizeOfAutoSize = Math.Max(sizeLeft / autoItemsCount, MinAutoSize);

                // set size of autoElements
                for (int i = 0; i < items.Length; i++)
                    if (sizes[i] < 0f)
                    {
                        items[i].SetSizeWithCurrentAnchors(Orientation, sizeOfAutoSize);
                        sizes[i] = sizeOfAutoSize;
                    }
            }

            // set positions
            Action<int, float> setPos;
            if (horizontal)
                setPos = (index, pos) =>
                {
                    Vector3 currentPos = items[index].anchoredPosition;
                    items[index].anchoredPosition = new Vector3(pos, currentPos.y, currentPos.z);
                };
            else
                setPos = (index, pos) =>
                {
                    Vector3 currentPos = items[index].anchoredPosition;
                    items[index].anchoredPosition = new Vector3(currentPos.x, pos, currentPos.z);
                };

            float pos = PadStart * (Reverse ? -1f : 1f);
            for (int i = 0; i < items.Length; i++)
            {
                setPos(i, pos);
                if (Reverse)
                    pos -= sizes[i] + Spacing;
                else
                    pos += sizes[i] + Spacing;
            }

            float getDesiredPercentage(int index)
            {
                if (Percentages is null || index >= Percentages.Count) return -1f;
                else return Percentages[index];
            }
        }
    }
}
