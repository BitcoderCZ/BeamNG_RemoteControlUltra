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
        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
                children[i] = transform.GetChild(i);

            return children;
        }
    }
}
