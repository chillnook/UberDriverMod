using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberDriverMod
{
    static class RankManager
    {
        enum RideOutcome
        {
            Completed = 1,
            Cancelled = 0
        }

        public static void CalculateRank(int level, float affectingXp = 0.3f, float requiredXp = 2)
        {
            int XP = (int)Math.Pow((double)level / affectingXp, requiredXp);
            int Level = (int)((double)affectingXp * Math.Sqrt(XP));

            Methods.ReturnDebugMessage("Level: " + level.ToString() + " XP: " + XP.ToString());
        }
    }
}
