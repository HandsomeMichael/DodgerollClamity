using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Dodgeroll.Common
{
    public class DodgerollPlayer : ModPlayer
    {
        private readonly int dodgerollLength = 20;

        private bool startDodgeroll = false;
        private bool isDodgerolling = false;
        private int dodgerollTimer = 0;
        private int invulnerableTimer = 0;

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
                dodgerollTimer = dodgerollLength;
                invulnerableTimer = (int)(dodgerollLength * DodgerollConfig.Instance.InvulnerableRatio);
            }

            base.ProcessTriggers(triggersSet);
        }

        public override void PreUpdate()
        {
            if (startDodgeroll)
            {
                startDodgeroll = false;

                Player.velocity += new Vector2(Player.direction * 5, 0);
                Player.immune = true;
                Player.immuneTime = invulnerableTimer;
            }

            if (dodgerollTimer <= 0) isDodgerolling = false;

            if (dodgerollTimer > 0) dodgerollTimer--;
            if (invulnerableTimer > 0) invulnerableTimer--;

            base.PreUpdate();
        }

        public override void PostUpdate()
        {
            if (isDodgerolling)
            {
                float progress = 1 - dodgerollTimer / (float)dodgerollLength;

                Player.fullRotationOrigin = new Vector2(11, 22);
                Player.fullRotation = Player.direction * MathHelper.Lerp(0, +MathHelper.TwoPi, progress);
            }

            base.PostUpdate();
        }
    }
}