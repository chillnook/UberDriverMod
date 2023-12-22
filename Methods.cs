using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.UI;
using GTA.Math;
using GTA.Native;
using System.Media;
using System.Text.RegularExpressions;
using LemonUI.Scaleform;
using System.Windows.Forms.VisualStyles;

namespace UberDriverMod
{
    static class Methods
    {
        public static bool hasDirectory = false;
        static SoundPlayer soundPlayer;
        static VariableTimer myTimer;

        public static void SetupMod()
        {
            string directoryPath = "scripts/UberDriver";
            if(!Directory.Exists(directoryPath))
            {
                hasDirectory = false;
                Methods.ReturnErrorMessage("No UberDriver directory detected! Please ensure you've installed the mod correctly. The mod will not run.");
                return;
            }
            else
            {
                hasDirectory = true;
            }

            CreateConfigFile();

            MainClass.config = ScriptSettings.Load("scripts\\UberDriver\\config.ini");

            DestinationLists.CreateCityList();
            DestinationLists.CreateCountryList();

            PassengerPickupLists.CreateCityList();

            Menus.CreateAdvancedSettingsMenu();
            Menus.CreateSettingsMenu();
            Menus.CreateUberMenu();

            UberProfile.CreateUberProfile();
        }

        public static void CreateConfigFile()
        {
            string directoryName = "scripts//UberDriver//";
            string fileName = Path.Combine(directoryName, "config.ini");

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            if (!File.Exists(fileName))
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine("[CONTROLS]");
                    sw.WriteLine("MenuControl = 167");
                    sw.WriteLine("The MenuKey option has been changed to allow for controller input. Controls for MenuControls can be found at https://docs.fivem.net/docs/game-references/controls/. Default is 167 (F6).");
                    sw.WriteLine("AcceptKey = B");
                    sw.WriteLine("DeclineKey = N");

                    sw.WriteLine(" ");

                    sw.WriteLine("[SETTINGS]");
                    sw.WriteLine("InstantRides = false");
                    sw.WriteLine("NotifSound = Relaxed");
                    sw.WriteLine("CreatedProfile = false");

                    sw.WriteLine(" ");

                    sw.WriteLine("[STATS]");
                    sw.WriteLine("Level = 1");
                    sw.WriteLine("Goal = 2");

                    sw.WriteLine("");

                    sw.WriteLine("[SAVING]");
                    sw.WriteLine("CreatedProfile = false");

                    sw.WriteLine(" ");

                    sw.WriteLine("[PROFILE]");
                    sw.WriteLine("Name = Player0");
                    sw.WriteLine("Trips = 0");
                    sw.WriteLine("Rating = 5");
                    sw.WriteLine("StartDate = 0");
                }
            }
        }

        /// <summary>
        /// Return a generic message.
        /// </summary>
        /// <param name="content">The content<see cref="string"/>.</param>
        public static void ReturnMessage(string content, bool clearNotifications = true, bool blink = false)
        {
            if(clearNotifications == true)
            {
                ClearNotifications();
            }
            
            Notification.Show("~p~Uber Driver message: \n~w~" + content, blink);
        }

        /// <summary>
        /// Return a debug message.
        /// </summary>
        /// <param name="content">The content<see cref="string"/>.</param>
        public static void ReturnDebugMessage(string content)
        {
            //ClearNotifications();
            if(Menus.debug == true)
            {
                Notification.Show("~y~Uber Driver debug message: \n~w~" + content, false);
            }
        }

        /// <summary>
        /// Return an error message.
        /// </summary>
        /// <param name="content">The content<see cref="string"/>.</param>
        public static void ReturnErrorMessage(string content)
        {
            ClearNotifications();
            Notification.Show("~r~Uber Driver error message: \n~w~" + content, false);
        }

        /// <summary>
        /// Clear on-screen notifications.
        /// </summary>
        /// <param name="loopUntilInt">The loopUntilInt<see cref="int"/>.</param>
        public static void ClearNotifications(int loopUntilInt = 100)
        {
            //loop through and clear notifications on screen, default clear 100 on screen
            for (int i = 0; i < loopUntilInt; i++)
            {
                Notification.Hide(i);
            }
        }

        public static string RegisterProfileInput(int i = 0)
        {
            if(i == 1) {
                Function.Call(Hash.DISPLAY_ONSCREEN_KEYBOARD, 0, "FMMC_KEY_TIP12F", "", "Profile Name", "", "", "", 20);
            } else
            {
                Function.Call(Hash.DISPLAY_ONSCREEN_KEYBOARD, 0, "FMMC_KEY_TIP8S", "", "Profile Name", "", "", "", 20);
            }

            while (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) == 0)
            {
                Script.Wait(0);
            }
            
            string result = Function.Call<string>(Hash.GET_ONSCREEN_KEYBOARD_RESULT);

            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
            foreach(var character in specialChar)
            {
                if (result.Contains(character))
                {
                    return "invalid";
                }
            }

            if(string.IsNullOrWhiteSpace(result))
            {
                return "";
            }

            MainClass.config.SetValue("Profile", "Name", result);
            MainClass.config.Save();
            return result;
        }

        /// <summary>
        /// Play an internal game sound.
        /// </summary>
        /// <param name="soundName">The soundName<see cref="string"/>.</param>
        /// <param name="soundGroup">The soundGroup<see cref="string"/>.</param>
        public static void PlaySound(string soundName, string soundGroup)
        {
            int sound;

            sound = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FRONTEND, sound, soundName, soundGroup);
            Function.Call(Hash.RELEASE_SOUND_ID, sound);
        }

        public static void PlayExternalSound(string directory)
        {
            soundPlayer = new SoundPlayer(directory);

            soundPlayer.Load();
            soundPlayer.Play();

            soundPlayer.Dispose();
        }

        public static void StopAllExternalSounds()
        {
            try
            {
                soundPlayer.Stop();
            } catch { }
        }

        /// <summary>
        /// Play an external sound.
        /// </summary>
        /// <param name="soundName">The soundName<see cref="string"/>.</param>
        /// <param name="path">The path<see cref="string"/>.</param>
        public static void PlayExternalSound(string soundName, string path)
        {
            SoundPlayer player = new SoundPlayer(@path);
            player.Play();
        }

        /// <summary>
        /// Returns whether or not player is in a valid vehicle.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsPlayerInVehicle()
        {
            if (Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Exists() && Game.Player.Character.CurrentVehicle.IsAlive && Game.Player.Character.IsAlive)
            {
                ClearNotifications();
                return true;
            }
            else
            {
                ReturnDebugMessage("You are not in a valid vehicle!");
                return false;
            }
        }

        /// <summary>
        /// Returns whether or not player is the driver of a vehicle.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsPlayerDriver()
        {
            if (IsPlayerInVehicle() && Game.Player.Character.SeatIndex == VehicleSeat.Driver)
            {
                ClearNotifications();
                return true;
            }
            else
            {
                ReturnDebugMessage("You are not the driver of this vehicle!");
                return false;
            }
        }

        /// <summary>
        /// Check if vehicle has an available seat.
        /// </summary>
        /// <param name="vehicle">The vehicle<see cref="Vehicle"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool VehicleHasAvailableSeat(Vehicle vehicle)
        {

            if (IsPlayerInVehicle() == false)
            {
                return false;
            }

            List<VehicleSeat> seats = new List<VehicleSeat>() { VehicleSeat.LeftFront, VehicleSeat.LeftRear, VehicleSeat.RightFront, VehicleSeat.RightRear };

            List<VehicleSeat> availableSeats = new List<VehicleSeat> { };

            foreach (VehicleSeat seat in seats)
            {
                if (vehicle.IsSeatFree(seat))
                {
                    availableSeats.Add(seat);
                }
            }

            if (availableSeats == null || !availableSeats.Any())
            {
                ReturnDebugMessage("You have no available seats!");
                return false;
            }
            else
            {
                ClearNotifications();
                return true;
            }
        }

        /// <summary>
        /// Return a random available seat.
        /// </summary>
        /// <param name="vehicle">The vehicle<see cref="Vehicle"/>.</param>
        /// <returns>The <see cref="VehicleSeat"/>.</returns>
        public static VehicleSeat GetRandomSeat(Vehicle vehicle)
        {
            if (Menus.debug == true)
            {
                GTA.UI.Screen.ShowSubtitle("Random vehicle seat was called.");
            }
            
            List<VehicleSeat> seats = new List<VehicleSeat>() { VehicleSeat.LeftFront, VehicleSeat.LeftRear, VehicleSeat.RightFront, VehicleSeat.RightRear };
            List<VehicleSeat> availableSeats = new List<VehicleSeat> { };

            availableSeats.Clear();

            foreach(var seat in seats)
            {
                if(vehicle.IsSeatFree(seat))
                {
                    availableSeats.Add(seat);
                }
            }

            VehicleSeat noSeat = VehicleSeat.None;

            if (VehicleChecks() == false)
            {
                if(Menus.debug == true)
                {
                    GTA.UI.Screen.ShowSubtitle("Vehicle checks came back false.");
                }
                return noSeat;
            }

            Random random = new Random();

            //Get random seat
            VehicleSeat randomSeat = availableSeats[random.Next(0, availableSeats.Count)];

            //Check if seat is available
            if (vehicle.IsSeatFree(randomSeat))
            {
                //Return available seat
                if (Menus.debug == true)
                {
                    GTA.UI.Screen.ShowSubtitle("Vehicle seat came back empty!");
                }
                return randomSeat;
            }
            else
            {
                //Seat not available
                if (Menus.debug == true)
                {
                    GTA.UI.Screen.ShowSubtitle("Vehicle seat came back not empty.");
                }
                return noSeat;
            }
        }

        public static bool VehicleChecks()
        {
            if (Methods.IsPlayerInVehicle() == false || Methods.IsPlayerDriver() == false)
            {
                if(Methods.IsPlayerInVehicle() == true)
                {
                    var playerChar = Game.Player.Character;
                    var playerVehicle = playerChar.CurrentVehicle;
                    GTA.UI.Screen.ShowHelpTextThisFrame("Your current state is not suitable for passengers. Please find an acceptable vehicle.");
                    if (Methods.VehicleHasAvailableSeat(playerVehicle) == false) return false;
                }
                GTA.UI.Screen.ShowHelpTextThisFrame("Your current state is not suitable for passengers. Please find an acceptable vehicle.");
                return false;
            }        

            return true;
        }

        /// <summary>
        /// Returns where the ride should take place.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetRideLocation()
        {
            var playerChar = Game.Player.Character;

            if(World.GetDistance(new Vector3(-122.2531f, -916.9026f, 28.9788f), playerChar.Position) >= 2950f)
            {
                return "Country";
            } else
            {
                return "City";
            }
        }

        public static bool CreatePed(Ped ped, Vector3 position, bool randomPed = false, bool freezePed = false, PedHash model = PedHash.Bartender01SFY)
        {
            if(randomPed == true)
            {
                RideMission.passenger = World.CreateRandomPed(position);
            }
            else
            {
                RideMission.passenger = World.CreatePed(model, new Vector3(position.X, position.Y, World.GetGroundHeight(position)));
            }

            if(freezePed == true)
            {
                RideMission.passenger.IsPositionFrozen = true;
            } else
            {
                RideMission.passenger.IsPositionFrozen = false;
            }

            if (RideMission.passenger.Exists() && RideMission.passenger.IsAlive)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public static void ClearPedTasks(Ped ped)
        {
            if (clearedTasks == true) return;
            ped.Task.ClearAllImmediately();
            clearedTasks = true;
        }

        public static void PedChecks(Ped ped)
        {
            if (RideMission.activeMission == false && ped == null) return;

            try
            {
                if (ped.IsDead || !ped.Exists() || ped == null)
                {
                    RideMission.CancelRide("pedDied");
                }
            }
            catch { }
        }

        public static bool inVehicle;
        public static bool enteringVehicle;
        public static bool clearedTasks = false;

        public static void PedVehicleChecks()
        {
            if (RideMission.activeMission == false) return;
            if (RideMission.passenger == null || !RideMission.passenger.Exists()) return;
            if (RideMission.currentVehicle == null) return;

            if (RideMission.passenger.IsInVehicle(RideMission.currentVehicle))
            {
                inVehicle = true;
            }

            if (!RideMission.passenger.IsInVehicle(RideMission.currentVehicle) && enteringVehicle == false)
            {
                inVehicle = false;
            }

            if (RideMission.passenger.IsGettingIntoVehicle)
            {
                enteringVehicle = true;
            }
            else { enteringVehicle = false; }
        }

        

        public static void PedEnterVehicle(Vehicle vehicle, Ped ped, VehicleSeat seat)
        {
            if (!Game.Player.Character.IsInVehicle()) return;

            if (vehicle != null && inVehicle == false && enteringVehicle == false)
            {
                ped.Task.EnterVehicle(vehicle, seat, -1, 1, EnterVehicleFlags.WarpIfDoorIsBlocked);
            }
        }

        public static bool drawBigMessage = false;
        private static BigMessage bigMessage;
        public static void CreateBigMessage(string title, string subtitle, int duration)
        {
            bigMessage = new BigMessage(title, subtitle, MessageType.Plane);
            drawBigMessage = true;

            myTimer = new VariableTimer(duration);
            myTimer.AutoReset = false;
            myTimer.OnTimerExpired += StopBigMessage;
            myTimer.Start();
        }

        public static void DrawBigMessage()
        {
            if(drawBigMessage == true)
            {
                bigMessage.Draw();
                bigMessage.Visible = true;
                GTA.UI.Screen.ShowSubtitle(myTimer.Counter.ToString());
            }
        }

        private static void StopBigMessage(object _)
        {
            drawBigMessage = false;
            bigMessage.Dispose();
            myTimer.Stop();
        }
        
        public static void UpdateTimers()
        {
            try
            {
                RideMission.rideStopwatch.Update(Game.TimeScale);
            } catch(Exception e) { }

            try
            {
                myTimer.Update(Game.TimeScale);
            } catch { }
        }
    }
}
