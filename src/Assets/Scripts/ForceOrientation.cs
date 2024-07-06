using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra
{
    public class ForceOrientation : MonoBehaviour
    {
        [SerializeField] private ScreenOrientation orientation;
        [SerializeField] private ScreenOrientation onDestroyOrientation = ScreenOrientation.AutoRotation;

        private void Start()
        {
            Screen.orientation = orientation;
        }

        private void OnDestroy()
        {
            Screen.orientation = onDestroyOrientation;
        }
    }
}
