using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Dodgeroll.Content
{
    public class DodgerollPlayer : ModPlayer
    {
        private DodgerollState state = DodgerollState.NONE;
        private Vector2 boost = Vector2.Zero;
        private int direction = 1;
        private int dodgerollTimer = 0;
        private int invulnerableTimer = 0;
        private int staminaTimer = 0;

        public float Stamina { get; set; } = 1;
        public float MaxStamina { get; } = 1;

        public static ModKeybind DodgerollKey { get; set; }

        public override void Load()
        {
            DodgerollKey = KeybindLoader.RegisterKeybind(Mod, "Dodgeroll", Keys.LeftControl);
        }

        public override bool CanUseItem(Item item)
        {
            if (!DodgerollConfig.Instance.EnableItemUseMidroll)
            {
                return state == DodgerollState.NONE || state == DodgerollState.FINISHED;
            }

            return base.CanUseItem(item);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            var isDodgerollAvailable = DodgerollConfig.Instance.EnableDodgeroll &&
                (DodgerollKey?.JustPressed ?? false) &&
                state == DodgerollState.NONE &&
                Stamina >= DodgerollConfig.Instance.StaminaUsage &&
                !Player.mount.Active;
            if (isDodgerollAvailable)
            {
                state = DodgerollState.STARTED;

                var staminaCost = DodgerollConfig.Instance.EnableStamina ? DodgerollConfig.Instance.StaminaUsage : 0;
                Stamina = Math.Max(0, Stamina - staminaCost);

                var defaultDirection = new Vector2(Player.direction, 0);
                boost = triggersSet.DirectionsRaw.SafeNormalize(defaultDirection) * DodgerollConfig.Instance.DodgerollBoost;
                direction = triggersSet.DirectionsRaw.X == 0 ? Player.direction : (int)triggersSet.DirectionsRaw.X;

                dodgerollTimer = DodgerollConfig.Instance.DodgerollLength;
                invulnerableTimer = (int)(DodgerollConfig.Instance.DodgerollLength * DodgerollConfig.Instance.InvulnerableRatio);
            }

            base.ProcessTriggers(triggersSet);
        }

        public override void PreUpdate()
        {
            if (state == DodgerollState.STARTED)
            {
                state = DodgerollState.ROLLING;

                Player.RemoveAllGrapplingHooks();
                Player.velocity += boost;
                Player.immune = true;
                Player.immuneTime = invulnerableTimer;
            }

            if (state == DodgerollState.ROLLING && dodgerollTimer <= 0)
            {
                state = DodgerollState.FINISHED;
            }

            if (state == DodgerollState.FINISHED)
            {
                state = DodgerollState.NONE;

                staminaTimer = (int)(DodgerollConfig.Instance.StaminaCooldown * 60);
            }

            var shouldRegenStamina = state == DodgerollState.NONE &&
                staminaTimer <= 0 &&
                Stamina < MaxStamina;
            if (shouldRegenStamina)
            {
                Stamina = Math.Min(Stamina + DodgerollConfig.Instance.StaminaRegenRate / 60, MaxStamina);
            }

            if (dodgerollTimer > 0) dodgerollTimer--;
            if (invulnerableTimer > 0) invulnerableTimer--;
            if (staminaTimer > 0) staminaTimer--;

            base.PreUpdate();
        }

        public override void PostUpdate()
        {
            if (state == DodgerollState.ROLLING)
            {
                var progress = 1 - dodgerollTimer / (float)DodgerollConfig.Instance.DodgerollLength;

                Player.direction = direction;
                Player.fullRotationOrigin = Player.Center - Player.position;
                Player.fullRotation = direction * MathHelper.Lerp(0, MathHelper.TwoPi, progress);
            }

            base.PostUpdate();
        }
    }
}