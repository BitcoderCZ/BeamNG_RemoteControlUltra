using BeamNG.RemoteControlLib;
using BeamNG.RemoteControlUltra.Layouts;
using BeamNG.RemoteControlUltra.Models;

#nullable enable
namespace BeamNG.RemoteControlUltra
{
    public class Settings
    {
        public SpeedUnit SpeedUnit { get; set; } = SpeedUnit.Kmh;

        public bool CustomLayout { get; set; } = false;

        public ControlsLayout CurrentLayout { get; set; } = ControlsLayout.Default;

        public ControlsLayout[] Layouts { get; set; } = { ControlsLayout.Default, ControlsLayout.Default, ControlsLayout.Default };

        public RequestedControls GetControlsRequest()
            => CustomLayout ? CurrentLayout.Request : RequestedControls.Default;
    }
}
