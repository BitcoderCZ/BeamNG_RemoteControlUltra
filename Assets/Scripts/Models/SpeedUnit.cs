using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace BeamNG.RemoteControlUltra.Models
{
    public enum SpeedUnit
    {
        Kmh,
        Mph
    }

    public static class SpeedUnitE
    {
        public static string GetDisplayText(this SpeedUnit speedUnit)
            => speedUnit switch
            {
                SpeedUnit.Kmh => "Km/h",
                SpeedUnit.Mph => "Mph",
                _ => throw new InvalidDataException($"Invalid speed unit '{speedUnit}'"),
            };
    }
}
