using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class TransformUtils
    {
        /************************** Transform **************************/
        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
                children[i] = transform.GetChild(i);

            return children;
        }

        /************************** RectTransform **************************/

        // from: https://discussions.unity.com/t/set-a-recttranforms-pivot-without-changing-its-position/139741/4
        /// <summary>
        /// Set pivot without changing the position of the element
        /// </summary>
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
            deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
            deltaPosition.Scale(rectTransform.localScale);          // apply scaling
            deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

            rectTransform.pivot = pivot;                            // change the pivot
            rectTransform.localPosition -= deltaPosition;           // reverse the position change
        }
    }
}
