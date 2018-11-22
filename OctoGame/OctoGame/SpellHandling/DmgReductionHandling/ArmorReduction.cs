using System;

namespace OctoGame.OctoGame.SpellHandling.DmgReductionHandling
{
    public class ArmorReduction
    {
        public double ArmorHandling(double armPen, double arm, double dmg)
        {
            // TEST IT, maybe an error!!! ( unity test helps)
            double def = 0;
            if (Math.Ceiling(arm - armPen) == 1)
                def = 0.26;
            else if (Math.Ceiling(arm - armPen) == 2)
                def = 0.13;
            else if (Math.Ceiling(arm - armPen) == 3)
                def = 0.8;
            else if (Math.Ceiling(arm - armPen) == 4)
                def = 0.5;
            else if (Math.Ceiling(arm - armPen) == 5)
                def = 0.3;
            else if (Math.Ceiling(arm - armPen) == 6)
                def = 0.1;

            var final = dmg - dmg * def;

            return final;
        }
    }
}
