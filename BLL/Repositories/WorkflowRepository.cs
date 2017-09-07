using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using BLL.Interfaces;
using  BOL;

namespace BLL.Repositories
{
    public class WorkflowRepository: IWorkflowRepository
    {
        private readonly YouxelVacation _context;


        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public WorkflowRepository( YouxelVacation context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }









        /// <summary>
        /// Gets the specified workflow identifier.
        /// </summary>
        /// <param name="workflowId">The workflow identifier.</param>
        /// <returns>workflow</returns>
        public Workflow Get(string workflowId)
        {
            return _context.Workflow.SingleOrDefault(q => q.ID == workflowId);
        }


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>list of workflow </returns>
        public IEnumerable<Workflow> GetAll()
        {
            return _context.Workflow.ToList().OrderBy(x => x.TeamID).ThenBy(x => x.LevelNum);
        }


        public IEnumerable<Workflow>GetByTeamID (string teamId)
        {
            return _context.Workflow.Where(x=>x.TeamID.Equals(teamId)).ToList().OrderBy(x=>x.LevelNum);
        }

        /// <summary>
        /// Adds the specified workflow.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        public void Add(Workflow workflow, string teamid)
        {
            var listofworkflows = _context.Workflow.ToList().OrderByDescending(x => x.ID);

            if (!listofworkflows.Any())
            {

                workflow.ID = "Workflow-0001";

            }

            else
            {

                var originalValue = listofworkflows.Select(x => x.ID).FirstOrDefault();

                // Get the numeric part (0002)
                var stringValue = Regex.Match(originalValue, @"\d+").Value;
                // Convert to int
                var intValue = Int32.Parse(stringValue);
                // Increase
                intValue++;
                // Convert back to string
                originalValue = "Workflow-" + intValue.ToString("D4");
                workflow.ID = originalValue;

            }
            workflow.TeamID = teamid;
            _context.Workflow.Add(workflow);

            _context.SaveChanges();
          
        }


        /// <summary>
        /// Updates the specified workflow.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        /// <param name="userid"></param>
        public void Update( Workflow workflow , string userid , string teamid)

        {


            workflow.TeamID = teamid;
            workflow.EmployeeID = userid;

            _context.Entry(workflow)
                    .State = EntityState.Modified;

            _context.SaveChanges();
        }



        public void delete(string teamid, string userid)
        {



       

            _context.RequestApproval.RemoveRange(_context.RequestApproval
      .Where(g =>
          g.ApprovalBy == userid)
      .ToList());


            _context.Workflow.Remove(_context.Workflow
                .FirstOrDefault(g =>g.TeamID.Equals(teamid) &&  g.EmployeeID.Equals(userid))) ;





            _context.SaveChanges();
        }



        public int   TeamNumLevels(string teamId)
        {
            return _context.Team.Where(x => x.ID.Equals(teamId)).Select(x => x.NumLevel).FirstOrDefault();
        }



        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            GC.SuppressFinalize(this);
        }






    }
}


