using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
using System.IO;
using GTA.UI;
using LemonUI;
using System.Windows.Forms;

namespace UberDriverMod
{
    public class MainClass : Script
    {
        public static ScriptSettings config;
        public static readonly ObjectPool pool = new ObjectPool();

        static int level = 0;

        // scenarios, hot areas, passengers greet you, passengers have a task while waiting (smoking, on phone, drunk), add celebrity clients, fix level up system
        // finish menu inputs, add controller support, fix peds looping back and forth when entering vehicles, add payouts, add proper payout prediction system (per mile), add appropriate
        // tipping system: vehicle quality, length of drive, add https version grabber from github to determine whether or not running version of mod is outdated

        // decide what to do when passenger flies out of vehicle

        public MainClass()
        {
            Tick += onTick;
            KeyDown += onKeyDown;

            Methods.SetupMod();
        }

        public void onTick(object sender, EventArgs e)
        {
            if (Methods.hasDirectory == false) return;

            pool.Process();

            RideMission.WaitForPassengerPickup();
            Methods.PedVehicleChecks();
            RideMission.WaitToArriveAtDestination();
            UberProfile.DrawUberProfile();
            Methods.DrawBigMessage();
            Methods.UpdateTimers();
            Methods.PedChecks(RideMission.passenger);
            
            if(Game.IsControlJustReleased(GTA.Control.PhoneOption)) UberProfile.draw = !UberProfile.draw;
            if (Game.IsControlJustReleased(GTA.Control.SelectCharacterFranklin)) openMenu();
            if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, config.GetValue("Controls", "MenuControl", 167)))
            {
                openMenu();
            }
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void openMenu()
        {
            if (config.GetValue("Saving", "CreatedProfile", false) == false)
            {
                string result = Methods.RegisterProfileInput();

                if (result == "invalid")
                {
                    Methods.ReturnMessage("Forbidden text! Please try again.", blink: true);
                    return;
                }

                if (result == "")
                {
                    Methods.ReturnMessage("Your name cannot be empty!", blink: true);
                    return;
                }

                config.SetValue("PROFILE", "Name", result);
                config.SetValue("SAVING", "CreatedProfile", true);
                config.Save();
            }

            Menus.settingsMenu.Visible = false; Menus.advancedSettingsMenu.Visible = false; Menus.uberMenu.Visible = true;
        }
    }
}
