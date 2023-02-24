using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
using LemonUI;
using LemonUI.Menus;
using System.Windows.Forms;
using System.Net;
using System.Drawing;
using System.Xml.Linq;
using System.Security;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Web;
using static Uber_Driver_Re_Written.CreateDeveloperMenu;

namespace Uber_Driver_Re_Written
{
    class CreateMenu
    {
        //Version variables
        public static string versionString;
        public static string currentVersion = "0.4a";
        public static int version;

        //Make pool accessible by all classes
        public static ObjectPool pool = new ObjectPool();

        //Create uber menu (main)
        public static NativeMenu uberMenu = new NativeMenu("Uber Driver", "Fulfilling your driving needs!");

        //Accepting rides checkbox
        public static NativeCheckboxItem acceptingRidesItem = new NativeCheckboxItem("Accepting Rides", "Whether or not you're accepting rides.", false);

        //Instant rides checkbox
        public static NativeCheckboxItem instantRidesItem = new NativeCheckboxItem("Instant Rides", "Whether or not you want rides to instantly come-in.", false);

        //Create stop job button
        public static NativeItem cancelRideItem = new NativeItem("Cancel Ride", "Cancel an ongoing ride.");

        //Create settings menu
        public static NativeMenu settingsMenu = new NativeMenu("Settings", "Settings", "Mod customization.");
        //Debug
        public static NativeCheckboxItem debugItem = new NativeCheckboxItem("Debug", "Debug option.", false);

        //Create list of possible drop-offs (city, country)
        public static NativeListItem<string> dropOffList = new NativeListItem<string>("Drop-off Location", "Where you want drop-off locations to be.", "City", "Country");

        //Notif sound checkbox
        public static NativeCheckboxItem notifSoundItem = new NativeCheckboxItem("Notification Sound", "Whether or not you want the ride notification to play.", true);

        //Create start job button
        public static NativeItem startRideItem = new NativeItem("Start Ride", "Force start a ride.");

        //Menu values
        public static string dropOff = "City";

        //Load ini file
        public static ScriptSettings config = ScriptSettings.Load("scripts\\UberDriver.ini");

        public static void CreateAllMenus()
        {
            //Allow connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //Make new web client
            HttpWebRequest webClient = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/chillnook/UberDriverInformation/main/Version.md?" + DateTime.Now.Ticks);
            webClient.Method = "GET";
            webClient.KeepAlive = false;
            HttpWebResponse Response = (HttpWebResponse)webClient.GetResponse();
            WebHeaderCollection header = Response.Headers;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(Response.GetResponseStream(), encoding))
            {
                versionString = reader.ReadToEnd().Trim().ToLower();
            }
            webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

            //Add menus to pool
            pool.Add(uberMenu);
            pool.Add(settingsMenu);
            pool.Add(devMenu);

            //Add items to uberMenu
            uberMenu.Add(acceptingRidesItem);
            uberMenu.Add(instantRidesItem);
            uberMenu.Add(cancelRideItem);
            uberMenu.AddSubMenu(settingsMenu);

            //Add items to settingsMenu
            settingsMenu.Add(dropOffList);
            settingsMenu.Add(notifSoundItem);
            settingsMenu.Add(startRideItem);
            settingsMenu.Add(debugItem);

            //Add items to devMenu
            devMenu.Add(showRideScenario);
            devMenu.Add(forceScenarioBox);
            devMenu.Add(forceScenarioList);

            //Check version also sets settings subtitle
            VersionCheck();

            //Set item methods
            acceptingRidesItem.CheckboxChanged += acceptingRidesCheckboxChanged;
            instantRidesItem.CheckboxChanged += instantRidesCheckboxChanged;
            cancelRideItem.Activated += cancelRideItemActivated;

            dropOffList.ItemChanged += dropOffListChanged;
            notifSoundItem.Activated += notifSoundItemActivated;
            startRideItem.Activated += startRideItemActivated;

        }

        public static void acceptingRidesCheckboxChanged(object sender, EventArgs e)
        {
            if(acceptingRidesItem.Checked)
            {
                config.SetValue("Settings", "ActiveRides", true);
                config.Save();
                RideTimer.StartTimer();
            } else { config.SetValue("Settings", "ActiveRides", false); RideTimer.rideWaitTimer.Enabled = false; config.Save(); RideTimer.rideWaitTimer.Stop(); }
        }

        public static void instantRidesCheckboxChanged(object sender, EventArgs e)
        {
            if (instantRidesItem.Checked)
            {
                config.SetValue("Settings", "InstantRides", true);
                config.Save();
            }
            else { config.SetValue("Settings", "InstantRides", false); config.Save(); }
        }

        public static void cancelRideItemActivated(object sender, EventArgs e)
        {

            //Set cancelled to true
            UberMission.rideCancelled = true;

            //Cancel ride
            UberMission.RideComplete();
        }
        public static void dropOffListChanged(object sender, ItemChangedEventArgs<string> e)
        {
            //Drop off string is equal to the selected item
            dropOff = dropOffList.SelectedItem;
        }

        public static void notifSoundItemActivated(object sender, EventArgs e)
        {
            if(notifSoundItem.Checked)
            {
                config.SetValue("Settings", "NotifSound", true);
                config.Save();
            } else { config.SetValue("Settings", "NotifSound", false); config.Save(); }
        }

        public static void startRideItemActivated(object sender, EventArgs e)
        {
            //Start job
            UberMission.JobCheck();
        }
        public static void debugItemActivated(object sender, EventArgs e)
        {
            //Debug item
            if(debugItem.Checked)
            {

            } else
            {

            }
        }

        public static void VersionCheck()
        {
            if(currentVersion != versionString.Trim()) {
                //Mod is up to outdated, make version text red
                settingsMenu.Subtitle = "~r~OUTDATED " + currentVersion.ToLower();
                GTA.UI.Notification.Show("Your Uber Driver build is outdated. Please update." + "~n~Current version: ~r~" + currentVersion + "~n~~w~Newest version: ~g~" + versionString);
            } else
            {
                //Mod is up to date, make version text green
                settingsMenu.Subtitle = "~g~" + currentVersion.ToLower();
            }

        }

        //Set values to their saved state
        public static void SetSavedValues()
        {
            //If active rides is set to true in the .ini, ensure the menu shows it
            if(config.GetValue("Settings", "ActiveRides", true))
            {
                acceptingRidesItem.Checked = true;
            } else { acceptingRidesItem.Checked = false; }

            //If instant rides is set to true in the .ini, ensure the menu shows it
            if (config.GetValue("Settings", "InstantRides", true))
            {
                instantRidesItem.Checked = true;
            } else { instantRidesItem.Checked = false; }

            if (config.GetValue("Settings", "NotifSound", true))
            {
                notifSoundItem.Checked = true;
            }
            else { notifSoundItem.Checked = false; }
        }

        public static int percent(double total, double current) => (int)(float)(current / total * 100.0);
    }
}
