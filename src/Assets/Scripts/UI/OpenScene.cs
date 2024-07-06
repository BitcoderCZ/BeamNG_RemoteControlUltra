using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI
{
    public class OpenScene : MonoBehaviour
    {
        public void Open(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
