using BeamNG.RemoteControlUltra.Models;
using BeamNG.RemoteControlUltra.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace BeamNG.RemoteControlUltra
{
    public class Settings
    {
        public SpeedUnit SpeedUnit { get; set; } = SpeedUnit.Kmh;

        public bool CustomLayout { get; set; } = false;

        public ControlsLayout? CurrentLayout { get; set; }

        public List<ControlsLayout> Layouts { get; set; } = new List<ControlsLayout>();
    }
}
