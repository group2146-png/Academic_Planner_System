using System;
using System.Collections.Generic;

public class ActivityItem
{
    public string Title { get; set; }
    public DateTime StartAt { get; set; }
    public string Difficulty { get; set; }
    public DateTime EndAt { get; set; }
    public string Status { get; set; } = "Pending";
}

public static class ActivityStore
{
    public static List<ActivityItem> Activities = new List<ActivityItem>();
}