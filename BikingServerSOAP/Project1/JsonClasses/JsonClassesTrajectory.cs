
public class Trajectory
{
    public Features[] features { get; set; }
}

public class Features
{
    public Properties properties { get; set; }
}

public class Properties
{
    public Segment[] segments { get; set; }
    public Summary summary { get; set; }
    public int[] way_points { get; set; }
}

public class Summary
{
    public float distance { get; set; }
    public float duration { get; set; }
}

public class Segment
{
    public float distance { get; set; }
    public float duration { get; set; }
    public Step[] steps { get; set; }
}

public class Step
{
    public float distance { get; set; }
    public float duration { get; set; }
    public int type { get; set; }
    public string instruction { get; set; }
    public string name { get; set; }
    public int[] way_points { get; set; }
}