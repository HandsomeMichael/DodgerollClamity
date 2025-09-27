using System;
using System.Collections.Generic;
using DodgerollClamity.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace DodgerollClamity.UI
{

    public enum DodgerollMeterPosition
    {
        TOP, BOTTOM, LEFT, RIGHT
    }

    [Autoload(Side = ModSide.Client)]
    class DodgerollMeterUISystem : ModSystem
    {
        private const string VanillaInterfaceLayer = "Vanilla: Entity Health Bars";
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals(VanillaInterfaceLayer)) + 1;
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("NightreignPatch" + ": UI",
                    delegate
                    {
                        DrawPlayerMeter(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            var player = Main.LocalPlayer;
            if (player == null || player.dead || !player.active || player.CCed) return;
            if (!player.TryGetModPlayer(out DodgerollPlayer dodgeroll)) return;

            if (dodgeroll.staminaTimer == 0 || dodgeroll.Stamina == 0f)
            {
                lastStamina = MathHelper.Lerp(lastStamina,dodgeroll.Stamina,0.2f);
            }
            if (fadingTimer > 0 && dodgeroll.Stamina >= dodgeroll.MaxStamina) fadingTimer--;
            if (dodgeroll.Stamina < dodgeroll.MaxStamina && fadingTimer != fadingLength)
            {
                fadingTimer = fadingLength;
            }
        }

        public const byte shakeMax = 20;
        public static void NotEnoughStamina()
        {
            if (!Main.dedServ)
                ModContent.GetInstance<DodgerollMeterUISystem>().shake = shakeMax;
        }

        public byte shake;
        public byte fadingTimer;
        public const byte fadingLength = 40;
        public float lastStamina;
                
        public void DrawPlayerMeter(SpriteBatch spriteBatch)
        {
            var player = Main.LocalPlayer;
            if (player == null || player.dead || !player.active || player.CCed) return;
            if (!player.TryGetModPlayer(out DodgerollPlayer dodgeroll)) return;

            // if (fadingTimer > 0 && dodgeroll.Stamina >= dodgeroll.MaxStamina) fadingTimer--;
            // if (dodgeroll.Stamina < dodgeroll.MaxStamina && fadingTimer != fadingLength)
            // {
            //     fadingTimer = fadingLength;
            // }

            var opacity = fadingTimer / (float)fadingLength * (((float)DodgerollConfig.Instance.StaminaBarOpacity) / 100f);
            var progress = dodgeroll.Stamina / dodgeroll.MaxStamina;

            float cdProgress = 0f;
            if (dodgeroll.IsRolling())
            {
                cdProgress = 1f - (float)dodgeroll.dodgerollTimer / (float)dodgeroll.GetDodgeMax();
            }
            else
            {
                cdProgress = (float)dodgeroll.staminaTimer / (float)dodgeroll.GetStaminaCD();
            }

            DodgerollMeterPosition posType = DodgerollConfig.Instance.StaminaPosition;

            Vector2 position;
            switch (posType)
            {
                case DodgerollMeterPosition.TOP:position = player.Top - new Vector2(0,DodgerollConfig.Instance.StaminaPositionOffset);break;
                case DodgerollMeterPosition.BOTTOM:position = player.Bottom + new Vector2(0,DodgerollConfig.Instance.StaminaPositionOffset);break;
                case DodgerollMeterPosition.LEFT:position = player.Left - new Vector2(DodgerollConfig.Instance.StaminaPositionOffset,0);break;
                case DodgerollMeterPosition.RIGHT:position = player.Right + new Vector2(DodgerollConfig.Instance.StaminaPositionOffset,0);break;
                default:
                    position = player.Bottom + new Vector2(0,DodgerollConfig.Instance.StaminaPositionOffset);
                    break;
            }
            position -= Main.screenPosition;
            //position.Y += DodgerollConfig.Instance.StaminaPositionOffset;
            position += new Vector2(Main.rand.Next(-shake, shake), Main.rand.Next(-shake, shake)) / 2f;
            position /= Main.UIScale;
            if (shake > 0) shake--;

            var barTexture = ModContent.Request<Texture2D>("DodgerollClamity/UI/StaminaBar").Value;
            var barNope = ModContent.Request<Texture2D>("DodgerollClamity/UI/StaminaBar_Nope").Value;
            var frameTexture = ModContent.Request<Texture2D>("DodgerollClamity/UI/StaminaFrame").Value;
            var frameBack = ModContent.Request<Texture2D>("DodgerollClamity/UI/StaminaFrame_Back").Value;
            var staminaCD = ModContent.Request<Texture2D>("DodgerollClamity/UI/StaminaCD").Value;

            var barRec = new Rectangle(0, 0, (int)(barTexture.Width * progress), barTexture.Height);
            var barNopeRec = new Rectangle(0, 0, (int)(barTexture.Width * lastStamina), barTexture.Height);
            var staminaCDRec = new Rectangle(0, 0, (int)(barTexture.Width * cdProgress), barTexture.Height);
            var orig = frameTexture.Size() / 2f;

            var lightColor = Lighting.GetColor((int)player.Bottom.X / 16, (int)player.Bottom.Y / 16);
            var color =  lightColor * opacity;//new Color(defactoColor,defactoColor,defactoColor) * opacity;
            float rotation = 0f;
            SpriteEffects effect = SpriteEffects.None;

            if (posType == DodgerollMeterPosition.LEFT || posType == DodgerollMeterPosition.RIGHT)
            {
                rotation = MathHelper.ToRadians(90);
                //effect = SpriteEffects.FlipHorizontally;
            }


            spriteBatch.Draw(frameBack, position, null, color, rotation, orig, 1f, effect, 0f);
            if (opacity > 0f) spriteBatch.Draw(barNope, position, barNopeRec, color, rotation, orig, 1f, effect, 0f);
            spriteBatch.Draw(barTexture, position, barRec, color, rotation, orig, 1f, effect, 0f);
            spriteBatch.Draw(frameTexture, position, null, color, rotation, orig, 1f, effect, 0f);

            spriteBatch.Draw(staminaCD, position, staminaCDRec, color, rotation, orig, 1f, effect, 0f);

        }
    }
}