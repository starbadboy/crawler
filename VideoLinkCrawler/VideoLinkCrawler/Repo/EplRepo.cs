using System.Data.SqlClient;
using Dapper;
using VideoLinkCrawler.Models;
using VideoLinkCrawler.Steps;

namespace VideoLinkCrawler.Repo
{
    public class EplRepo
    {
        private readonly SqlConnection _connection;
        private SqlConnection _localsqlConnection;
        private SqlConnection _productionsqlConnection;
        public EplRepo()
        {
            _localsqlConnection = new SqlConnection(@"Server = .\SQLEXPRESS; Database = Epldb; User ID = test; Password = 1234asdf;");
            _productionsqlConnection = new SqlConnection(@"Data Source=SQL5036.SmarterASP.NET;Initial Catalog=DB_A27F66_epl;User Id=DB_A27F66_epl_admin;Password=asdf1234;");
            _connection = _productionsqlConnection;
            _connection.Open();
        }

        public void UpDateScheduleLink(Schedule scheduler)
        {
            var sql = @"declare @homeid int;
                        declare @awayid int;
                        select @homeid=teamid from TeamInfo where TeamNameCN=@Home
                        select @awayid=teamid from TeamInfo where TeamNameCN=@Away
                        IF EXISTS(SELECT 1 FROM  [dbo].[Schedule] WHERE Homeid=@homeid and Awayid=@awayid and Time=@time)
                        BEGIN UPDATE [Schedule] set Link =@Link , LastModifiedOn= getdate() where Homeid=@homeid and Awayid=@awayid and Time=@time End";
            _connection.Execute(sql, scheduler);
            _connection.Close();
        }


        public void UpDateScheduleLink(DbSchedule scheduler)
        {
            var sql = @"UPDATE [Schedule] set Link =isnull(@Link,Link),HighLightLink =isnull(@HighLightLink,HighLightLink),
                       FirstHalfLink =isnull(@FirstHalfLink,FirstHalfLink),
                       SecondHalfLink =isnull(@SecondHalfLink,SecondHalfLink), 
                      LastModifiedOn =getdate() WHERE Id=@Id";
            _connection.Execute(sql, scheduler);
            _connection.Close();
        }

        public void UpDateSchedule(Schedule scheduler)
        {
            var sql = @"declare @homeid int;
                        declare @awayid int;
                        select @homeid=teamid from TeamInfo where TeamNameShort=@Home
                        select @awayid=teamid from TeamInfo where TeamNameShort=@Away
                        IF EXISTS(SELECT 1 FROM  [dbo].[Schedule] WHERE Homeid=@homeid and Awayid=@awayid)
                        BEGIN UPDATE [dbo].[Schedule] set Time =@Time, LastModifiedOn= getdate() where Homeid=@homeid and Awayid=@awayid End
                        ELSE 
                        Insert into [dbo].[Schedule](homeid,awayid,time,lastmodifiedon) values (@homeid,@awayid,@time,getdate())";
            _connection.Execute(sql, scheduler);
            _connection.Close();
        }



        public void DeleteSchedule(int i)
        {
            var sql = @"delete from schedule where id =" + i;
            _connection.Execute(sql);
            _connection.Close();
        }
    }
}