using Terraria;
using Terraria.ModLoader;

namespace DodgerollClamity.Content.Buffs
{
    public class BuffRaged : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Melee) += 0.2f;
        }
    }
    public class BuffHawked : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Ranged) += 0.05f;
            player.GetAttackSpeed(DamageClass.Ranged) += 0.2f;
        }
    }
    public class BuffFamilius : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Summon) += 0.05f;
            player.statDefense += Main.hardMode ? 10 : 5;
            player.moveSpeed += 0.3f;
        }
    }
    public class BuffMagical : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.manaRegenBonus += 10;
        }
    }
    public class BuffStealthy : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Throwing) += 0.05f;
        }
    }

    public class BuffPrecise : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Generic) += 50;
        }
    }
}