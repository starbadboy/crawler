using System;

namespace VideoLinkCrawler.Models
{
    public class Schedule
    {
        public string League { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }
        public string Link { get; set; }
        public DateTime Time { get; set; }
    }
}