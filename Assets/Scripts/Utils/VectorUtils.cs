using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class VectorUtils
    {
        public static Vector2 XY(this Vector3 v)
            => new Vector2(v.x, v.y);

        public static Vector2 Abs(this Vector2 v)
            => new Vector2(MathF.Abs(v.x), MathF.Abs(v.y));
    }
}
