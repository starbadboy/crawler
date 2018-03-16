using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using VideoLinkCrawler.Models;
using VideoLinkCrawler.Repo;

namespace VideoLinkCrawler.Steps
{
    [TestFixture]
    public class ScheduleCrawler
    {
        private IWebDriver _driver;
        public List<Schedule> Schedules = new List<Schedule>();
        private readonly EplRepo _eplRepo = new EplRepo();

        [SetUp]
        public void SetupTest()
        {
            _driver = new ChromeDriver();
            var _BaseUrl = "https://www.premierleague.com/fixtures";
            _driver.Navigate().GoToUrl(_BaseUrl);
           _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
        }

        [Test]
        public void Crawl()
        {
            var content = _driver.FindElement(By.ClassName("fixtures"));
            var datelong = content.FindElements(By.CssSelector(".date.long"));
            var matchlists = content.FindElements(By.ClassName("matchList"));
            int i = 0;
            foreach (var matchlist in matchlists)
            {
                if(string.IsNullOrEmpty(matchlist.Text))
                    continue;
                
                string[] stringSeparators = {"\r\n"};
                string[] lines = datelong[i].Text.Split(stringSeparators, StringSplitOptions.None);
                i++;

                
                var matches = matchlist.FindElements(By.ClassName("matchFixtureContainer"));

                foreach (var match in matches)
                {
                    string[] data = match.Text.Split(stringSeparators, StringSplitOptions.None);
                    var teams = match.FindElements(By.ClassName("teamName"));

                    var home = teams[0].Text;
                    var time = ParseDateTime(data[1],lines[0]);
                    var away = teams[1].Text;


                    var schedule = new Schedule
                    {
                        Home = home,
                        Away = away,
                        Time = time
                    };
                    if(GetTimePart(data[1])!=DateTime.MinValue)
                      Schedules.Add(schedule);
                }
  
            }
           
            UpdateDb(Schedules);

        }

        private DateTime ParseDateTime(string time,string date)
        {
            string[] data=date.Split(' ');
            var dayoftheweek = data[0];
            var day = data[1];
            var month = data[2];
            var year = data[3];
           
            var datetimestring = dayoftheweek + ", " + month + " " + day + ", " + year;
        
            DateTime dateTime;
           
            DateTime.TryParseExact(datetimestring, "D", CultureInfo.CreateSpecificCulture("en-US"),
                DateTimeStyles.None, out dateTime);
            var timepart = GetTimePart(time);
            dateTime = dateTime.AddHours(timepart.Hour);
            dateTime = dateTime.AddMinutes(timepart.Minute);
            if (dateTime == DateTime.MinValue)
                dateTime = DateTime.Parse(SqlDateTime.MinValue.ToString());
            return dateTime;
        }

        private DateTime GetTimePart(string time)
        {
            DateTime timepart;
            DateTime.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out timepart);
            if (timepart < DateTime.Today)
                DateTime.TryParseExact(time, "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out timepart);
            return timepart;
        }

        public void UpdateDb(List<Schedule> schedules)
        {
            foreach (var sche in schedules)
            {
                _eplRepo.UpDateSchedule(sche);
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