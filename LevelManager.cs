using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.UI;

namespace Uber_Driver_Re_Written
{
    class LevelManager
    {

        //Load ini file
        public static ScriptSettings config = ScriptSettings.Load("scripts\\UberDriver.ini");

        private static bool levelUp;

        public static void LevelFunctions()
        {
            GTA.UI.Screen.ShowSubtitle(config.GetValue("Stats", "Completed", 0).ToString() + " first");
            //Add completed ride
            config.SetValue("Stats", "Completed", config.GetValue("Stats", "Completed", 0) + 1);
            config.Save();
            GTA.UI.Screen.ShowHelpText(config.GetValue("Stats", "Completed", 0).ToString() + " second");

            //Check if player is ready to levelup
            if (config.GetValue("Stats", "Completed", 0) >= config.GetValue("Stats", "Goal", 2))
            {
                LevelUp();
            } else
            {
                //Otherwise, show level progress
                Notification.Show("Level: " + config.GetValue("Stats", "Level", 1) + "~n~Progress until next level: " + config.GetValue("Stats", "Completed", 0) + "/" + config.GetValue("Stats", "Goal", 2));
            }            
        }

        public static void LevelUp()
        {
            //Set new goal
            config.SetValue("Stats", "Goal", config.GetValue("Stats", "Goal", 0) * 2);
            config.Save();

            //Set new level
            config.SetValue("Stats", "Level", config.GetValue("Stats", "Level", 1) + 1);
            config.Save();

            //Level up function
            Notification.Show("Level up!" + " You are now level " + config.GetValue("Stats", "Level", 1) + "!");

            //Check for rewards
            Rewards();
        }

        public static void Rewards()
        {
            int Level = config.GetValue("Stats", "Level", 1);
            if (Level == 2)
            {

            }

            if(Level == 3)
            {

            }

            if(Level == 4)
            {
                //High cliente rides
                RewardMessage("High clientele rides now unlocked.");
                Screen.ShowHelpText("You have unlocked high clientele rides. These will be chosen at random when accepting rides.", 10000, true);
            }
        }

        public static void RewardMessage(string content)
        {
            Notification.Show(NotificationIcon.SocialClub, "Level Rewards", "Unlock", content);
        }
    }
}
