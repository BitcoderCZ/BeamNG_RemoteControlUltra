using System;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class VectorUtils
    {
        public static Vector3 XYN(this Vector2 v)
            => new Vector3(v.x, v.y, 0f);
        public static Vector3 XYO(this Vector2 v)
            => new Vector3(v.x, v.y, 1f);

        public static Vector2 XY(this Vector3 v)
            => new Vector2(v.x, v.y);

        public static Vector2 Abs(this Vector2 v)
            => new Vector2(MathF.Abs(v.x), MathF.Abs(v.y));
    }
}
