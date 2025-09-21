using Terraria.DataStructures;

namespace DodgerollClamity
{
    public static class Helpme
    {
        public static bool IsOther(this PlayerDeathReason reason, int deathReasonOtherID)
        {
            return reason.SourceOtherIndex == deathReasonOtherID;
        }
    }

    public static class DeathReasonOtherID
    {
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