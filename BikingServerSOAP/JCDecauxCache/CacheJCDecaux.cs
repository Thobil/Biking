using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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
            string responseBody = JCDecauxAPICall("https://api.jcdecaux.com/vls/v3/contracts", query).Result;
            contracts.update(responseBody);
            return responseBody;
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
            string responsebody = JCDecauxAPICall("https://api.jcdecaux.com/vls/v3/stations", query).Result;
            s.update(responsebody);
            return responsebody;
        }


        static async Task<string> JCDecauxAPICall(string url, string query)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url + "?" + query);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

