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
using System.Reflection.Emit;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace UberDriverMod
{
    static class RideMission
    {
        public static bool activeMission;

        public static Ped passenger;
        public static Blip passengerBlip;

        private static string destinationName;
        private static Vector3 destinationPosition;
        public static Blip destinationBlip;

        public static Vehicle currentVehicle;

        private static List<string> passengerIdleAnim = new List<string>() { "WORLD_HUMAN_SMOKING", "WORLD_HUMAN_STAND_IMPATIENT", "WORLD_HUMAN_STAND_MOBILE", "WORLD_HUMAN_BUM_STANDING" };
        public static string randomScenario;

        public static VariableStopwatch rideStopwatch;

        // ensure vehicle is valid (has enough seats/doors) before starting mission
        public static void StartRide()
        {
            var playerChar = Game.Player.Character;

            if (Methods.VehicleChecks() == false) return;

            // implement scenarios

            if (Methods.GetRideLocation() == "City")
            {
                if (CreateCityPickup() == false) return;
            }

            CreateCityDestination();

            passengerBlip = passenger.AddBlip();
            passengerBlip.Sprite = BlipSprite.Friend;
            passengerBlip.Name = "Passenger";
            passengerBlip.Color = BlipColor.Blue;
            passengerBlip.ShowRoute = true;
            GTA.UI.Screen.ShowSubtitle("Go to the ~b~passenger.", 10000);

            passenger.BlockPermanentEvents = true;
            Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, passenger, 0, 0);
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, passenger, 17, 1);

            var random = new Random();
            int index = random.Next(passengerIdleAnim.Count);
            randomScenario = passengerIdleAnim[index];
            passenger.Task.StartScenario(randomScenario, 0);

            activeMission = true;
            Menus.cancelRideButton.Enabled = true;

            GTA.UI.Screen.ShowSubtitle(CalculateEstimatedPayout().ToString(), 7000);
        }

        private static bool CreateCityPickup()
        {
            var random = new Random();
            int index = random.Next(PassengerPickupLists.cityPickupsList.Count);

            Vector3 pickupLocation = PassengerPickupLists.cityPickupsList[index];

            if (Methods.CreatePed(passenger, pickupLocation, true, true) == true)
            {
                return true;
            }
            else
            {
                Methods.ReturnErrorMessage("Could not create the passenger.");
                return false;
            }
        }

        private static bool CreateCityDestination()
        {
            var random = new Random();
            int index = random.Next(DestinationLists.cityDestinationsList.Count);

            destinationName = DestinationLists.cityDestinationsList[index].Item1;
            destinationPosition = DestinationLists.cityDestinationsList[index].Item2;

            return true;
        }

        private static bool SetCityDestination()
        {
            destinationBlip = World.CreateBlip(destinationPosition);
            GTA.UI.Screen.ShowSubtitle("Deliver the ~b~passenger ~w~to ~y~" + destinationName + ".", 10000);
            destinationBlip.ShowRoute = true;

            if (destinationBlip.Exists())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool createdDestination = false;
        private static bool hornControlEnabled = true;
        public static void WaitForPassengerPickup()
        {
            if (activeMission == false) return;
            if (createdDestination == true) return;

            if (passengerBlip.ShowRoute == false) passengerBlip.ShowRoute = true;

            var playerChar = Game.Player.Character;
            currentVehicle = playerChar.CurrentVehicle;

            if (World.GetDistance(playerChar.Position, passenger.Position) < 70f)
            {
                passenger.IsPositionFrozen = false;
            }

            if (Methods.inVehicle == true && createdDestination == false)
            {
                if (SetCityDestination() == false)
                {
                    Methods.ReturnErrorMessage("Could not create destination");
                }
                else
                {
                    passenger.PlayAmbientSpeech("GENERIC_HI");
                    playerChar.PlayAmbientSpeech("GENERIC_HOWS_IT_GOING", SpeechModifier.Force);
                    CreateStopwatch();
                    createdDestination = true;
                }
            }

            if (Methods.inVehicle == false && Methods.enteringVehicle == false)
            {
                if (Methods.VehicleChecks() == false)
                {
                    return;
                }

                if (World.GetDistance(playerChar.Position, passenger.Position) <= 6.5f)
                {
                    passenger.IsPositionFrozen = false;
                    WaitForPedToEnterVehicle();
                    hornControlEnabled = false;
                }
                else
                {
                    hornControlEnabled = true;
                    Methods.clearedTasks = false;
                }

                if (World.GetDistance(playerChar.Position, passenger.Position) >= 10.5f && World.GetDistance(playerChar.Position, passenger.Position) < 14f && hornControlEnabled == true)
                {
                    passenger.IsPositionFrozen = false;
                    GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_VEH_HORN~ to get the passengers attention.");

                    if (Game.IsControlJustPressed(GTA.Control.VehicleHorn))
                    {
                        passenger.Task.GoTo(playerChar.Position);
                    }
                }
            }

            if (Menus.debug == true)
            {
                Methods.ReturnDebugMessage("In vehicle: " + Methods.inVehicle.ToString() + "\nEntering vehicle: " + Methods.enteringVehicle.ToString() + "\nVehicle seat: " + randomSeat.ToString() + "\nchosenRandomSeat: " + chosenRandomSeat);
            }
        }

        private static bool chosenRandomSeat = false;
        private static VehicleSeat randomSeat;
        private static void WaitForPedToEnterVehicle()
        {
            var playerChar = Game.Player.Character;
            currentVehicle = playerChar.CurrentVehicle;

            if(chosenRandomSeat == false)
            {
                randomSeat = Methods.GetRandomSeat(currentVehicle);

                if (randomSeat == VehicleSeat.None)
                {
                    chosenRandomSeat = false;
                } else
                {
                    chosenRandomSeat = true;
                }
            }

            if (Methods.enteringVehicle == false)
            {
                passenger.AlwaysKeepTask = true;
                Methods.PedEnterVehicle(currentVehicle, passenger, randomSeat);
            }
        }

        // choose which scenario this ride will be

        // create passenger, find safe location (pre determined location or random?)

        // set blip to passenger, set route to passenger

        // upon being near passenger, have them enter the vehicle

        // create option if passenger is farther away than the specified area to pick them up, if u honk the passenger notices u and enters

        // create method for ped to enter vehicle

        // once ped is in vehicle, grab a destination from the destination lists and set the blip waypoint

        // once arrived at the destination, the passenger leaves the vehicle, walks away

        // mission ends, payouts are received (tip, fare cost)

        public static void WaitToArriveAtDestination()
        {
            if (Methods.inVehicle == false || passenger == null || activeMission == false || createdDestination == false) return;

            var playerChar = Game.Player.Character;
            if (World.GetDistance(playerChar.Position, destinationPosition) < 5f)
            {
                // end mission
                EndRide();
            }

            Vector3 markerCoords = new Vector3(destinationPosition.X, destinationPosition.Y, World.GetGroundHeight(destinationPosition));
            World.DrawMarker(MarkerType.VerticalCylinder, markerCoords, Vector3.Zero, Vector3.Zero, new Vector3(4f, 4f, 4f), Color.Yellow);

            if (destinationBlip.ShowRoute == false) { destinationBlip.ShowRoute = true; }
        }

        public static int CalculatePayout()
        {
            float rideDuration = rideStopwatch.Counter;

            float minimumPayout = 50f;
            float durationCalc = rideDuration * 0.00035f; //dollar a second, inflated since gta prices are high
            int ridePayout = (int)Math.Round(minimumPayout + durationCalc);

            return ridePayout;
        }
        
        //incomplete
        public static float CalculateEstimatedPayout()
        {
            var playerChar = Game.Player.Character;

            float distance = World.CalculateTravelDistance(playerChar.Position, destinationPosition);
            float distanceInMiles = (float)Math.Round(distance / 1609.344f, 2);
            float estimatedPayout = distanceInMiles * 21f + 50;

            GTA.UI.Screen.ShowSubtitle(estimatedPayout.ToString());
            return estimatedPayout; 
        }

        public static void MissionCleanup()
        {
            activeMission = false;
            createdDestination = false;

            VehicleSeat passengerSeat = passenger.SeatIndex;

            try
            {
                passenger.Task.LeaveVehicle(LeaveVehicleFlags.WarpIfShuffleLinkIsBlocked);
                passenger.SetNoCollision(currentVehicle, true);
                passenger.Task.WanderAround();
                passenger.MarkAsNoLongerNeeded();
                passenger = null;
            }
            catch { }

            rideStopwatch.Stop();
            Methods.inVehicle = false;
            Methods.enteringVehicle = false;
            Methods.clearedTasks = false;
            randomSeat = VehicleSeat.None;
            hornControlEnabled = false;
            chosenRandomSeat = false;

            try
            {
                passengerBlip.Delete();
                destinationBlip.Delete();
            }
            catch { }

            destinationName = null;
            destinationPosition = Vector3.Zero;

            currentVehicle = null;
            Menus.cancelRideButton.Enabled = false;
            GTA.UI.Screen.ShowSubtitle("");

            try
            {
                foreach (var door in currentVehicle.Doors)
                {
                    Function.Call(Hash.SET_VEHICLE_DOOR_SHUT, currentVehicle, int.Parse(door.ToString()), true);
                }
            } catch { }
        }

        private static string GetElapsedRideTime()
        {
            var ts = TimeSpan.FromMilliseconds(rideStopwatch.Counter);
            var parts = string
                            .Format("{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s",
                                ts.Days, ts.Hours, ts.Minutes, ts.Seconds)
                            .Split(':')
                            .SkipWhile(s => Regex.Match(s, @"^00\w").Success)
                            .ToArray();
            var result = string.Join(" ", parts);

            return result;
        }

        public static void EndRide()
        {
            if (activeMission == false) return;
            try
            {
                if (Methods.inVehicle == true)
                {
                    currentVehicle.BringToHalt(5f, 1);
                }
                passenger.PlayAmbientSpeech("GENERIC_THANKS", SpeechModifier.Force);
                MissionCleanup();
                Methods.CreateBigMessage("Trip Completed", "Rating: /5 " + "Fare: $" + CalculatePayout() + " Tip: " + " Elapsed Time: " + GetElapsedRideTime(), 10000);
            }
            catch { }
        }

        public static void CancelRide(string reason)
        {
            if (activeMission == false) return;

            if(reason == "cancel")
            {
                try
                {
                    if (Methods.inVehicle == true)
                    {
                        currentVehicle.BringToHalt(5f, 1);
                    }
                    passenger.PlayAmbientSpeech("GENERIC_INSULT_HIGH");
                    MissionCleanup();
                }
                catch { }
            }

            if(reason == "pedDied")
            {
                Methods.CreateBigMessage("Trip Failed", "The passenger died.", 7500);
                MissionCleanup();
            }
        }

        private static void CreateStopwatch()
        {
            rideStopwatch = new VariableStopwatch();
            rideStopwatch.Start();
        }
    }
}

