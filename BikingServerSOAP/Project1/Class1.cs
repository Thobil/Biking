using GeoCoordinatePortable;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using JCDecauxCache;

namespace Project1
{
    internal class Class1
    {
        
        static void Main()
        {
            faittout t = new faittout();
            string[] s = t.getTrajectory("rue pelisson villeurbanne", "rue tronchet lyon");
            foreach (string a in s)
                Console.WriteLine(a);
            s = t.getTrajectory("rue pelisson villeurbanne", "rue tronchet lyon");
            foreach (string a in s)
                Console.WriteLine(a);
        }
    }

    class faittout
    {
        CacheJCDecaux cache = new CacheJCDecaux();
        private List<Station> stations;

        private List<string> trajectory = new List<string>();

        public string[] getTrajectory(string fromAdress, string toAdress)
        {
            if (fromAdress == null || toAdress == null || fromAdress == "" || toAdress == "") return trajectory.ToArray();

            GeoInfo fromInfo = transformAdressToGeoCoordinate(fromAdress);
            GeoInfo toInfo = transformAdressToGeoCoordinate(toAdress);

            Station firstStation = getClosestStation(fromInfo, false);
            Station lastStation = getClosestStation(toInfo, true);

            if (fromInfo == null || toInfo == null || firstStation == null || lastStation == null) return trajectory.ToArray();

            if (distance(fromInfo.getCoordinate(), toInfo.getCoordinate()) < distance(fromInfo.getCoordinate(), firstStation.getCoordinates()))
                trajectory.AddRange(trajectoryPath(fromInfo.getCoordinate(), toInfo.getCoordinate(), true));
            else
            {
                trajectory.AddRange(trajectoryPath(fromInfo.getCoordinate(), firstStation.getCoordinates(), true));
                trajectory.AddRange(trajectoryPath(firstStation.getCoordinates(), lastStation.getCoordinates(), false));
                trajectory.AddRange(trajectoryPath(firstStation.getCoordinates(), toInfo.getCoordinate(), true));
            }
            return trajectory.ToArray();
        }

        private Station getClosestStation(GeoInfo to, bool isOnBike)
        {
            if (stations == null)
            {
                string city = getContractOfCity(to.getCity());
                if (city == null) return null;

                stations = getStations(city);
                if (stations == null) return null;
            }

            Station closest = null;
            double distMin = -1;
            foreach (Station s in stations)
            {
                if (s == null) continue;
                if ((!isOnBike && s.mainStands.availabilities.bikes > 0) || (isOnBike && s.mainStands.availabilities.stands > 0))
                {
                    double dist = distance(to.getCoordinate(), s.getCoordinates());
                    if (distMin > dist || distMin == -1)
                    {
                        distMin = dist;
                        closest = s;
                    }
                }
            }
            return closest;
        }

        private string getContractOfCity(string city)
        {
            string response = cache.getContracts();
            if (response == null) return null;
            List<Contract> contracts = JsonSerializer.Deserialize<List<Contract>>(response);
            if (contracts == null) return null;

            foreach (Contract contract in contracts)
            {
                if (contract.name.ToLower() == city.ToLower())
                    return contract.name;
                if (contract.cities != null)
                    foreach (string cc in contract.cities)
                    {
                        if (city.ToLower() == cc.ToLower())
                            return contract.name;
                    }
            }
            return null;
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

            Task<string> responsebody = APICall("https://api.openrouteservice.org/v2/directions/" + locomotion, "api_key=" + API_OpenStreetMap.key + start + end);
            if (responsebody == null) return new List<string>();
            string response = responsebody.Result;

            Trajectory r = JsonSerializer.Deserialize<Trajectory>(response);

            if (r != null)
            {
                if (r.features != null)
                {
                    foreach (Step s in r.features[0].properties.segments[0].steps)
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
            Task<string> responsebody = APICall("https://api.openrouteservice.org/geocode/search", "api_key=" + API_OpenStreetMap.key + "&text=" + adress);

            if (responsebody == null) return null;
            string response = responsebody.Result;

            Adress r = JsonSerializer.Deserialize<Adress>(response);
            if (r.features.Length == 0) return new GeoInfo(-79.4063075, 0.3149312, null);
            return new GeoInfo(r.features[0].geometry.coordinates[1], r.features[0].geometry.coordinates[0], r.geocoding.query.parsed_text.city);
        }

        private List<Station> getStations(string contract)
        {
            Console.WriteLine("a " + contract);
            string response = cache.getAllStationsOfContract(contract);
            if (response == null) return null;
            return JsonSerializer.Deserialize<List<Station>>(response);
        }

        static async Task<string> APICall(string url, string query)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url + "?" + query);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsStringAsync();
        }
    }

    public class CacheJCDecaux
    {
        Cache contracts;
        List<StationCache> stations;

        public CacheJCDecaux()
        {
            stations = new List<StationCache>();
            contracts = new Cache(1, 0, 0);
            contractApiCallAndUpdateCache();
        }

        public string getContracts()
        {
            if (contracts.isExpired())
                return contractApiCallAndUpdateCache();
            return contracts.jsonCache;
        }

        private string contractApiCallAndUpdateCache()
        {
            string query = "apiKey=" + API_JCDecaux.Key;
            Task<string> responsebody = JCDecauxAPICall("https://api.jcdecaux.com/vls/v3/contracts", query);
            if (responsebody == null) return null;
            string response = responsebody.Result;
            contracts.update(response);
            return response;
        }

        public string getAllStationsOfContract(string contract)
        {
            foreach (StationCache station in stations)
            {
                if (station.contract == contract)
                {
                    if (station.isExpired())
                        return stationApiCallAndUpdateCache(contract, station);
                    else
                        return station.jsonCache;
                }
            }
            StationCache s = new StationCache(0, 10, 0);
            stations.Add(s);
            return stationApiCallAndUpdateCache(contract, s);
        }

        private string stationApiCallAndUpdateCache(string contract, StationCache s)
        {
            string query = "contract=" + contract + "&apiKey=" + API_JCDecaux.Key;
            Task<string> responsebody = JCDecauxAPICall("https://api.jcdecaux.com/vls/v3/stations", query);
            if (responsebody == null) return null;
            string response = responsebody.Result;
            s.update(response);
            return response;
        }


        static async Task<string> JCDecauxAPICall(string url, string query)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 300000;
            HttpResponseMessage response = await client.GetAsync(url + "?" + query);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsStringAsync();
        }
    }
}
