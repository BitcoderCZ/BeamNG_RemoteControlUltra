using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.CustomComponents
{
    [ExecuteAlways]
    public class ConstantDimensions : MonoBehaviour
    {
        public WidthOrHeight BasedOn;
        public bool StretchAndFit;
        public float Ratio = 1f;

        private RectTransform rectTransform = null!;
        private RectTransform parent = null!;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parent = (RectTransform)rectTransform.parent;
        }

        private void Update()
        {
            WidthOrHeight basedOn = BasedOn;

            if (basedOn == WidthOrHeight.Smaller)
                basedOn = rectTransform.rect.width < rectTransform.rect.height ? WidthOrHeight.Width : WidthOrHeight.Height;
            else if (basedOn == WidthOrHeight.Larger)
                basedOn = rectTransform.rect.width > rectTransform.rect.height ? WidthOrHeight.Width : WidthOrHeight.Height;

            if (StretchAndFit)
            {
                float newWidth = rectTransform.rect.width, newHeight = rectTransform.rect.height;

                if (basedOn == WidthOrHeight.Width)
                {
                    newWidth = parent.rect.width;
                    newHeight = newWidth * Ratio;
                    if (newHeight > parent.rect.height)
                    {
                        newHeight = parent.rect.height;
                        newWidth = newHeight / Ratio;
                    }
                }
                else if (basedOn == WidthOrHeight.Height)
                {
                    newHeight = parent.rect.height;
                    newWidth = newHeight * Ratio;
                    if (newWidth > parent.rect.width)
                    {
                        newWidth = parent.rect.width;
                        newHeight = newWidth / Ratio;
                    }
                }

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            }
            else
            {
                if (basedOn == WidthOrHeight.Width)
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.width * Ratio);
                else if (basedOn == WidthOrHeight.Height)
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.height * Ratio);
            }
        }

        public enum WidthOrHeight
        {
            Width,
            Height,
            Smaller,
            Larger
        }
    }
}
