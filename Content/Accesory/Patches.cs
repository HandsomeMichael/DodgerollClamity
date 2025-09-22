using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DodgerollClamity.Content.Accesory
{
    // at some point ill probs make lib for all my shit    
    public abstract class ModItemPatch : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.ModItem != null && entity.ModItem.Mod.Name == ModName && ItemName == entity.ModItem.Name;
        }
        public virtual string ItemName => "cum";
        public virtual string ModName => "CalamityMod";

        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod(ModName);
        }
    }

    public abstract class ModItemPatchMultiple : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.ModItem != null && entity.ModItem.Mod.Name == ModName && ItemName.Contains(entity.ModItem.Name);
        }
        public virtual string[] ItemName => new string[] { "cum" };
        public virtual string ModName => "CalamityMod";

        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod(ModName);
        }
    }

    public class MasterNinjaGear1 : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.ClimbingClaws || entity.type == ItemID.ShoeSpikes;
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeBoost += 0.3f;
            }
        }
    }

    public class MasterNinjaGear2 : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.MasterNinjaGear;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase dodgeroll boost by 90%\nReduce stamina usage by 10%"));
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeBoost += 0.9f;
                pl.statDodgeStaminaUsage -= 0.1f;
            }
        }
    }

    public class EvilBossAcc : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BrainOfConfusion || entity.type == ItemID.WormScarf;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Slightly increase dodgeroll time"));
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeTime += 4;
            }
        }
    }

    public class OnHurtVanillas : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
        entity.type == ItemID.HoneyComb || entity.type == ItemID.SweetheartNecklace || entity.type == ItemID.PanicNecklace ||
        entity.type == ItemID.HoneyBalloon || entity.type == ItemID.BeeCloak || entity.type == ItemID.StingerNecklace || entity.type == ItemID.StarCloak || entity.type == ItemID.BeeCloak;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Also applies on [c/F6AE2A:perfect] dodgeroll"));
        }
    }

    public class StatisPatch : ModItemPatch
    {
        public override string ItemName => "StatisNinjaBelt";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase dodgeroll boost by 100%"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeBoost += 1f;
            }
        }
    }

    public class StatisVoidSash : ModItemPatch
    {
        public override string ItemName => "StatisVoidSash";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase dodgeroll boost by 120%\nAutomatically dodgeroll on fatal damage but consumes all of stamina"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeBoost += 1.2f;
                pl.rollInstinct = true;
            }
        }
    }

    public class GrandGelatin : ModItemPatch
    {
        public override string ItemName => "GrandGelatin";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Using healing potion fully recover roll stamina"));
        }
    }

    public class IronBoots : ModItemPatch
    {
        public override string ItemName => "IronBoots";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase dodgeroll boost by 40% underwater"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.wet && player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeBoost += 0.4f;
            }
        }
    }

    public class YharimsGift : ModItemPatch
    {
        public override string ItemName => "YharimsGift";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "[c/F6AE2A:Perfect] dodgeroll will grant 'Precise' buff "));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer dp))
            {
                dp.yharimsGift = true;
            }
        }
    }
}
