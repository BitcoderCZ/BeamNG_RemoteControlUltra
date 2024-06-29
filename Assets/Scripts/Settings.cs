using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Models;
using System.Collections.Generic;

#nullable enable
namespace BeamNG.RemoteControlUltra
{
    public class Settings
    {
        public SpeedUnit SpeedUnit { get; set; } = SpeedUnit.Kmh;

        public bool CustomLayout { get; set; } = false;

        public ControlsLayout CurrentLayout { get; set; } = ControlsLayout.Default;

        public List<ControlsLayout> Layouts { get; set; } = new List<ControlsLayout>();
    }
}
