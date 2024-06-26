using BeamNG.RemoteControlUltra.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Managers
{
    public class Prefabs : Singleton<Prefabs>
    {
        protected override bool destroyPrevious => false;
        protected override bool dontDestroyOnLoad => true;

        [SerializeField] private List<GameObject> uiScenes = null!;
        public static List<GameObject> UIScenes => Ins.uiScenes;

        [SerializeField] private GameObject button = null!;
        public static GameObject Button => Ins.button;
    }
}
