using System.ComponentModel;
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
        [DefaultValue(0.75F)]
        public float InvulnerableRatio { get; set; }
    }
}