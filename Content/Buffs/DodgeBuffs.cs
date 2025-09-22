using Terraria;
using Terraria.ModLoader;

namespace DodgerollClamity.Content.Buffs
{
    public class BuffPrecise : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Generic) += 50;
        }
    }
}