using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra
{
    public class Save
    {
        private static string path => Path.Combine(Application.persistentDataPath, "save.json");

        public static Save Ins { get; private set; } = new Save();

        public bool SeenLegalNotice { get; set; } = false;

        static Save()
        {
            Read();
        }

        public static void Write()
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Ins));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save: {ex}");
            }
        }
        public static void Read()
        {
            if (!File.Exists(path))
            {
                Ins = new Save();
                Write();
                return;
            }

            try
            {
                Save? save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(path));
                if (save is null) throw new NullReferenceException();
                else Ins = save;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load: {ex}");

                Ins = new Save();
                Write();
            }
        }
    }
}
