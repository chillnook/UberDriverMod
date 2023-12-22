using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberDriverMod
{
    static class PassengerPickupLists
    {
        public static List<Vector3> cityPickupsList = new List<Vector3>();
        public static List<Vector3> countryPickusList = new List<Vector3>();

        public static void CreateCityList()
        {
            cityPickupsList.Add(new Vector3(-1725.380f, -438.2585f, 44.3593f));
            cityPickupsList.Add(new Vector3(-1631.487f, -533.3721f, 35.7951f));
            cityPickupsList.Add(new Vector3(-1379.649f, -534.9885f, 30.3274f));
            cityPickupsList.Add(new Vector3(-1232.781f, -561.1039f, 28.3274f));
            cityPickupsList.Add(new Vector3(-960.0758f, -684.9960f, 24.4858f));
        }
    }
}
