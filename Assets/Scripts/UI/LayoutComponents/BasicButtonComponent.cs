using BeamNG.RemoteControlUltra.UI.LayoutComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.LayoutComponents
{
    public class BasicButtonComponent : ButtonComponent
    {
        public override string TypeName => "Basic";
        public override Vector2 MinSize => new Vector2(140, 120);
    }
}
