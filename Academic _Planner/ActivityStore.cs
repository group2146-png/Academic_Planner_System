using System;
using System.Collections.Generic;
using System.Linq;

namespace Academic__Planner
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Difficulty { get; set; } = "Medium";   // ipinapakita bilang Priority (High / Medium / Low)
        public DateTime Start { get; set; }                  // araw na ginawa
        public DateTime End { get; set; }                    // Deadline
        public string Notes { get; set; } = "";
        public bool IsCompleted { get; set; }
    }

    // Isang listahan na gamit ng Dashboard, My Activities at Completed
    public static class ActivityStore
    {
        public static List<Activity> Items { get; } = new List<Activity>();

        // Hindi pa tapos, nakaayos ayon sa deadline
        public static List<Activity> Pending() =>
            Items.Where(a => !a.IsCompleted).OrderBy(a => a.End).ThenBy(a => a.Id).ToList();

        // Tapos na, pinakabago ang deadline sa taas
        public static List<Activity> Done() =>
            Items.Where(a => a.IsCompleted).OrderByDescending(a => a.End).ThenByDescending(a => a.Id).ToList();
    }
}
