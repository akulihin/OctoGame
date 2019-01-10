using System;
using System.Threading.Tasks;

namespace OctoGame.OctoGame.SpellHandling.DmgReductionHandling
{
    public sealed class ArmorReduction : IServiceSingleton
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        public double ArmorHandling(double armPen, double arm, double dmg)
        {
            // TEST IT, maybe an error!!! ( unity test helps)
           
            var switchVar = Convert.ToInt32(Math.Ceiling(arm - armPen));
            if (switchVar > 6) switchVar = 6;

            return dmg - dmg * GetArmorPercentDependsOnLvl(switchVar);
        }


        public double GetArmorPercentDependsOnLvl(double armorLvl)
        {
            double armorPercent = 0;

            switch (armorLvl)

            {
                case 1:
                    armorPercent = 0.26;
                    break;
                case 2:
                    armorPercent = 0.39;
                    break;
                case 3:
                    armorPercent = 0.47;
                    break;
                case 4:
                    armorPercent = 0.52;
                    break;
                case 5:
                    armorPercent = 0.55;
                    break;
                case 6:
                    armorPercent = 0.56;
                    break;
            }


            return armorPercent;
        }
    }
}