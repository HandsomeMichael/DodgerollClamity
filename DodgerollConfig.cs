using System.ComponentModel;
using Dodgeroll.UI;
using Terraria.ModLoader.Config;

namespace Dodgeroll
{
    public class DodgerollConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static DodgerollConfig Instance;

        [DefaultValue(true)]
        public bool EnableDodgeroll { get; set; }

        [Slider]
        [DefaultValue(0.75)]
        public float InvulnerableRatio { get; set; }

        [Slider]
        [DefaultValue(30)]
        public int DodgerollLength { get; set; }

        [Slider]
        [DefaultValue(7)]
        public int DodgerollBoost { get; set; }

        [Header("Stamina")]

        [DefaultValue(true)]
        public bool EnableStamina { get; set; }

        [Slider]
        [DefaultValue(0.2)]
        public float StaminaUsage { get; set; }

        [Slider]
        [DefaultValue(0.5)]
        public float StaminaRegenRate { get; set; }

        [Slider]
        [DefaultValue(1)]
        public float StaminaCooldown { get; set; }

        [DefaultValue(DodgerollMeterPosition.TOP)]
        public DodgerollMeterPosition StaminaPosition { get; set; }

        [Slider]
        [DefaultValue(15)]
        public int StaminaPositionOffset { get; set; }
    }
}