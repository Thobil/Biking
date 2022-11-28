using GeoCoordinatePortable;

public class Contract
{
    public string name { get; set; }
}
public class Station
{
    public int number { get; set; }
    public string contractName { get; set; }
    public string name { get; set; }
    public string address { get; set; }
    public Position position { get; set; }
    public bool banking { get; set; }
    public bool bonus { get; set; }
    public string status { get; set; }
    public bool connected { get; set; }
    public bool overflow { get; set; }
    public object shape { get; set; }
    public Totalstands totalStands { get; set; }
    public Mainstands mainStands { get; set; }
    public object overflowStands { get; set; }

    public GeoCoordinate getCoordinates()
    {
        return new GeoCoordinate(position.latitude, position.longitude);
    }
}

public class Position
{
    public float latitude { get; set; }
    public float longitude { get; set; }
}

public class Totalstands
{
    public Availabilities availabilities { get; set; }
    public int capacity { get; set; }
}

public class Availabilities
{
    public int bikes { get; set; }
    public int stands { get; set; }
    public int mechanicalBikes { get; set; }
    public int electricalBikes { get; set; }
    public int electricalInternalBatteryBikes { get; set; }
    public int electricalRemovableBatteryBikes { get; set; }
}

public class Mainstands
{
    public Availabilities1 availabilities { get; set; }
    public int capacity { get; set; }
}

public class Availabilities1
{
    public int bikes { get; set; }
    public int stands { get; set; }
    public int mechanicalBikes { get; set; }
    public int electricalBikes { get; set; }
    public int electricalInternalBatteryBikes { get; set; }
    public int electricalRemovableBatteryBikes { get; set; }
}
