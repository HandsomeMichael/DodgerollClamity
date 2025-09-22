using Terraria.DataStructures;

namespace DodgerollClamity
{
    public static class Helpme
    {
        // public static bool IsAny(this object value, params object[] param)
        // {
        //     for (int i = 0; i < param.Length; i++)
        //     {
        //         if (value == param[i]) return true;
        //     }
        //     return false;
        // }

        public static bool IsAny<T>(this T value, params T[] param)
        {
            for (int i = 0; i < param.Length; i++)
            {
                if (value.Equals(param[i])) return true;
            }
            return false;
        }

        public static bool IsOther(this PlayerDeathReason reason, int deathReasonOtherID, bool checkOtherSource = true)
        {
            if (checkOtherSource && (reason.SourceItem != null || reason.SourceNPCIndex > -1 || reason.SourceProjectileLocalIndex > -1 || reason.SourceProjectileType > 0 || reason.SourcePlayerIndex > -1)) return false;
            return reason.SourceOtherIndex == deathReasonOtherID;
        }
    }

    public static class DeathReasonOtherID
    {
        public const int None = -1;
        public const int FallDamage = 0;
        public const int Drowned = 1;
        public const int Lava = 2;
        public const int TileTouch = 3;
        public const int Suffocate = 7;
        public const int IdkWhatThisIs = 8;
        public const int PoisonOrVenom = 9;
        public const int Electrified = 10;
        public const int WallOfFleshThing = 11;
        public const int WOFTongued = 12;
        public const int InfernoPotion = 16;
        public const int Starved = 18;
    }
}