using BeamNG.RemoteControlLib;
using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Layouts
{
    public class ControlsLayout
    {
        public const int MaxNumbButtons = 15;
        public const int MaxNumbAxes = 14; // 15 - Accelometer

        public int AccelometerAxes { get; set; } = 0;

        public int HighestButtonSlot => Buttons.Max(item => item.Key) + 1;
        public int HighestAxisSlot => Axes.Max(item => item.Key) + 1;

        public Dictionary<int, Button> Buttons { get; set; } = new();
        public Dictionary<int, Axis> Axes { get; set; } = new();

        [JsonConverter(typeof(JsonSubtypes), "Type")]
        [JsonSubtypes.KnownSubType(typeof(Button), "Button")]
        [JsonSubtypes.KnownSubType(typeof(Axis), "Axis")]
        public abstract class LayoutObject
        {
            public abstract LayoutObjectType Type { get; }

            public string Name { get; set; }

            public Vector2 Pos { get; set; }
            public Vector2 Size { get; set; }

            protected LayoutObject(string _name, Vector2 _pos, Vector2 _size)
            {
                Name = _name;
                Pos = _pos;
                Size = _size;
            }
        }

        public class Button : LayoutObject
        {
            public override LayoutObjectType Type => LayoutObjectType.Button;

            public Button(string _name, Vector2 _pos, Vector2 _size) : base(_name, _pos, _size)
            {
            }
        }

        public class Axis : LayoutObject
        {
            public override LayoutObjectType Type => LayoutObjectType.Axis;

            //public float DefaultValue { get; set; }
            //public bool SnapToDefaultWhenNotInUse { get; set; }
            //public float SnapToDefaultSpeed { get; set; }

            public Axis(string _name, Vector2 _pos, Vector2 _size) : base(_name, _pos, _size)
            {
            }
        }

        public enum LayoutObjectType
        {
            Button,
            Axis
        }
    }
}
