using GTA.UI;
using LemonUI.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberDriverMod
{
    static class Menus
    {

        private static string currentVersion = "1.0R";

        public static NativeMenu uberMenu;
        public static NativeMenu settingsMenu;
        public static NativeMenu advancedSettingsMenu;

        public static NativeItem cancelRideButton;

        public static bool debug;

        // ---------
        // UBER MENU
        // ---------

        public static void CreateUberMenu()
        {
            uberMenu = new NativeMenu("Uber Driver", "Enhanced driving system");

            NativeCheckboxItem onlineCheckbox = new NativeCheckboxItem("Online", "Online and accepting passengers!", false);
            cancelRideButton = new NativeItem("Cancel Ride", "Cancel your current ride.");
            cancelRideButton.Enabled = false;

            uberMenu.Add(onlineCheckbox);
            uberMenu.Add(cancelRideButton);
            uberMenu.AddSubMenu(settingsMenu);

            uberMenu.BannerText.Font = GTA.UI.Font.ChaletComprimeCologne;
            uberMenu.Banner.Color = Color.Purple;

            onlineCheckbox.CheckboxChanged += onlineCheckboxChanged;
            cancelRideButton.Activated += cancelRideButtonActivated;

            MainClass.pool.Add(uberMenu);
        }

        private static void onlineCheckboxChanged(object sender, EventArgs e)
        {
            NativeCheckboxItem checkbox = (NativeCheckboxItem)sender;
            bool isChecked = checkbox.Checked;

            Methods.ClearNotifications();

            if (isChecked)
            {
                Notification.Show("You are now ~g~online.");
                Methods.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
            }
            else
            {
                Notification.Show("You are now ~c~offline.");
                Methods.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
            }
        }

        public static void cancelRideButtonActivated(object sender, EventArgs e)
        {
            RideMission.CancelRide("cancel");
        }

        // -------------
        // SETTINGS MENU
        // -------------

        private static NativeListItem<string> notificationList;
        public static void CreateSettingsMenu()
        {
            settingsMenu = new NativeMenu("Settings", "Customize Your Experience");

            settingsMenu.BannerText.Font = GTA.UI.Font.ChaletComprimeCologne;
            settingsMenu.Banner.Color = Color.Purple;

            notificationList = new NativeListItem<string>("Notification Sound", "Sound that plays upon receiving a ride offer. Press ENTER to preview or confirm.", "Relaxed", "Pulse", "Ping", "Custom", "No Sound");
            NativeCheckboxItem noRideDelayCheckbox = new NativeCheckboxItem("No Ride Offer Delay", "Receive new ride offers immediately.", false);

            notificationList.Activated += notificationListSelected;
            noRideDelayCheckbox.CheckboxChanged += noRideDelayCheckboxChanged;

            settingsMenu.Add(notificationList);
            settingsMenu.Add(noRideDelayCheckbox);
            settingsMenu.AddSubMenu(advancedSettingsMenu);
            MainClass.pool.Add(settingsMenu);
        }

        private static void notificationListSelected(object sender, EventArgs e)
        {
            string selectedItem = notificationList.SelectedItem;
            string directory = "scripts/UberDriver/Sounds/";

            Methods.StopAllExternalSounds();

            if(selectedItem == "Custom" && !File.Exists(directory + selectedItem + ".wav"))
            {
                Methods.ReturnMessage("You don't have a custom sound set up! Please refer to README.txt for instructions!");
                return;
            } else if(selectedItem == "No Sound")
            {
            } else
            {
                Methods.PlayExternalSound(directory + selectedItem + ".wav");
            }

            MainClass.config.SetValue("Settings", "NotifSound", selectedItem);
            MainClass.config.Save();
        }

        private static void noRideDelayCheckboxChanged(object sender, EventArgs e)
        {
            NativeCheckboxItem checkbox = (NativeCheckboxItem)sender;
            bool isChecked = checkbox.Checked;

            Methods.ClearNotifications();
            MainClass.config.SetValue("Settings", "InstantRides", isChecked);
            MainClass.config.Save();
        }

        // ----------------------
        // ADVANCED SETTINGS MENU
        // ----------------------

        public static void CreateAdvancedSettingsMenu()
        {
            advancedSettingsMenu = new NativeMenu("Advanced Settings", "Advanced");

            advancedSettingsMenu.BannerText.Font = GTA.UI.Font.ChaletComprimeCologne;
            advancedSettingsMenu.Banner.Color = Color.Purple;

            NativeItem forceStartRide = new NativeItem("Force Start Ride", "Moved here since \"No Ride Offer Delay\" option exists. Normally used for development purposes.");
            NativeCheckboxItem debugCheckbox = new NativeCheckboxItem("Debug", "This will probably be useless unless you're a developer or bug reporting.", false);
            NativeItem getVersion = new NativeItem("Get Version", "Gets the version of the mod you are running.");

            advancedSettingsMenu.Add(forceStartRide);
            advancedSettingsMenu.Add(debugCheckbox);
            advancedSettingsMenu.Add(getVersion);

            debugCheckbox.CheckboxChanged += debugCheckboxChanged;
            forceStartRide.Activated += forceStartRideButtonActivated;
            getVersion.Activated += getVersionButtonActivated;

            MainClass.pool.Add(advancedSettingsMenu);
        }

        private static void debugCheckboxChanged(object sender, EventArgs e)
        {
            NativeCheckboxItem checkbox = (NativeCheckboxItem)sender;
            bool isChecked = checkbox.Checked;

            Methods.ClearNotifications();

            debug = isChecked;
        }

        public static void forceStartRideButtonActivated(object sender, EventArgs e)
        {
            if (RideMission.activeMission == true) return;
            RideMission.StartRide();
        }

        public static void getVersionButtonActivated(object sender, EventArgs e)
        {
            Methods.ReturnMessage("You are running version ~y~" + currentVersion + "~w~.");
        }
    }
}
