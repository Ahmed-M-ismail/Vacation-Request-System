using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BLL.Repositories;
using BOL;


namespace YouxelVacationRequests.Tests
{
    [TestClass]
    public class TeamRepositoryTests
    {
        [TestMethod]
        public void TeamRepository_GetTeam_Call_GetResults_Test()
        {
            var context = new  YouxelVacation();


            var firstOrDefault = context.Team.FirstOrDefault();
            if (firstOrDefault == null) return;
            var  teamId = firstOrDefault.ID;

            var target = new TeamRepository(context);
            var result = target.Get(teamId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ID == teamId);
        }





    
        [TestMethod]
        public void TeamRepository_GetAllTeams_Call_GetResults_Test()
        {
            var context = new  YouxelVacation();
            var expectedCount = context.Team.Count();


            

            var target = new TeamRepository(context);
            var results = target.GetAll().ToList();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
        }








        
        [TestMethod]
        public void TeamRepository_AddTeam_Added_NotFail_Test()
        {
            var context = new  YouxelVacation();
            var expected = context.Team.Count() + 1;

            var target = new TeamRepository(context);
       
            var firstOrDefault = context.Employee.FirstOrDefault();
            if (firstOrDefault != null)
            {
                var employeeId = firstOrDefault.ID;
                var teamId = context.Team.Select(e => e.ID).Max() + 1;
                var team = new Team()
                {
                    ID = teamId,
                    ManagerID = employeeId,
                    Name = "IOS",
                    NumLevel = 4,
                };
                target.Add(team);
            }

            var actual = context.Team.Count();
            Assert.AreEqual(expected, actual);
        }











        [TestMethod]
        public void TeamRepository_UpdateTeam_NotFail_Test()
        {
            var context = new  YouxelVacation(); /*for target */
            var team = context.Team.FirstOrDefault();
            var target = new TeamRepository(context);

            var context2 = new  YouxelVacation(); /*expected to be */
            var firstOrDefault = context2.Employee.FirstOrDefault(e => e.ID != team.ManagerID);
            if (firstOrDefault != null)
                if (team != null) team.ManagerID= firstOrDefault.ID;
            target.Update(team);

            if (team != null)
            {
                var actual = target.Get(team.ID);

                Assert.AreEqual(team.ManagerID, actual.ManagerID);
            }
        }













    }
}
