using System;
using System.IO;
using CalamityMod.Items.Accessories;
using DodgerollClamity.Content.Buffs;
using DodgerollClamity.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace DodgerollClamity.Content
{
    class DodgerollPlayer : ModPlayer
    {
        // Actual stats
        public short statDodgeTime;
        public float statDodgeBoost;
        public float statDodgeRegen;
        public float statDodgeStaminaUsage;
        public bool rollInstinct;
        public bool yharimsGift;

        public override void ResetEffects()
        {
            statDodgeRegen = 0;
            statDodgeBoost = 0;
            statDodgeTime = 0;
            statDodgeStaminaUsage = 0f;
            rollInstinct = false;
            yharimsGift = false;
        }

        // Dodgeroll states

        DodgerollState state = DodgerollState.NONE;
        Vector2 boost = Vector2.Zero;
        public int direction = 1;
        public int dodgerollTimer = 0;
        public int GetDodgeMax()
        {
            return statDodgeTime + DodgerollConfig.Instance.DodgerollLength;
        }
        public int staminaTimer = 0;
        public int GetStaminaCD()
        {
            return (int)(DodgerollConfig.Instance.StaminaCooldown * 60);
        }
        public bool dodgedSomething = false;
        public float Stamina { get; set; } = 1;
        public float MaxStamina { get; } = 1;

        // System instances

        public static ModKeybind DodgerollKey { get; set; }

        public bool IsRolling()
        {
            return state == DodgerollState.ROLLING;
        }

        public override void Load()
        {
            DodgerollKey = KeybindLoader.RegisterKeybind(Mod, "Dodgeroll", Keys.LeftControl);    
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (IsRolling() && IsPerfectDodge() && damageSource.IsOther(DeathReasonOtherID.FallDamage))
            {
                CombatText.NewText(Player.Hitbox, Color.Gold, "Lucky...", true);
                Player.statLife = 1;
                playSound = false;
                genDust = false;
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (!IsRolling() && rollInstinct && Stamina == MaxStamina && (info.Damage > Player.statLife || Player.statLife <= 50) && !Player.CCed)
            {
                CombatText.NewText(Player.Hitbox, Color.Red, "Instinct!", true);
                InstinctDodged();

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket modPacket = Mod.GetPacket();
                    modPacket.Write((byte)DodgerollClamity.MessageType.FuckingDodge);
                    modPacket.Write(Player.whoAmI);
                    modPacket.Send(-1, Main.myPlayer);
                }
                return true;
            }
            return base.FreeDodge(info);
        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            // dodge any undodgeeable stuff except fall damage
            if (dodgerollTimer > 0 && !damageSource.IsOther(DeathReasonOtherID.FallDamage))
            {
                // handles dodge bonus
                if (!dodgedSomething && dodgeable)
                {
                    GiveDodgeBonus(damageSource);
                    dodgedSomething = true;
                }
                //Main.NewText("asd"+damageSource.SourceOtherIndex);

                // would still immune to any damage including those annoying cactus damage from fargo god i hate that thing
                return true;
            }
            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
        public void InstinctDodged()
        {
            InitiateDodgeroll(Vector2.Zero, Player.direction); // no dodge boost
            staminaTimer = GetStaminaCD() * 2;
            Stamina = 0;
            dodgedSomething = true; // grant no bonus
            Player.immune = true;
            Player.immuneTime = 60;
        }

        public bool IsPerfectDodge()
        {
            int deltaDodge = GetDodgeMax() - dodgerollTimer;
            return deltaDodge <= perfectDodgeThreeshold;
        }
        public const int perfectDodgeThreeshold = 5;
        public void GiveDodgeBonus(PlayerDeathReason source)
        {
            if (CalamityCalls.IsAdrenaline(Player))
            {
                return;
            }

            // give 5% stamina back
            float staminaBonus = 0.05f;
            bool perfectDodge = IsPerfectDodge();

            if (perfectDodge)
            {
                staminaBonus = 0.05f;

                if (DodgerollConfig.Instance.PerfectRollTriggerHurtEffect)
                {
                    Player.HurtModifiers modifiers = new()
                    {
                        DamageSource = source,
                        PvP = false,
                        CooldownCounter = -1,
                        Dodgeable = true,
                        HitDirection = Player.direction
                    };

                    var info = modifiers.ToHurtInfo(0, Player.statDefense, Player.DefenseEffectiveness.Value, 0f, false);

                    PlayerLoader.OnHurt(Player, info);
                    PlayerLoader.PostHurt(Player, info);
                }


                if (Player.starCloakItem != null && !Player.starCloakItem.IsAir)
                {
                    DoStarCloak();
                }

                // give back 5% of mana, might need rebalance idk
                if (Player.magicCuffs)
                {
                    Player.statMana = Math.Min(Player.statManaMax2, Player.statMana + (int)((float)Player.statManaMax2 * 0.5f));
                }

                if (Player.panic)
                {
                    Player.AddBuff(BuffID.Panic, 30);
                }

                if (Player.honeyCombItem != null && !Player.honeyCombItem.IsAir)
                {
                    DoHoneyComb();
                }

                if (yharimsGift)
                {
                    Player.AddBuff(ModContent.BuffType<BuffPrecise>(), 50);
                    CombatText.NewText(Player.Hitbox, Color.Gold, "Precise!", true);
                }
                else
                {
                    CombatText.NewText(Player.Hitbox, Color.Gold, "Perfect!", true);
                }
            }
            else
            {
                CombatText.NewText(Player.Hitbox, Color.LightYellow, "Dodged", true);
            }

            if (Player.HeldItem != null && !Player.HeldItem.IsAir && Player.HeldItem.damage > 0)
            {
                if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
                {
                    Player.AddBuff(ModContent.BuffType<BuffRaged>(), perfectDodge ? 80 : 15);
                }
                else if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Ranged))
                {
                    Player.AddBuff(ModContent.BuffType<BuffHawked>(), perfectDodge ? 70 : 15);
                }
                else if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Summon))
                {
                    Player.AddBuff(ModContent.BuffType<BuffFamilius>(), perfectDodge ? 90 : 30);
                }
                else if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Magic))
                {
                    Player.AddBuff(ModContent.BuffType<BuffMagical>(), perfectDodge ? 60 : 20);
                }
                else if (CalamityCalls.IsRogueClass(Player.HeldItem.DamageType))
                {
                    // dodge grant from 25% or 90% stealth
                    CalamityCalls.GiveRogueStealth(Player, perfectDodge ? 0.9f : 0.25f);
                    Player.AddBuff(ModContent.BuffType<BuffStealthy>(), 60);
                }
            }

            // give 5% more if its a projectile
            if (source.SourceProjectileType > 0)
            {
                staminaBonus += 0.05f;
            }

            Stamina = Math.Min(Stamina + staminaBonus, MaxStamina);
        }
        
        /// <summary>
        /// Do nerfed honey comb
        /// </summary>
        public void DoHoneyComb()
        {
            int num20 = 1;
            if (Main.rand.Next(3) == 0) num20++;
            if (Player.strongBees && Main.rand.Next(3) == 0) num20++;

            float damage = 8f;
            if (Player.strongBees) damage = 10f;

            if (Main.masterMode) damage *= 2f;
            else if (Main.expertMode) damage *= 1.5f;

            IEntitySource projectileSource_Accessory = Player.GetSource_Accessory(Player.honeyCombItem);
            for (int num22 = 0; num22 < num20; num22++)
            {
                float speedX = (float)Main.rand.Next(-35, 36) * 0.02f;
                float speedY = (float)Main.rand.Next(-35, 36) * 0.02f;
                Projectile.NewProjectile(projectileSource_Accessory, Player.position.X, Player.position.Y, speedX, speedY, Player.beeType(), Player.beeDamage((int)damage), Player.beeKB(0f), Main.myPlayer);
            }

            Player.AddBuff(48, 300);
        }
        
        /// <summary>
        /// Do nerfed star cloak
        /// </summary>
        public void DoStarCloak()
        {
            float x = Player.position.X + (float)Main.rand.Next(-400, 400);
            float y = Player.position.Y - (float)Main.rand.Next(500, 800);
            Vector2 vector = new Vector2(x, y);
            float num16 = Player.position.X + (float)(Player.width / 2) - vector.X;
            float num17 = Player.position.Y + (float)(Player.height / 2) - vector.Y;
            num16 += (float)Main.rand.Next(-100, 101);
            float num18 = (float)Math.Sqrt(num16 * num16 + num17 * num17);
            num18 = 23f / num18;
            num16 *= num18;
            num17 *= num18;
            int type = 726;
            Item item = Player.starCloakItem;
            if (Player.starCloakItem_starVeilOverrideItem != null)
            {
                item = Player.starCloakItem_starVeilOverrideItem;
                type = 725;
            }

            if (Player.starCloakItem_beeCloakOverrideItem != null)
            {
                item = Player.starCloakItem_beeCloakOverrideItem;
                type = 724;
            }

            if (Player.starCloakItem_manaCloakOverrideItem != null)
            {
                item = Player.starCloakItem_manaCloakOverrideItem;
                type = 723;
            }

            int num19 = 15;
            if (Main.masterMode) num19 *= 3;
            else if (Main.expertMode) num19 *= 2;

            Projectile.NewProjectile(Player.GetSource_Accessory(item), x, y, num16, num17, type, num19, 5f, Player.whoAmI, 0f, Player.position.Y);
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
            // dont do dodgeroll
            if (!DodgerollConfig.Instance.EnableDodgeroll || state != DodgerollState.NONE || Player.dead || Player.mount.Active || Player.CCed) return;

            bool dodgeKeyPressed = DodgerollKey?.JustPressed ?? false;
            bool haveStamina = Stamina >= GetStaminaUsage();

            if (dodgeKeyPressed)
            {
                if (!haveStamina)
                {
                    SoundEngine.PlaySound(new SoundStyle("DodgerollClamity/Sounds/NoRoll"), Player.Center);
                    DodgerollMeterUISystem.NotEnoughStamina();
                    return;
                }

                // Main.NewText("Local client dodged");

                var defaultDirection = new Vector2(Player.direction, 0);
                var dodgeBoost = triggersSet.DirectionsRaw.SafeNormalize(defaultDirection) * DodgerollConfig.Instance.DodgerollBoost * (1f + statDodgeBoost);
                var dodgeDirection = triggersSet.DirectionsRaw.X == 0 ? Player.direction : (int)triggersSet.DirectionsRaw.X;

                InitiateDodgeroll(dodgeBoost, dodgeDirection);

                // sync, how ? i dont fucking know
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    // Main.NewText("Sync dodge data");
                    ModPacket modPacket = Mod.GetPacket();
                    modPacket.Write((byte)DodgerollClamity.MessageType.FuckingDodge);
                    modPacket.Write(Player.whoAmI);
                    modPacket.WriteVector2(dodgeBoost);
                    modPacket.Write(dodgeDirection);
                    modPacket.Send(-1, Main.myPlayer);
                }
            }
        }

        public void InitiateDodgeroll(Vector2 dodgeBoost, int dodgeDirection)
        {
            // Main.NewText("Dodgeroll initiated");
            state = DodgerollState.STARTED;

            var staminaCost = GetStaminaUsage();
            Stamina = Math.Max(0, Stamina - staminaCost);

            boost = dodgeBoost;
            direction = dodgeDirection;
            dodgerollTimer = GetDodgeMax();
            dodgedSomething = false;

            SoundEngine.PlaySound(new SoundStyle("DodgerollClamity/Sounds/Roll"+Main.rand.Next(1,4)), Player.Center);
            
            // didnt work ig
            // if (DodgerollConfig.Instance.CancelItemUseMidroll)
            // {
            //     Player.itemAnimation = 0;
            //     Player.itemTime = 0;
            //     if (Player.heldProj > 0)
            //     {
            //         Main.projectile[Player.heldProj].Kill();
            //     }
            // }
        }

        public float GetStaminaUsage()
        {
            if (!ShouldUseStamina())
            {
                return 0f;
            }
            
            float staminaUsage = DodgerollConfig.Instance.StaminaUsage + statDodgeStaminaUsage;
            return Utils.Clamp(staminaUsage, 0f, 1f);
        }
        public bool ShouldUseStamina()
        {
            if (CalamityCalls.IsAdrenaline(Player))
            {
                return false;
            }
            return true;
        }

        public void HandleFuckingDodge(BinaryReader reader, int whoAmI)
        {
            var dodgeBoost = reader.ReadVector2();
            var dodgeDirection = reader.ReadInt32();
            // Main.NewText("Handling Dodge data");
            InitiateDodgeroll(dodgeBoost, dodgeDirection);
        }

        public override void PostUpdateMiscEffects()
        {
            // disable dashing upon rolling or out of stamina
            if ((Stamina <= 0.1f || IsRolling()) && DodgerollConfig.Instance.DashRequireStamina)
            {
                Player.dashType = 0;
            }

            // reduce stamina
            if (Player.dashType > 0 && Player.timeSinceLastDashStarted == 1)
            {
                // Stamina = Math.Max(0, Stamina - GetStaminaUsage() * 0.5f);
                staminaTimer = GetStaminaCD();
            }
        }

        public override void PreUpdate()
        {
            //Main.NewText("state "+state.ToString()+" on "+dodgerollTimer);

            if (state == DodgerollState.STARTED)
            {
                state = DodgerollState.ROLLING;
                Player.RemoveAllGrapplingHooks();

                // better boost management
                if (boost.X > 0)
                {
                    if (Player.velocity.X + boost.X < boost.X) { Player.velocity.X = boost.X; }
                    else { Player.velocity.X += boost.X; }
                }
                else
                {
                    if (Player.velocity.X + boost.X > boost.X) { Player.velocity.X = boost.X; }
                    else { Player.velocity.X += boost.X; }
                }
                Player.velocity.Y += boost.Y;

                //Player.velocity += boost;

                // half fall damage
                int tilePositionY = (int)(Player.position.Y / 16f);
                Player.fallStart = (int)MathHelper.Lerp(Player.fallStart, tilePositionY, 0.5f);

                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffType = Player.buffType[i];
                    //buffType.IsAny(BuffID.OnFire, BuffID.OnFire3, BuffID.CursedInferno)
                    if (Player.buffTime[i] > 1 && Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                    {
                        Player.buffTime[i] = Math.Max(Player.buffTime[i] - 40, 1);
                    }
                }
            }

            if (state == DodgerollState.ROLLING && dodgerollTimer <= 0)
            {
                state = DodgerollState.FINISHED;
            }

            if (state == DodgerollState.FINISHED)
            {
                state = DodgerollState.NONE;
                staminaTimer = GetStaminaCD();

                // reset rotation
                Player.fullRotationOrigin = Player.Center - Player.position;
                Player.fullRotation = 0f;
            }

            if (dodgerollTimer > 0) dodgerollTimer--;
            if (staminaTimer > 0) staminaTimer--;

        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (Player.HasBuff(ModContent.BuffType<BuffRaged>()))
            {
                scale += 0.25f;
            }
        }

        public override void PostUpdateEquips()
        {

            // regen stamina
            if (state == DodgerollState.NONE && staminaTimer <= 0 && Stamina < MaxStamina)
            {
                // reset dodged
                dodgedSomething = false;
                if (Player.IsStandingStillForSpecialEffects)
                {
                    statDodgeRegen += 0.1f;// gain 10/s
                }

                Stamina = Math.Min(Stamina + (DodgerollConfig.Instance.StaminaRegenRate + statDodgeRegen) / 60, MaxStamina);
            }
        }


        public override void PostUpdate()
        {
            // roll effect
            if (state == DodgerollState.ROLLING)
            {
                Player.armorEffectDrawShadow = true;
                float progressMax = GetDodgeMax();
                var progress = 1 - dodgerollTimer / progressMax;

                Player.direction = direction;
                Player.fullRotationOrigin = Player.Center - Player.position;
                Player.fullRotation = direction * MathHelper.Lerp(0, MathHelper.TwoPi, progress);
            }
        }
    }
}