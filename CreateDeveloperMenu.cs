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
using static Uber_Driver_Re_Written.CreateMenu;

namespace Uber_Driver_Re_Written
{
    static class CreateDeveloperMenu
    {
        //Create dev menu
        public static NativeMenu devMenu = new NativeMenu("Uber Driver Debug", "Fulfilling your driving needs!");

        //Show ride scenario
        public static NativeCheckboxItem showRideScenario = new NativeCheckboxItem("Display Ride Scenario", "Whether or not to display the ride scenario.", true);

        //Force next ride scenario list
        public static NativeListItem<string> forceScenarioList = new NativeListItem<string>("Force Ride Scenario", "What scenario the next ride will be.", "Normal", "Drunk", "Celebrity", "Robbery");

        //Force next ride scenario checkbox
        public static NativeCheckboxItem forceScenarioBox = new NativeCheckboxItem("Force Ride Scenario", "Whether or not to force the next ride as the selected scenario.", false);

    }
}
