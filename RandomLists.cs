using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Math;
using GTA;
using GTA.Native;

namespace Uber_Driver_Re_Written
{
    class RandomLists
    {
        //Make celebrity available to all classes
        public static List<Tuple<string, PedHash>> celebrityNames = new List<Tuple<string, PedHash>>();

        //Strings
        public static string CelebrityName;

        //Ped Model
        public static PedHash CelebrityModel;

        //Create list function / adding items to list
        public static void CreateCelebrityList() {
            celebrityNames.Add(new Tuple<string, PedHash>("Lazlow", PedHash.Lazlow2));
            celebrityNames.Add(new Tuple<string, PedHash>("Lacey Jones", PedHash.LaceyJones02));
            celebrityNames.Add(new Tuple<string, PedHash>("Kerry Mcintosh", PedHash.KerryMcintosh));
            celebrityNames.Add(new Tuple<string, PedHash>("Mark Fosterburg", PedHash.Markfost));
            celebrityNames.Add(new Tuple<string, PedHash>("Miranda Cowan", PedHash.Miranda));
            celebrityNames.Add(new Tuple<string, PedHash>("Tyler Dixon", PedHash.TylerDixon));
            celebrityNames.Add(new Tuple<string, PedHash>("Poppy Mitchell", PedHash.PoppyMich02));
            celebrityNames.Add(new Tuple<string, PedHash>("Al Di Napoli", PedHash.AlDiNapoli));
            celebrityNames.Add(new Tuple<string, PedHash>("Anita Mendoza", PedHash.AnitaCutscene));
            celebrityNames.Add(new Tuple<string, PedHash>("Tao Cheng", PedHash.TaoCheng2));
            celebrityNames.Add(new Tuple<string, PedHash>("Solomon Richards", PedHash.Solomon));
            celebrityNames.Add(new Tuple<string, PedHash>("Martin Madrazo", PedHash.MartinMadrazoCutscene));
            celebrityNames.Add(new Tuple<string, PedHash>("Tony Prince", PedHash.TonyPrince));
            celebrityNames.Add(new Tuple<string, PedHash>("Sessanta", PedHash.Sessanta));
            celebrityNames.Add(new Tuple<string, PedHash>("Moodymann", PedHash.Moodyman02));
            celebrityNames.Add(new Tuple<string, PedHash>("Agatha Baker", PedHash.Agatha));
            celebrityNames.Add(new Tuple<string, PedHash>("Tom Connors", PedHash.TomCasino));
        }

        //Generate random first and last name
        public static void GetName()
        {
            Random random = new Random();

            //Get postion of first and last name
            int celebrityName = random.Next(celebrityNames.Count);

            //Convert to string
            CelebrityName = celebrityNames[celebrityName].Item1;

            //Set model
            CelebrityModel = celebrityNames[celebrityName].Item2;

            return;
        }
    }
}
