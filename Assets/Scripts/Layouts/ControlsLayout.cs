using BeamNG.RemoteControlLib;
using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Layouts
{
    public class ControlsLayout
    {
        public const int MaxNumbButtons = 15;
        public const int MaxNumbAxes = 14; // 15 - Accelometer

        public const int MaxSlotsPerControlType = 15;

        public static ControlsLayout Default => new ControlsLayout();

        public int AccelometerAxis { get; set; } = 0;

        [JsonIgnore] public RequestedControls Request => new RequestedControls(HighestButtonSlot + 1, Math.Max(HighestAxisSlot, AccelometerAxis + 1));
        [JsonIgnore] public int HighestButtonSlot => Buttons.Max(item => (int?)item.Key) ?? -1;
        [JsonIgnore] public int HighestAxisSlot => Axes.Max(item => (int?)item.Key) ?? -1;

        public Dictionary<int, Button> Buttons { get; set; } = new();
        public Dictionary<int, Axis> Axes { get; set; } = new();

        public static int MaxCount(LayoutObjectType type)
            => type switch
            {
                LayoutObjectType.Button => MaxNumbButtons,
                LayoutObjectType.Axis => MaxNumbAxes,
                _ => throw new InvalidDataException($"Unknown LayoutObjectType '{type}'"),
            };

        public ControlsLayout Clone()
            => new ControlsLayout()
            {
                AccelometerAxis = this.AccelometerAxis,
                Buttons = this.Buttons.ToDictionary(item => item.Key, item => (Button)item.Value.Clone()),
                Axes = this.Axes.ToDictionary(item => item.Key, item => (Axis)item.Value.Clone()),
            };

        public abstract class LayoutObject
        {
            public abstract LayoutObjectType Type { get; }

            public string TypeName { get; set; }
            public string Name { get; set; }

            public int Order { get; set; }
            public Vector2 Pos { get; set; }
            public Vector2 Size { get; set; }

            protected LayoutObject(string _typeName, string _name, int _order, Vector2 _pos, Vector2 _size)
            {
                TypeName = _typeName;
                Name = _name;
                Order = _order;
                Pos = _pos;
                Size = _size;
            }

            public abstract LayoutObject Clone();
        }

        public class Button : LayoutObject
        {
            public override LayoutObjectType Type => LayoutObjectType.Button;

            public Button(string _typeName, string _name, int _order, Vector2 _pos, Vector2 _size) : base(_typeName, _name, _order, _pos, _size)
            {
            }

            public override LayoutObject Clone()
                => new Button(TypeName, Name, Order, Pos, Size);
        }

        public class Axis : LayoutObject
        {
            public override LayoutObjectType Type => LayoutObjectType.Axis;

            public float DefaultValue { get; set; }
            public bool SnapToDefaultWhenNotInUse { get; set; }
            public float SnapToDefaultSpeed { get; set; }

            public Axis(string _typeName, string _name, int _order, Vector2 _pos, Vector2 _size) : base(_typeName, _name, _order, _pos, _size)
            {
            }

            public override LayoutObject Clone()
                => new Axis(TypeName, Name, Order, Pos, Size)
                {
                    DefaultValue = this.DefaultValue,
                    SnapToDefaultWhenNotInUse = this.SnapToDefaultWhenNotInUse,
                    SnapToDefaultSpeed = this.SnapToDefaultSpeed,
                };
        }
    }
}
