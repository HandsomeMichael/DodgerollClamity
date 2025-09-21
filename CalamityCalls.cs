using System;
using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DodgerollClamity
{
    public static class CalamityCalls
    {
        public static bool IsRogueClass(DamageClass damage)
        {
            if (DodgerollClamity.Get.calamity != null)
            {
                return IsRogueClass_Inner(damage);
            }
            return damage.Name == "RogueDamageClass";
        }

        [JITWhenModsEnabled("CalamityMod")]
        private static bool IsRogueClass_Inner(DamageClass damage)
        {
            return damage.CountsAsClass<RogueDamageClass>();
        }

        public static void GiveRogueStealth(Player player, float value) { if (DodgerollClamity.Get.calamity != null) GiveRogueStealth_Inner(player, value); }

        [JITWhenModsEnabled("CalamityMod")]
        private static void GiveRogueStealth_Inner(Player player, float value)
        {
            var playerClam = player.Calamity();
            playerClam.rogueStealth = Math.Min(playerClam.rogueStealth + playerClam.rogueStealthMax * value, playerClam.rogueStealthMax);
        }

        public static bool IsAdrenaline(Player player)
        {
            if (DodgerollClamity.Get.calamity != null)
            {
                return IsAdrenaline_Inner(player);
            }
            return false;
        }

        [JITWhenModsEnabled("CalamityMod")]
        private static bool IsAdrenaline_Inner(Player player)
        {
            return player.Calamity().adrenalineModeActive;
        }

        // [JITWhenModsEnabled("CalamityMod")]
        // private static void YharimGiftEffect(Player player,IEntitySource source)
        // {
        //     int num3 = (int)player.GetBestClassDamage().ApplyTo(375f);
        //     num3 = player.ApplyArmorAccDamageBonusesTo(num3);
        //     CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<SkyFlareFriendly>(), num3, 9f, player.whoAmI);
        // }
    }
}