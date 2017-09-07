using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BLL.Repositories;
using BOL;


namespace YouxelVacationRequests.Tests
{
    [TestClass]
    public class WorkflowRepositoryTests
    {
        [TestMethod]
        public void WorkflowRepository_GetWorkflow_Call_GetResults_Test()
        {
            var context = new  YouxelVacation();


            var firstOrDefault = context.Workflow.FirstOrDefault();
            if (firstOrDefault == null) return;
            var workflowId = firstOrDefault.ID;

            var target = new WorkflowRepository(context);
            var result = target.Get(workflowId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ID == workflowId);
        }






        [TestMethod]
        public void WorkflowRepository_GetAllWorkflows_Call_GetResults_Test()
        {
            var context = new  YouxelVacation();
            var expectedCount = context.Workflow.Count();

            var target = new WorkflowRepository(context);
            var results = target.GetAll().ToList();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
        }









        [TestMethod]
        public void WorkflowRepository_AddWorkflow_Added_NotFail_Test()
        {
            var context = new  YouxelVacation();
            var expected = context.Workflow.Count() + 1;

            var target = new WorkflowRepository(context);

            var firstOrDefault = context.Employee.FirstOrDefault();
            var team = context.Team.FirstOrDefault();
            if (firstOrDefault != null)
            {
                if (team != null)
                {
                    var teamId = team.ID;

                    var employeeId = firstOrDefault.ID;
                    var workflowId = context.Team.Select(e => e.ID).Max() + 1;
                    var workflow = new Workflow()
                    {
                        ID = workflowId,
                 
                        EmployeeID = employeeId,
                       LevelNum = 4,
                    };
                    target.Add(workflow , teamId );
                }

                var actual = context.Workflow.Count();
                Assert.AreEqual(expected, actual);
            }
        }




        [TestMethod]
        public void WorkflowRepository_UpdateWorkflow_NotFail_Test()
        {
            var context = new YouxelVacation(); /*for target */
            var workflow = context.Workflow.FirstOrDefault();
            var target = new WorkflowRepository(context);

            var context2 = new YouxelVacation(); /*expected to be */
            var firstOrDefault = context2.Employee.FirstOrDefault();
            if (firstOrDefault != null)
                if (workflow!= null) workflow.EmployeeID = firstOrDefault.ID;
            target.Update(workflow , firstOrDefault.ID  , workflow.TeamID);

            if (workflow != null)
            {
                var actual = target.Get(workflow.ID);

                Assert.AreEqual(workflow.EmployeeID, actual.EmployeeID);
            }
        }











    }
}
