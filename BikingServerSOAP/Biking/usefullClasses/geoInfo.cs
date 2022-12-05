using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GeoInfo
{
    string city;
    GeoCoordinate coordinate;

    public GeoInfo(double latitude, double longitude, string city)
    {
        this.coordinate = new GeoCoordinate(latitude, longitude);
        this.city = city;
    }
    
    public string getCity()
    {
        return city;
    }

    public GeoCoordinate getCoordinate()
    {
        return coordinate;
    }
    
}
