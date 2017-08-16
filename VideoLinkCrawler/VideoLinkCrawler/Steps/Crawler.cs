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
    public class Crawler
    {
        private IWebDriver _driver;
        public List<Schedule> Schedules = new List<Schedule>();
        private readonly EplRepo _eplRepo = new EplRepo();

        [SetUp]
        public void SetupTest()
        {
           _driver = new ChromeDriver();
            var _BaseUrl = "http://nba.tmiaoo.com/body.html";
            _driver.Navigate().GoToUrl(_BaseUrl);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void Crawl()
        {
            var content = _driver.FindElement(By.ClassName("game-container-inner"));
            var gamelists = content.FindElements(By.ClassName("game-item"));
            foreach (var gamelist in gamelists)
            {
                string[] stringSeparators = {"\r\n"};
                string[] lines = gamelist.Text.Split(stringSeparators, StringSplitOptions.None);
                bool isHd = lines[0] == "HD";
                int index = isHd ? 1 : 0;
                var leaguename = lines[0+index];

                if (!leaguename.Contains("英超")) continue;
                var schedule = new Schedule
                {
                    Home = lines[2 + index],
                    Away = lines[4 + index],
                    Time = ParseDateTime(lines[1 + index]),
                    Link = gamelist.FindElement(By.ClassName("boss ")).GetAttribute("href"),
                    League = leaguename
                };
                Schedules.Add(schedule);
            }
            UpdateDb(Schedules);

        }

        private DateTime ParseDateTime(string datetime)
        {
            DateTime time;
            if (!DateTime.TryParseExact(datetime, "MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out time))
                DateTime.TryParseExact(datetime, "MM-dd HH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out time);
            return time;
        }

        public void UpdateDb(List<Schedule> schedules)
        {
            foreach (var sche in schedules)
            {
                _eplRepo.UpDateScheduleLink(sche);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Close();
            _driver.Quit();
        }




    }

}