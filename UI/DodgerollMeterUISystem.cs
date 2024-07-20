using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Dodgeroll.UI
{
    [Autoload(Side = ModSide.Client)]
    class DodgerollMeterUISystem : ModSystem
    {
        DodgerollMeterUI meterUI;
        UserInterface ui;

        GameTime lastUpdateUIGameTime;

        public override void Load()
        {
            meterUI = new DodgerollMeterUI();
            meterUI.Activate();

            ui = new UserInterface();
            ui.SetState(meterUI);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            lastUpdateUIGameTime = gameTime;
            if (ui?.CurrentState != null)
            {
                ui.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "Dodgeroll: Dodgeroll Meter",
                    () =>
                    {
                        if (lastUpdateUIGameTime != null && ui?.CurrentState != null)
                        {
                            ui.Draw(Main.spriteBatch, lastUpdateUIGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}