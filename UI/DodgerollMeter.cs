using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using DodgeRoll.Content;

namespace Dodgeroll.UI
{
    class DodgerollMeter : UIElement
    {
        static readonly int side = 6;
        static readonly int largerSide = side * 5;
        static readonly int offset = 8;

        static readonly Rectangle barRectangle = new(0, 0, side * 5 + offset, side + offset);
        static readonly Rectangle staminaRectangle = new(0, 0, side * 5, side);
        static readonly Vector2 toCenterOffset = new(barRectangle.Width / 2, barRectangle.Height / 2);
        static readonly Vector2 toCenterStaminaOffset = new(offset / 2, offset / 2);

        readonly int fadingLength = 60;

        Player player;
        DodgerollPlayer dodgeroll;
        int fadingTimer = 0;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (player == null || dodgeroll == null) return;

            var barTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            var staminaTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);

            var opacity = fadingTimer / (float)fadingLength;
            var opacityColor = new Color(opacity, opacity, opacity, opacity);

            var barColor = Color.Gray.MultiplyRGBA(opacityColor);
            var staminaColor = Color.Gold.MultiplyRGBA(opacityColor);

            barTexture.SetData(new Color[] { barColor });
            staminaTexture.SetData(new Color[] { staminaColor });

            var progress = dodgeroll.Stamina / dodgeroll.MaxStamina;
            var currentStaminaRectangle = new Rectangle(0, 0, (int)(staminaRectangle.Width * progress), staminaRectangle.Height);

            var position = (player.Center - Main.screenPosition) / Main.UIScale - new Vector2(0, player.height / 2f + 15);
            var barPosition = position - toCenterOffset;
            var staminaPosition = position + toCenterStaminaOffset - toCenterOffset;

            spriteBatch.Draw(barTexture, barPosition, barRectangle, barColor);
            spriteBatch.Draw(staminaTexture, staminaPosition, currentStaminaRectangle, staminaColor);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (fadingTimer > 0 && dodgeroll.Stamina >= dodgeroll.MaxStamina) fadingTimer--;

            player = Main.LocalPlayer;
            if (!player.TryGetModPlayer(out dodgeroll)) return;

            if (dodgeroll.Stamina < dodgeroll.MaxStamina) fadingTimer = fadingLength;
        }
    }
}
