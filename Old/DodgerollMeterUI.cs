// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Terraria.UI;
// using Terraria;
// using DodgerollClamity.Content;
// using System;
// using Terraria.ModLoader;

// namespace DodgerollClamity.UI
// {

//     public class DodgerollMeterUI : UIState
//     {
//         DodgerollMeter dodgerollMeter;

//         public override void OnInitialize()
//         {
//             dodgerollMeter = new();
//             Append(dodgerollMeter);
//         }
//     }

//     public enum DodgerollMeterPosition
//     {
//         TOP, BOTTOM, LEFT, RIGHT
//     }

//     // Old
//     // class DodgerollMeter : UIElement
//     // {
//     //     static readonly int side = 6;
//     //     static readonly int largerSide = side * 5;
//     //     static readonly int padding = 8;
//     //     readonly int fadingLength = 60;

//     //     Player player;
//     //     DodgerollPlayer dodgeroll;
//     //     Texture2D barTexture;
//     //     Texture2D staminaTexture;
//     //     int fadingTimer = 0;

//     //     public override void Draw(SpriteBatch spriteBatch)
//     //     {
//     //         if (player == null || dodgeroll == null) return;

//     //         barTexture ??= new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
//     //         staminaTexture ??= new Texture2D(Main.graphics.GraphicsDevice, 1, 1);

//     //         var opacity = fadingTimer / (float)fadingLength;
//     //         var opacityColor = new Color(opacity, opacity, opacity, opacity);

//     //         var barColor = Color.Gray.MultiplyRGBA(opacityColor);
//     //         var staminaColor = Color.Gold.MultiplyRGBA(opacityColor);

//     //         barTexture.SetData(new Color[] { barColor });
//     //         staminaTexture.SetData(new Color[] { staminaColor });

//     //         var progress = dodgeroll.Stamina / dodgeroll.MaxStamina;
//     //         var width = 0;
//     //         var height = 0;
//     //         var staminaWidth = 0;
//     //         var staminaHeight = 0;

//     //         switch (DodgerollConfig.Instance.StaminaPosition)
//     //         {
//     //             case DodgerollMeterPosition.TOP:
//     //             case DodgerollMeterPosition.BOTTOM:
//     //                 width = largerSide;
//     //                 height = side;
//     //                 staminaWidth = (int)(width * progress);
//     //                 staminaHeight = height;
//     //                 break;
//     //             case DodgerollMeterPosition.LEFT:
//     //             case DodgerollMeterPosition.RIGHT:
//     //                 width = side;
//     //                 height = largerSide;
//     //                 staminaWidth = width;
//     //                 staminaHeight = (int)(height * progress);
//     //                 break;
//     //         }

//     //         var barRectangle = new Rectangle(0, 0, width + padding, height + padding);
//     //         var staminaRectangle = new Rectangle(0, 0, staminaWidth, staminaHeight);

//     //         var toCenterOffset = new Vector2(barRectangle.Width / 2, barRectangle.Height / 2);

//     //         var anchorPosition = DodgerollConfig.Instance.StaminaPosition switch
//     //         {
//     //             DodgerollMeterPosition.TOP => player.Top,
//     //             DodgerollMeterPosition.BOTTOM => player.Bottom,
//     //             DodgerollMeterPosition.LEFT => player.Left,
//     //             DodgerollMeterPosition.RIGHT => player.Right,
//     //             _ => throw new NotImplementedException(),
//     //         };

//     //         var offset = DodgerollConfig.Instance.StaminaPosition switch
//     //         {
//     //             DodgerollMeterPosition.TOP => new Vector2(0, DodgerollConfig.Instance.StaminaPositionOffset),
//     //             DodgerollMeterPosition.BOTTOM => new Vector2(0, -DodgerollConfig.Instance.StaminaPositionOffset),
//     //             DodgerollMeterPosition.LEFT => new Vector2(DodgerollConfig.Instance.StaminaPositionOffset, 0),
//     //             DodgerollMeterPosition.RIGHT => new Vector2(-DodgerollConfig.Instance.StaminaPositionOffset, 0),
//     //             _ => throw new NotImplementedException(),
//     //         };

//     //         var toCenterStaminaOffset = new Vector2(padding / 2, padding / 2);
//     //         if (DodgerollConfig.Instance.StaminaPosition == DodgerollMeterPosition.LEFT || DodgerollConfig.Instance.StaminaPosition == DodgerollMeterPosition.RIGHT)
//     //         {
//     //             toCenterStaminaOffset.Y += height - staminaHeight;
//     //         }

//     //         var position = (anchorPosition - Main.screenPosition) / Main.UIScale - offset;
//     //         var barPosition = position - toCenterOffset;
//     //         var staminaPosition = position + toCenterStaminaOffset - toCenterOffset;

//     //         spriteBatch.Draw(barTexture, barPosition, barRectangle, barColor);
//     //         spriteBatch.Draw(staminaTexture, staminaPosition, staminaRectangle, staminaColor);
//     //     }

//     //     public override void Update(GameTime gameTime)
//     //     {
//     //         base.Update(gameTime);

//     //         if (fadingTimer > 0 && dodgeroll.Stamina >= dodgeroll.MaxStamina) fadingTimer--;

//     //         player = Main.LocalPlayer;
//     //         if (!player.TryGetModPlayer(out dodgeroll)) return;

//     //         if (dodgeroll.Stamina < dodgeroll.MaxStamina) fadingTimer = fadingLength;
//     //     }
//     // }
// }
