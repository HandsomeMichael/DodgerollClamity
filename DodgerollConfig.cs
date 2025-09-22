using System.ComponentModel;
using DodgerollClamity.UI;
using Terraria.ModLoader.Config;

namespace DodgerollClamity
{
    public class DodgerollConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static DodgerollConfig Instance;

        [DefaultValue(true)]
        public bool EnableDodgeroll { get; set; }

        // will always be fully invulnerable

        // [Slider]
        // [DefaultValue(1f)]
        // public float InvulnerableRatio { get; set; }

        [Slider]
        [DefaultValue(25)]
        public int DodgerollLength { get; set; }

        [Slider]
        [DefaultValue(7)]
        public int DodgerollBoost { get; set; }

        [DefaultValue(true)]
        public bool EnableItemUseMidroll { get; set; }

        [DefaultValue(true)]
        public bool CancelItemUseMidroll { get; set; }

        [DefaultValue(false)]
        public bool PerfectRollTriggerHurtEffect { get; set; }

        [DefaultValue(false)]
        public bool BonusOnDodgeNonsense { get; set; }

        [DefaultValue(false)]
        public bool DashRequireStamina { get; set; }

        [Header("Stamina")]

        // will always be enabled

        // [DefaultValue(true)]
        // public bool EnableStamina { get; set; }

        [Slider]
        [DefaultValue(0.5)]
        public float StaminaUsage { get; set; }

        [Slider]
        [DefaultValue(0.4)]
        public float StaminaRegenRate { get; set; }

        [Slider]
        [Range(0f, 10f)]
        [DefaultValue(1.3f)]
        public float StaminaCooldown { get; set; }

        [DefaultValue(DodgerollMeterPosition.TOP)]
        public DodgerollMeterPosition StaminaPosition { get; set; }

        [Slider]
        [DefaultValue(15)]
        public int StaminaPositionOffset { get; set; }
        
        [Slider]
        [Range(0, 100)]
        [DefaultValue(50)]
        public int StaminaBarOpacity { get; set; }
    }
}