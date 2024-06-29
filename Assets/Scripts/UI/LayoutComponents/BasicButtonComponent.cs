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
