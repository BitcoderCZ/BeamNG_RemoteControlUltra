using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Models
{
    public enum Corner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    public static class CornerE
    {
        public static Vector2 GetPivot(this Corner corner)
            => corner switch
            {
                Corner.TopLeft => new Vector2(0, 1),
                Corner.TopRight => new Vector2(1, 1),
                Corner.BottomLeft => new Vector2(0, 0),
                Corner.BottomRight => new Vector2(1, 0),
                _ => throw new InvalidDataException($"Unknown Corner '{corner}'"),
            };

        public static Corner Oposite(this Corner corner) => corner switch
        {
            Corner.TopLeft => Corner.BottomRight,
            Corner.TopRight => Corner.BottomLeft,
            Corner.BottomLeft => Corner.TopRight,
            Corner.BottomRight => Corner.TopLeft,
            _ => throw new InvalidDataException($"Unknown Corner '{corner}'"),
        };

        public static Vector2 NormalizationMultiplier(this Corner corner)
            => corner switch
            {
                Corner.TopLeft => new Vector2(1, -1),
                Corner.TopRight => new Vector2(-1, -1),
                Corner.BottomLeft => new Vector2(1, 1),
                Corner.BottomRight => new Vector2(-1, 1),
                _ => throw new InvalidDataException($"Unknown Corner '{corner}'"),
            };
    }
}
