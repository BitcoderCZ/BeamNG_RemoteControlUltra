using BeamNG.RemoteControlUltra.Managers;
using BeamNG.RemoteControlUltra.UI;
using System;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.Scenes
{
    public class UI_SaveLoadLayout : UIScene
    {
        protected override bool CloseWhenEscape => true;

        public void Write(int slot)
        {
            checkSlot(slot);

            CustomLayoutManager.Ins.Write();

            Save.Ins.Settings.Layouts[slot] = Save.Ins.Settings.CurrentLayout.Clone();
        }

        public void Read(int slot)
        {
            checkSlot(slot);

            UIManager.Ins.CloseUI(true);

            Save.Ins.Settings.CurrentLayout = Save.Ins.Settings.Layouts[slot].Clone();

            CustomLayoutManager.Ins.Read();
        }

        private void checkSlot(int slot)
        {
            if (slot < 0 || slot >= Save.Ins.Settings.Layouts.Length)
                throw new ArgumentOutOfRangeException(nameof(slot));
        }
    }
}
