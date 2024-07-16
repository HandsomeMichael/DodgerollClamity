using System;
using System.Timers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Dodgeroll.Common
{
    public class DodgerollPlayer : ModPlayer
    {
        private bool startDodgeroll = false;
        private bool isDodgerolling = false;
        private int dodgerollTimer = 0;

        public static ModKeybind DodgerollKey { get; set; }

        public override void Load()
        {
            DodgerollKey = KeybindLoader.RegisterKeybind(Mod, "Dodgeroll", Keys.LeftControl);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            bool isDodgerollAvailable = DodgerollConfig.Instance.EnableDodgeroll &&
                (DodgerollKey?.JustPressed ?? false) &&
                !isDodgerolling;
            if (isDodgerollAvailable)
            {
                startDodgeroll = true;
                isDodgerolling = true;
                dodgerollTimer = 22;
            }

            base.ProcessTriggers(triggersSet);
        }

        public override void PreUpdate()
        {
            if (dodgerollTimer > 0)
            {
                dodgerollTimer--;
            }

            if (startDodgeroll)
            {
                Player.velocity += new Vector2(Player.direction * 5, 0);
                startDodgeroll = false;
            }

            if (isDodgerolling)
            {
                Player.immune = true;
                Player.immuneTime = dodgerollTimer;
            }

            if (dodgerollTimer <= 0)
            {
                isDodgerolling = false;
            }

            base.PreUpdate();
        }
    }
}