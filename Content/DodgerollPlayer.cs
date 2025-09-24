using System;
using System.IO;
using CalamityMod;
using CalamityMod.Items.Accessories;
using DodgerollClamity.Content.Buffs;
using DodgerollClamity.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
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
        public byte dodgeType;
        public float statDodgeCD;

        // bonus stats

        public enum BonusType
        {
            None,
            Melee,
            Ranged,
            Summon,
            Magic,
            Rogue,
            Healer,
            Any,
            AnyPerfect
        }
        public BonusType bonusType;
        public short bonusTimer;

        public override void ResetEffects()
        {
            statDodgeCD = 0f;
            statDodgeRegen = 0;
            statDodgeBoost = 0;
            statDodgeTime = 0;
            statDodgeStaminaUsage = 0f;
            rollInstinct = false;
            yharimsGift = false;


            if (!Main.dedServ && bonusTimer > 0 && bonusType != BonusType.Any)
            {
                int num = 0;
                num += Player.bodyFrame.Y / 56;
                if (num >= Main.OffsetsPlayerHeadgear.Length) num = 0;

                Vector2 vector = Main.OffsetsPlayerHeadgear[num];
                vector *= Player.Directions;
                Vector2 vector2 = new Vector2(Player.width / 2, Player.height / 2) + vector + (Player.MountedCenter - Player.Center);
                Player.sitting.GetSittingOffsetInfo(Player, out var posOffset, out var seatAdjustment);
                vector2 += posOffset + new Vector2(0f, seatAdjustment);
                if (Player.face == 19) vector2.Y -= 5f * Player.gravDir;
                if (Player.head == 276) vector2.X += 2.5f * (float)direction;

                if (Player.mount.Active && Player.mount.Type == 52)
                {
                    vector2.X += 14f * (float)direction;
                    vector2.Y -= 2f * Player.gravDir;
                }

                float y = -11.5f * Player.gravDir;
                Vector2 vector3 = new Vector2(3 * direction - ((direction == 1) ? 1 : 0), y) + Vector2.UnitY * Player.gfxOffY + vector2;
                Vector2 vector4 = new Vector2(3 * Player.shadowDirection[1] - ((direction == 1) ? 1 : 0), y) + vector2;
                Vector2 vector5 = Vector2.Zero;
                if (Player.mount.Active && Player.mount.Cart)
                {
                    int num2 = Math.Sign(Player.velocity.X);
                    if (num2 == 0)
                        num2 = direction;

                    vector5 = new Vector2(MathHelper.Lerp(0f, -8f, Player.fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(Player.fullRotation / ((float)Math.PI / 4f)))).RotatedBy(Player.fullRotation);
                    if (num2 == Math.Sign(Player.fullRotation))
                        vector5 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(Player.fullRotation / ((float)Math.PI / 4f)));
                }

                if (Player.fullRotation != 0f)
                {
                    vector3 = vector3.RotatedBy(Player.fullRotation, Player.fullRotationOrigin);
                    vector4 = vector4.RotatedBy(Player.fullRotation, Player.fullRotationOrigin);
                }

                float num3 = 0f;
                Vector2 vector6 = Player.position + vector3 + vector5;
                Vector2 vector7 = Player.oldPosition + vector4 + vector5;
                vector7.Y -= num3 / 2f;
                vector6.Y -= num3 / 2f;
                float num4 = 0.3f;

                int num5 = (int)Vector2.Distance(vector6, vector7) / 3 + 1;
                if (Vector2.Distance(vector6, vector7) % 3f != 0f) num5++;

                for (float num6 = 1f; num6 <= (float)num5; num6 += 1f)
                {
                    Dust obj = Main.dust[Dust.NewDust(Player.Center, 0, 0, DustID.TheDestroyer)];
                    // a
                    obj.rotation = Player.velocity.ToRotation();
                    obj.position = Vector2.Lerp(vector7, vector6, num6 / (float)num5);
                    obj.noGravity = true;
                    obj.velocity = Vector2.Zero;
                    obj.customData = this;
                    obj.scale = num4;
                    obj.shader = GameShaders.Armor.GetShaderFromItemId(DodgeEyeEffect());//GameShaders.Armor.GetSecondaryShader(Player.cYorai, Player);
                }
            }
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
            int cd = (int)(DodgerollConfig.Instance.StaminaCooldown * 60);
            return cd - (int)((float)cd * statDodgeCD);
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
                    modPacket.Write((byte)DodgerollClamity.MessageType.InstinctDodgedServer);
                    modPacket.Send();
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

                if (Player.HeldItem != null && !Player.HeldItem.IsAir && Player.HeldItem.damage > 0)
                {
                    if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
                    {
                        bonusType = BonusType.Melee;
                        bonusTimer = 60 * 4;
                    }
                    else if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Ranged))
                    {
                        bonusType = BonusType.Ranged;
                        bonusTimer = 60 * 4;
                    }
                    else if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Summon))
                    {
                        bonusType = BonusType.Summon;
                        bonusTimer = 60 * 8;
                    }
                    else if (Player.HeldItem.DamageType.CountsAsClass(DamageClass.Magic))
                    {
                        bonusType = BonusType.Magic;
                        bonusTimer = 60 * 8;
                    }
                    else if (CalamityCalls.IsRogueClass(Player.HeldItem.DamageType))
                    {
                        CalamityCalls.GiveRogueStealth(Player, 0.9f);
                        bonusType = BonusType.Rogue;
                        bonusTimer = 60 * 3;
                    }
                    else
                    {
                        bonusType = BonusType.AnyPerfect;
                        bonusTimer = 60 * 5;
                    }
                }

                if (DodgerollClamity.Get.calamity != null)
                {
                    UpdateCalamityRoverDrive();
                }
            }
            else
            {
                if (Player.HeldItem != null && !Player.HeldItem.IsAir && Player.HeldItem.damage > 0 && CalamityCalls.IsRogueClass(Player.HeldItem.DamageType))
                {
                    CalamityCalls.GiveRogueStealth(Player, 0.25f);
                }
                else
                {
                    bonusType = BonusType.Any;
                    bonusTimer = 60 * 3; // 3 second of 5% more damage
                }
                CombatText.NewText(Player.Hitbox, Color.LightYellow, "Dodged", true);
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

                // send this to the server lil bro
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket modPacket = Mod.GetPacket();
                    modPacket.Write((byte)DodgerollClamity.MessageType.FuckingDodgeServer);
                    modPacket.WriteVector2(dodgeBoost);
                    modPacket.Write(dodgeDirection);
                    modPacket.Send();
                }
            }
        }

        public void InitiateDodgeroll(Vector2 dodgeBoost, int dodgeDirection)
        {
            //Main.NewText("Dodgeroll initiated for "+Player.name);
            state = DodgerollState.STARTED;

            var staminaCost = GetStaminaUsage();
            Stamina = Math.Max(0, Stamina - staminaCost);

            boost = dodgeBoost;
            direction = dodgeDirection;
            dodgerollTimer = GetDodgeMax();
            dodgedSomething = false;

            if (!Main.dedServ)
                SoundEngine.PlaySound(new SoundStyle("DodgerollClamity/Sounds/Roll" + Main.rand.Next(1, 4)).WithVolumeScale(0.5f).WithPitchOffset(Main.rand.NextFloat(0.9f, 1.1f)), Player.Center);

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

        // public void HandleFuckingDodge(BinaryReader reader, int whoAmI)
        // {
        //     Vector2 dodgeBoost = reader.ReadVector2();
        //     int dodgeDirection = reader.ReadInt32();
        //     // Main.NewText("Handling Dodge data");
        //     InitiateDodgeroll(dodgeBoost, dodgeDirection);
        // }

        public int DodgeEyeEffect()
        {
            switch (bonusType)
            {
                case BonusType.Melee: return ItemID.RedDye;
                case BonusType.Ranged: return ItemID.OrangeDye;
                case BonusType.Magic: return ItemID.BlueDye;
                case BonusType.Summon: return ItemID.GreenDye;
                case BonusType.Healer: return ItemID.LimeDye;
                case BonusType.Rogue: return ItemID.PurpleDye;
                case BonusType.AnyPerfect: return ItemID.TealDye;
                default: return ItemID.YellowDye;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            // disable any of these mechanic when player is adrenalined
            if (CalamityCalls.IsAdrenaline(Player)) return;

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
                        if (DodgerollClamity.Get.calamity != null)
                        {
                            AbsorberEffect(i);
                        }
                        else
                        {
                            Player.buffTime[i] = Math.Max(Player.buffTime[i] - 50, 1);
                        }
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
            if (bonusTimer > 0 && bonusType == BonusType.Melee)
            {
                scale += 0.3f;
            }
        }

        public override void PostUpdateEquips()
        {
            // apply buffs
            if (bonusTimer > 0)
            {
                switch (bonusType)
                {
                    case BonusType.Melee:
                        Player.GetDamage(DamageClass.Melee) += 0.2f;
                        break;
                    case BonusType.Ranged:
                        Player.GetDamage(DamageClass.Ranged) += 0.05f;
                        Player.GetAttackSpeed(DamageClass.Ranged) += 0.2f;
                        break;
                    case BonusType.Magic:
                        Player.GetDamage(DamageClass.Magic) += 0.1f;
                        Player.manaRegenBonus += 10;
                        break;
                    case BonusType.Summon:
                        Player.GetDamage(DamageClass.Summon) += 0.05f;
                        Player.statDefense += Main.hardMode ? 10 : 5;
                        Player.moveSpeed += 0.3f;
                        break;
                    case BonusType.Rogue:
                        Player.GetDamage(DamageClass.Throwing) += 0.05f;
                        break;
                    case BonusType.Any:
                        statDodgeBoost += 0.20f;
                        break;
                    case BonusType.AnyPerfect:
                        Player.GetDamage(DamageClass.Generic) += 0.1f;
                        break;
                    default: break;
                }

                if (DodgerollClamity.Get.calamity != null)
                {
                    UpdateCalamityBuff();
                }

                bonusTimer--;
            }
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

        #region Calamity Methods
        [JITWhenModsEnabled("CalamityMod")]
        public void UpdateCalamityBuff()
        {
            var playerClam = Player.Calamity();
            if (playerClam.dodgeScarf)
            {
                Player.GetDamage(DamageClass.Generic) += 0.1f;
            }

            if (playerClam.evasionScarf)
            {
                Player.GetDamage(DamageClass.Generic) += 0.15f;
            }

            if (playerClam.warbannerOfTheSun)
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.5f;
            }

            if (playerClam.spiritOrigin)
            {
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.5f;
            }
        }

        [JITWhenModsEnabled("CalamityMod")]
        public void AbsorberEffect(int i)
        {
            if (Player.Calamity().absorber && IsPerfectDodge())
            {
                Player.buffTime[i] = Math.Max(Player.buffTime[i] - 60 * 5, 1);
            }
            else
            {
                Player.buffTime[i] = Math.Max(Player.buffTime[i] - 50, 1);
            }
        }

        [JITWhenModsEnabled("CalamityMod")]
        public void UpdateCalamityRoverDrive()
        {
            // but jesse what if calamity doesnt cap the shield durabili I DONT CARE FUCK YOU
            Player.Calamity().RoverDriveShieldDurability += 5;
        }
        #endregion
    }
}