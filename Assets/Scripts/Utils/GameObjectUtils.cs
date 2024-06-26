using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class GameObjectUtils
    {
        public static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (!(parent == null))
            {
                child.transform.SetParent(parent.transform, worldPositionStays: false);
                RectTransform? rectTransform = child.transform as RectTransform;
                // (bool) - same as C++ ?
                if (rectTransform is not null && (bool)rectTransform)
                {
                    rectTransform.anchoredPosition = Vector2.zero;
                    Vector3 localPosition = rectTransform.localPosition;
                    localPosition.z = 0f;
                    rectTransform.localPosition = localPosition;
                }
                else
                    child.transform.localPosition = Vector3.zero;

                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                setLayerRecursively(child, parent.layer);
            }

            void setLayerRecursively(GameObject go, int layer)
            {
                go.layer = layer;
                Transform transform = go.transform;
                for (int i = 0; i < transform.childCount; i++)
                    setLayerRecursively(transform.GetChild(i).gameObject, layer);
            }
        }
    }
}
