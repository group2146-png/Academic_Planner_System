using System;
using System.Collections.Generic;

public class Activity
{
    public int Id { get; set; }                       
    public string Title { get; set; }
    public string Difficulty { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsCompleted { get; set; } = false;    // false = Pending
}

public static class ActivityStore
{
    public static List<Activity> Items = new List<Activity>();
}