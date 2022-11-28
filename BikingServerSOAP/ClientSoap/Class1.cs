using System;
using ClientSoap.BikingRef;

namespace ClientSoap
{
    internal class Class1
    {
        static void Main(string[] args)
        {
            MyBikingClient b = new MyBikingClient();
            string[] s = b.getTrajectory("3 Place de la République, Mulhouse", "BOULEVARD CHARLES STOESSEL, Mulhouse");
            foreach (string s2 in s)
            {
                Console.WriteLine(s2);
            }
        }
    }
}
