using ActivemqProducer;
using Biking.ProxyCache;
using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biking
{
    public class MyBiking : IMyBiking
    {
        CacheJCDecauxClient cache = new CacheJCDecauxClient();
        private List<Station> stations;
        private List<string> trajectory = new List<string>();

        public string getTrajectory(string fromAdress, string toAdress)
        {
            QueueJCDecaux queue = new QueueJCDecaux();

            if (fromAdress == null || toAdress == null || fromAdress == "" || toAdress == "") return queue.sendMessage(trajectory.ToArray());

            GeoInfo fromInfo = transformAdressToGeoCoordinate(fromAdress);
            GeoInfo toInfo = transformAdressToGeoCoordinate(toAdress);

            if (fromInfo == null || toInfo == null) return queue.sendMessage(trajectory.ToArray());

            Station firstStation = getClosestStation(fromInfo, false);
            Station lastStation = getClosestStation(toInfo, true);

            TrajectoryInfos walk = trajectoryPath(fromInfo.getCoordinate(), toInfo.getCoordinate(), true);

            if(firstStation == null || lastStation == null)
            {
                trajectory.Add("Total duration : " + Math.Ceiling(walk.duration / 60) + " min\nTotal distance : " + Math.Ceiling(walk.distance / 100) / 10 + " km");
                trajectory.AddRange(walk.instructions);
                return queue.sendMessage(trajectory.ToArray());
            }

            TrajectoryInfos walk1 = trajectoryPath(fromInfo.getCoordinate(), firstStation.getCoordinates(), true);
            TrajectoryInfos bike = trajectoryPath(firstStation.getCoordinates(), lastStation.getCoordinates(), false);
            TrajectoryInfos walk2 = trajectoryPath(lastStation.getCoordinates(), toInfo.getCoordinate(), true);

            float totalDuration = walk1.duration + walk2.duration + bike.duration;
            float totalDistance = walk1.distance + walk2.distance + bike.distance;

            if (walk.duration <= totalDuration)
            {
                trajectory.Add("Total duration : " + Math.Ceiling(walk.duration/60) + " min\nTotal distance : " + Math.Ceiling(walk.distance/100)/10 + " km");
                trajectory.AddRange(walk.instructions);
            }
            else
            {
                trajectory.Add("Total duration : " + Math.Ceiling(totalDuration /60) + " min\nTotal distance : " + Math.Ceiling(totalDistance /100)/10 + " km");
                trajectory.AddRange(walk1.instructions);
                trajectory.AddRange(bike.instructions);
                trajectory.AddRange(walk2.instructions);
            }

            return queue.sendMessage(trajectory.ToArray());
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
            if (city == null) return null;
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

        private TrajectoryInfos trajectoryPath(GeoCoordinate from, GeoCoordinate to, bool onFoot)
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
            if (responsebody == null) return new TrajectoryInfos(9999999,9999999, new List<string>());
            string response = responsebody.Result;

            Trajectory r = JsonSerializer.Deserialize<Trajectory>(response);

            if (r != null)
            {
                if (r.features != null)
                {
                    foreach (Step s in r.features[0].properties.segments[0].steps)
                    {
                        if (s == null) continue;
                        string direction = "Instruction : " + s.instruction + "\nDistance : " + Math.Ceiling(s.distance) + " m \nDuration " + Math.Ceiling(s.duration/60) + " min";
                        traj.Add(direction);
                    }
                }
            }
            return new TrajectoryInfos(r.features[0].properties.summary.duration, r.features[0].properties.summary.distance, traj);
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
            if (r.features.Length == 0) return null;
            return new GeoInfo(r.features[0].geometry.coordinates[1], r.features[0].geometry.coordinates[0], r.geocoding.query.parsed_text.city);
        }

        private List<Station> getStations(string contract)
        {
            string response = cache.getAllStationsOfContract(contract);
            try
            {
                return JsonSerializer.Deserialize<List<Station>>(response);
            }
            catch (Exception)
            {
                return null;
            }
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
}
