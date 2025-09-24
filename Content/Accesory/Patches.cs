using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
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

    public class DodgeCD : ModItemPatch
    {
        public virtual float CDMult => 0f;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodgeCD", $"Reduces dogeroll regen cooldown by {CDMult*100f}%"));
        }
        public override void UpdateEquip(Item item, Player player)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeCD += CDMult;
            }
        }
    }

    public class MasterNinjaGear1 : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.AnkhShield || entity.type == ItemID.Tabi || entity.type == ItemID.ClimbingClaws || entity.type == ItemID.ShoeSpikes;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase dodgeroll boost by 30%"));
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeBoost += 0.3f;
            }
        }
    }

    public class AnkhShieldNoClam : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModLoader.HasMod("CalamityMod");
        }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.AnkhShield;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Automatically dodgeroll on fatal damage but consumes all of stamina"));
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.rollInstinct = true;
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

    public class TheAmalgam : ModItemPatch
    {
        public override string ItemName => "TheAmalgam";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase dodgeroll boost by 200% when hurt\nIncrease stamina regen when hurt\nReduces dodgeroll stamina usage by 10%"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                if (player.immuneTime > 0)
                {
                    pl.statDodgeBoost += 2f;
                    pl.statDodgeRegen += 0.2f; // 20% per second
                }
                pl.statDodgeStaminaUsage -= 0.1f;
            }
        }
    }

    public class WarbanneroftheSun : ModItemPatch
    {
        public override string ItemName => "WarbanneroftheSun";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase melee speed by 50% on [c/F6AE2A:perfect] dodgeroll"));
        }
    }

    public class DaawnlightSpiritOrigin : ModItemPatch
    {
        public override string ItemName => "DaawnlightSpiritOrigin";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase ranged attack speed by 50% on [c/F6AE2A:perfect] dodgeroll"));
        }
    }

    public class TheCamper : ModItemPatch
    {
        public override string ItemName => "TheCamper";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase stamina regen by 4/s"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeRegen += 4;
            }
        }
    }

    public class TheAbsorber : ModItemPatch
    {
        public override string ItemName => "TheAbsorber";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "[c/F6AE2A:Perfect] dodgeroll will absorb 5 second of curable debuff"));
        }
    }

    public class AsgardsValor : ModItemPatch
    {
        public override string ItemName => "AsgardsValor";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Reduces dodgeroll regen cooldown by 20%"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeCD += 0.2f;
            }
        }
    }

    public class DeepDiver : ModItemPatch
    {
        public override string ItemName => "DeepDiver";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Reduces dodgeroll regen cooldown by 10%"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeCD += 0.1f;
            }
        }
    }

    public class OrnateShield : ModItemPatch
    {
        public override string ItemName => "OrnateShield";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Reduces dodgeroll regen cooldown by 10%"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeCD += 0.1f;
            }
        }
    }

    public class AsgardianAegis : ModItemPatch
    {
        public override string ItemName => "AsgardianAegis";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Reduces dodgeroll regen cooldown by 35%"));
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out DodgerollPlayer pl))
            {
                pl.statDodgeCD += 0.35f;
            }
        }
    }

    public class CounterScarf : ModItemPatch
    {
        public override string ItemName => "CounterScarf";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase damage by 10% on [c/F6AE2A:perfect] dodgeroll "));
        }
    }

    public class EvasionScarf : ModItemPatch
    {
        public override string ItemName => "EvasionScarf";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "Increase damage by 15% on [c/F6AE2A:perfect] dodgeroll\n'Worn by manly and badass hero'"));
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

    public class RoverDrive : ModItemPatch
    {
        public override string ItemName => "RoverDrive";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "dodge", "[c/F6AE2A:Perfect] dodgeroll brings back 5 durability"));
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
