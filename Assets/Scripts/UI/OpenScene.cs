using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
