using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.UI;
using GTA.Math;

namespace Uber_Driver_Re_Written
{
    class GenericMethods
    {
        public static void CreateIniFile()
        {
            //File name/path
            string fileName = "scripts//UberDriver.ini";

            //If the ini file doesn't exist
            if(!File.Exists(fileName))
            {
                using(StreamWriter sw = File.CreateText(fileName))
                {
                    //Writing data to .ini file

                    //Settings section
                    sw.WriteLine("[Settings]");
                    sw.WriteLine("MenuKey = F6");
                    sw.WriteLine("AcceptKey = E");
                    sw.WriteLine("DeclineKey = T");
                    sw.WriteLine("ActiveRides = false");
                    sw.WriteLine("InstantRides = false");
                    sw.WriteLine("NotifSound = true");

                    //Stats section
                    sw.WriteLine("[Stats]");
                    sw.WriteLine("Level = 1");
                    sw.WriteLine("Completed = 0");
                    sw.WriteLine("Goal = 2");

                }   
            }
        }

        //Check if player is in vehicle with at least 2 seats
        public static void VehicleCheck(Player player)
        {           
            Vehicle playerVehicle;

            player = Game.Player;

            if (player.Character.IsInVehicle() == false)
            {
                GenericMethods.ErrorMessage("You are no longer in a vehicle.");
                UberMission.RideComplete();
                return;
            }

            //Check ensures player is in a vehicle, exists, and is alive
            if (player.Character.IsInVehicle() && player.Character.IsAlive)
            {
                //Sets variable to players current vehicle
                playerVehicle = player.Character.CurrentVehicle;

                //Player is in vehicle, set variable to true
                UberMission.isInVehicle = true;

                //Mission vehicle
                UberMission.missionVehicle = Game.Player.LastVehicle;

                //Check if vehicle has 2 seats
                if (playerVehicle.IsSeatFree(VehicleSeat.Passenger) || playerVehicle.IsSeatFree(VehicleSeat.LeftRear) || playerVehicle.IsSeatFree(VehicleSeat.RightRear))
                {
                    //Seat free, set bool to true
                    UberMission.isSeatFree = true;
                } else
                {
                    UberMission.isSeatFree = false;
                }
            } else
            {
                //Player is not in vehicle, doesn't exist, or is not alive. Notify player
                UberMission.isInVehicle = false;
                ErrorMessage("You are not in a valid vehicle, or there is an issue with your character.");
                return;
            }

            if(UberMission.isInVehicle == true && UberMission.isSeatFree == true)
            {
                //If player is in vehicle and seat is available, start the job.
                UberMission.CreateJob();
            } else
            {
                //Failed job check, notify player
                ErrorMessage("You are not in a valid vehicle, or the seat is not free.");
                return;
            }
        } 

        //Error method method so it doesn't have to be re-written everytime
        public static void ErrorMessage(string reason)
        {
            UberMission.rideCancelled = true;
            Notification.Show("Uber Driver returned error: " + reason);
        }
    }
}
