using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JCDecauxCache
{
    public class CacheJCDecaux : ICacheJCDecaux
    {
        Cache contracts;
        List<StationCache> stations;

        CacheJCDecaux()
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
            foreach(StationCache station in stations)
            {
                if(station.contract == contract)
                {
                    if (station.isExpired())
                        return stationApiCallAndUpdateCache(contract, station);
                    else
                        return station.jsonCache;
                }
            }
            StationCache s = new StationCache(0, 10, 0);
            stations.Add(s);
            return stationApiCallAndUpdateCache(contract,s);
        }

        private string stationApiCallAndUpdateCache(string contract, StationCache s)
        {
            string query = "contract=" + contract + "&apiKey=" + API_JCDecaux.Key;
            Task<string> responsebody = JCDecauxAPICall("https://api.jcdecaux.com/vls/v3/stations", query);
            if (responsebody == null) return null;
            string response = clean(responsebody.Result);
            s.update(response);
            return response;
        }


        static async Task<string> JCDecauxAPICall(string url, string query)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url + "?" + query);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsStringAsync();
        }

        private string clean(string s)
        {
            // TODO
            return s;
        }
    }
}

