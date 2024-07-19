using Terraria.UI;

namespace Dodgeroll.UI
{
    class DodgerollMeterUI : UIState
    {
        DodgerollMeter dodgerollMeter;

        public override void OnInitialize()
        {
            dodgerollMeter = new();
            Append(dodgerollMeter);
        }
    }
}