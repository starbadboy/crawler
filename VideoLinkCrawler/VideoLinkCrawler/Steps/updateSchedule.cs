using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using VideoLinkCrawler.Models;
using VideoLinkCrawler.Repo;

namespace VideoLinkCrawler.Steps
{
    [TestFixture]
    public class UpdateSchedule
    {
        private readonly EplRepo _eplRepo = new EplRepo();

        [Test]
        public void UpdateScheduleDb()
        {
            var schedule = new DbSchedule
            {
                Id = 20,
                Link = null,
                HighLightLink = "https://openload.co/embed/sa8Otuyz7mA/",
                FirstHalfLink = "https://openload.co/embed/jCfFB5KIZjc/",
                SecondHalfLink = "https://openload.co/embed/4CZOzExKnqc/"
            };
            UpdateDb(schedule);
        }

        public void UpdateDb(DbSchedule schedules)
        {
             _eplRepo.UpDateScheduleLink(schedules);
           
        }



    }

    public class DbSchedule
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string HighLightLink { get; set; }
        public string FirstHalfLink { get; set; }
        public string SecondHalfLink { get; set; }
    }
}