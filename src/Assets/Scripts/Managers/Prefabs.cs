﻿using BeamNG.RemoteControlUltra.Utils;
using System.Collections.Generic;
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

        [SerializeField] private List<GameObject> buttonControls = null!;
        public static List<GameObject> ButtonControls => Ins.buttonControls;

        [SerializeField] private List<GameObject> axisControls = null!;
        public static List<GameObject> AxisControls => Ins.axisControls;
    }
}
