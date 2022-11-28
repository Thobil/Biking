﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using GeoCoordinatePortable;
using Project1.Cache;
using System.Net.Http;
using System.Globalization;

namespace Project1
{
    internal class faistout
    {
        static void Main()
        {
            faistout2 f = new faistout2();
            List<string> s = f.getTrajectoryToFirstStation("3 Place de la République, Mulhouse", "BOULEVARD CHARLES STOESSEL, Mulhouse");
            foreach(string s2 in s)
            {
                Console.WriteLine(s2);
            }
        }
    }

    internal class faistout2
    {
        private List<Station> stations;

        private List<string> trajectory = new List<string>();


        public List<string> getTrajectoryToFirstStation(string fromAdress, string toAdress)
        {
            GeoInfo fromInfo = transformAdressToGeoCoordinate(fromAdress);
            GeoInfo toInfo = transformAdressToGeoCoordinate(toAdress);

            Station firstStation = getClosestStation(fromInfo);

            if (distance(fromInfo.getCoordinate(), toInfo.getCoordinate()) < distance(fromInfo.getCoordinate(), firstStation.getCoordinates()))
                trajectory.AddRange(trajectoryPath(fromInfo.getCoordinate(), toInfo.getCoordinate(), true));
            else
            {
                Station lastStation = getClosestStation(toInfo);
                trajectory.AddRange(trajectoryPath(fromInfo.getCoordinate(), firstStation.getCoordinates(), true));
                trajectory.AddRange(trajectoryPath(firstStation.getCoordinates(), lastStation.getCoordinates(), false));
                trajectory.AddRange(trajectoryPath(firstStation.getCoordinates(), toInfo.getCoordinate(), true));
            }
            return trajectory;
        }

        private Station getClosestStation(GeoInfo to)
        {
            if (stations == null) stations = getStations(to.getCity());

            Station closest = null;
            double distMin = -1;
            foreach (Station s in stations)
            {
                if (s == null) continue;
                double dist = distance(to.getCoordinate(), s.getCoordinates());
                if (distMin > dist || distMin == -1)
                {
                    distMin = dist;
                    closest = s;
                }
            }
            return closest;
        }

        private List<string> trajectoryPath(GeoCoordinate from, GeoCoordinate to, bool onFoot)
        {
            List<string> traj = new List<string>();

            string locomotion;
            if (onFoot)
            {
                locomotion = "foot-walking";
                traj.Add("---------------Walk---------------");
            }
            else
            {
                locomotion = "cycling-regular";
                traj.Add("---------------Bike---------------");
            }

            
            string start = "&start=" + from.Longitude.ToString(CultureInfo.InvariantCulture) + "," + from.Latitude.ToString(CultureInfo.InvariantCulture);
            string end = "&end=" + to.Longitude.ToString(CultureInfo.InvariantCulture) + "," + to.Latitude.ToString(CultureInfo.InvariantCulture);
            string response = APICall("https://api.openrouteservice.org/v2/directions/" + locomotion, "api_key=" + API_OpenStreetMap.key + start + end).Result;
            
            Trajectory r = JsonSerializer.Deserialize<Trajectory>(response);

            if (r != null)
            {
                if(r.features != null)
                {
                    foreach(Step s in r.features[0].properties.segments[0].steps)
                    {
                        if (s == null) continue;
                        string direction = "Instruction : " + s.instruction + "\nDistance : " + s.distance + "\nDuration " + s.duration;
                        traj.Add(direction);
                    }
                }
            }
            return traj;
        }

        private double distance(GeoCoordinate a, GeoCoordinate b)
        {
            return a.GetDistanceTo(b);
        }

        private GeoInfo transformAdressToGeoCoordinate(string adress)
        {
            string response = APICall("https://api.openrouteservice.org/geocode/search", "api_key=" + API_OpenStreetMap.key + "&text=" + adress).Result;
            Adress r = JsonSerializer.Deserialize<Adress>(response);
            if (r.features.Length == 0) return new GeoInfo(-79.4063075, 0.3149312,null);
            return new GeoInfo(r.features[0].geometry.coordinates[1], r.features[0].geometry.coordinates[0], r.geocoding.query.parsed_text.city);
        }

        private List<Contract> getContracts()
        {
            CacheJCDecauxClient cache = new CacheJCDecauxClient();
            string response = cache.getContracts();
            return JsonSerializer.Deserialize<List<Contract>>(response);
        }

        private List<Station> getStations(string contract)
        {
            CacheJCDecauxClient cache = new CacheJCDecauxClient();
            string response = cache.getAllStationsOfContract(contract);
            return JsonSerializer.Deserialize<List<Station>>(response);
        }

        static async Task<string> APICall(string url, string query)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url + "?" + query);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();

        }
    }
}
