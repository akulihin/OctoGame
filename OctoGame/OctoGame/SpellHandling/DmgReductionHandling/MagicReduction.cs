
using System.Threading.Tasks;

namespace OctoGame.OctoGame.SpellHandling.DmgReductionHandling
{
    public sealed class MagicReduction : IServiceSingleton
    {  
        public Task InitializeAsync()
            => Task.CompletedTask;

        public double ResistHandling(double magPen, double magResist, double dmg)
        {
            double def = 0;
            if (magResist - magPen == 1)
                def = 0.26;
            else if (magResist - magPen == 2)
                def = 0.13;
            else if (magResist - magPen == 3)
                def = 0.8;
            else if (magResist - magPen == 4)
                def = 0.5;
            else if (magResist - magPen == 5)
                def = 0.3;
            else if (magResist - magPen == 6)
                def = 0.1;

            var final = dmg - dmg * def;

            return final;
        }
    }
}
