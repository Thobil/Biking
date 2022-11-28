using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDecauxCache
{
    internal class Cache
    {
        DateTime date;
        TimeSpan interval;

        public Cache(int hour, int min, int sec)
        {
            date = new DateTime();
            interval = new TimeSpan(hour, min, sec);
        }

        public bool isExpired()
        {
            return (date + interval).CompareTo(DateTime.Now)<0; 
        }

        public void update(string apiJson)
        {
            jsonCache = apiJson;
            date = DateTime.Now;
        }

        public string jsonCache { get; set; }
    }

    class StationCache : Cache
    {
        public StationCache(int hour, int min, int sec) : base(hour, min, sec) { }

        public string contract { get; set; }
    }
}
