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

namespace Uber_Driver_Re_Written
{
    public class Class1 : Script
    {

        //Load ini file
        public static ScriptSettings config = ScriptSettings.Load("scripts\\UberDriver.ini");

        //Set player
        private static Player player = Game.Player;

        //Bools
        public static bool alreadySet;

        public Class1()
        {

            //If UberDriver.dll exists, delete it
            try { File.Delete("scripts//UberDriverini"); } catch(Exception e) { }

            Tick += onTick;
            KeyDown += onKeyDown;

            //Create all menus (check CreateMenu.cs for more information)
            CreateMenu.CreateAllMenus();
            UberMission.CreateRobberyProgressBar();

            //Create ini file
            GenericMethods.CreateIniFile();

            //Create drop-off lists
            AvailableDropoffs.CreateCityList();
            AvailableDropoffs.CreateCountryList();

            //Create celebrity list
            RandomLists.CreateCelebrityList();

            //Set values to their saved state
            CreateMenu.SetSavedValues();

            //Set default functions
            SetDefaultFunctions();
        }

        public static void onTick(object sender, EventArgs e)
        {

            if (RideTimer.stealRide == true)
            {
                try
                {
                    Game.Player.Character.CurrentVehicle.Speed = 0f;
                    Game.Player.Character.CurrentVehicle.IsEngineRunning = false;
                } catch { }

                if(CreateMenu.debugItem.Checked)
                {
                    GTA.UI.Screen.ShowHelpText("stealride");
                }

                UberMission.ShootAtPlayer();

                Wait(5000);

                Game.Player.Character.Health = 0;

                Wait(6000);

                UberMission.RideComplete();

                RideTimer.stealRide = false;
            }

            if (UberMission.ActiveRide == true && UberMission.rideScenario == "Robbery" && UberMission.robberyScenarioEntered == true)
            {
                try
                {   
                    Game.Player.Character.CurrentVehicle.Speed = 0f;
                    Game.Player.Character.CurrentVehicle.IsEngineRunning = false;
                } catch { }
            }

            //Update Pool
            CreateMenu.pool.Process();

            if(UberMission.spaceMashEnabled == true)
            {
                try { UberMission.robberyProgressBarBG.Draw(); } catch { }
                try { UberMission.robberyProgressBar.Draw(); } catch { }
                try { UberMission.robberyProgressPreview.Draw(); } catch { }
            }       

            try
            {
                //If vehicle is destroyed cancel ride
                if (UberMission.missionVehicle.IsConsideredDestroyed == true && UberMission.ActiveRide == true)
                {
                    UberMission.rideCancelled = true;
                    UberMission.RideComplete();
                }
            } catch { }            

            if (!Game.Player.Character.IsInVehicle() && UberMission.ActiveRide == true)
            {
                GTA.UI.Screen.ShowSubtitle("Return to the ~b~vehicle.");
                UberMission.CreateVehicleBlip();
                alreadySet = false;
                
            } else
            {
                if (alreadySet == true)
                {
                    //nothing, cant return will stop ontick
                } else
                {
                    //Resume last event                
                    try
                    {
                        //Player not at ped
                        if (Game.Player.Character.Position.DistanceTo(UberMission.Passenger.Position) > 7f)
                        {
                            GTA.UI.Screen.ShowSubtitle("Drive to the ~b~passenger.", int.MaxValue);
                        }

                        //Destination event
                        if (UberMission.ActiveRide == true && UberMission.Passenger.IsInVehicle(player.Character.CurrentVehicle) == true && UberMission.Passenger.IsInVehicle(player.Character.CurrentVehicle))
                        {
                            GTA.UI.Screen.ShowSubtitle("Drive the ~b~passenger ~w~to ~y~" + AvailableDropoffs.DropName + ".", int.MaxValue);
                        }
                    }
                    catch { }

                    //Delete blip
                    try { UberMission.vehicleBlip.Delete(); UberMission.vehicleBlipCreated = false; } catch { }

                    //Set already set to true
                    alreadySet = true;
                }               
               
            }

            //If player is not in vehicle, wait
            if (!Game.Player.Character.IsInVehicle() && UberMission.ActiveRide == true) return;

            if (UberMission.ActiveRide == true && UberMission.Passenger.IsInVehicle(player.Character.CurrentVehicle) == true && UberMission.Passenger.IsInVehicle(player.Character.CurrentVehicle))
            {
                //Once passenger has entered, set drop off, delete their blip
                try
                {
                    UberMission.passengerBlip.Delete();
                }
                catch (Exception error)
                {
                    GenericMethods.ErrorMessage(error.ToString());
                }

                //Set drop off is in PedEnterVehicle      
                UberMission.SetDestination();
            } else
            {
                //Once player is near ped, wait until ped is in vehicle
                if (UberMission.isInVehicle == true && UberMission.ActiveRide == true && UberMission.passengerCheck == true && !UberMission.Passenger.IsInVehicle(player.Character.CurrentVehicle))
                {

                    if (player.Character.CurrentVehicle.Position.DistanceTo(UberMission.Passenger.Position) <= 6.5f)
                    {
                        //Give player notice
                        GTA.UI.Screen.ShowSubtitle("Wait for the ~b~passenger.", int.MaxValue);

                        //Make passenger enter vehicle
                        UberMission.PedEnterVehicle();

                        //Keep player engine off
                        player.Character.CurrentVehicle.IsEngineRunning = false;
                    }

                }
            }

            if(UberMission.ActiveRide == true && UberMission.setDestination == true)
            {
                //Wait until player reaches destination
                if(player.Character.Position.DistanceTo(UberMission.destinationBlip.Position) <= 4f)
                {
                    UberMission.CalculateInts();
                }
            }

            if(RideTimer.notificationShown == false && RideTimer.rideOffered == true)
            {
                UberMission.CreateOffer();
                RideTimer.notificationShown = true;
                return;
            }
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            //Open menu
            if (e.KeyCode == config.GetValue("Settings", "MenuKey", Keys.F6)) { CreateMenu.uberMenu.Visible = !CreateMenu.uberMenu.Visible; }

            //Open dev menu
            if (e.KeyCode == config.GetValue("Settings", "MenuKey", Keys.F10))
            {
                if(!CreateMenu.debugItem.Checked) { return; }
                CreateDeveloperMenu.devMenu.Visible = !CreateDeveloperMenu.devMenu.Visible; 
            }

            if (e.KeyCode == config.GetValue("Settings", "AcceptKey", Keys.E) && RideTimer.rideOffered == true)
            {
                //Show message
                if(UberMission.rideScenario == "Celebrity")
                {
                    GTA.UI.Screen.ShowHelpText("You accepted a high clientele ride. Drive carefully or risk the ride being cancelled.", 8000, true);
                }

                if(UberMission.rideScenario == "Drunk")
                {

                }

                RideTimer.rideWaitTimer.Enabled = false;
                RideTimer.rideWaitTimer.Stop();
                RideTimer.rideOffered = false;
                UberMission.JobCheck();
                return;
            }

            if (e.KeyCode == config.GetValue("Settings", "DeclineKey", Keys.T) && RideTimer.rideOffered == true)
            {                
                RideTimer.rideOffered = false;
                RideTimer.rideWaitTimer.Enabled = false;
                RideTimer.rideWaitTimer.Stop();
                RideTimer.StartTimer();

                for (int i = 0; i < 100; i++)
                {
                    Notification.Hide(i);
                }

                return;
            }       
            
            if(e.KeyCode == Keys.Space && UberMission.spaceMashEnabled == true)
            {
                UberMission.robberyProgressBar.Size = new System.Drawing.SizeF(CreateMenu.percent(30, UberMission.progress += 3), 8f);
                
                if(UberMission.progress >= 57)
                {
                    UberMission.Passenger.Task.ClearAllImmediately();
                    UberMission.Passenger.Task.RunTo(Vector3.RandomXYZ());
                    RideTimer.mashTimer.Stop();
                    Script.Wait(5000);
                    UberMission.RideComplete();
                }
            }
        }

        private void SetDefaultFunctions()
        {
            if(CreateMenu.acceptingRidesItem.Checked == true)
            {
                RideTimer.StartTimer();
            }
        }
    }
}
