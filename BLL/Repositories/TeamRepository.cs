using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using BLL.Interfaces;
using BOL;


namespace BLL.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly YouxelVacation _context;


        /// <summary>
        /// Initializes a new instance of the <see cref="TeamRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public TeamRepository(YouxelVacation context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }









        /// <summary>
        /// Get team by Id
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns>team</returns>
        public Team Get(string teamId)
        {
            return _context.Team.SingleOrDefault(q => q.ID == teamId);
        }


        /// <summary>
        /// Get All teams
        /// </summary>
        /// <returns>
        /// List of teams
        /// </returns>
        public IEnumerable<Team> GetAll()
        {
            return _context.Team.ToList();
        }

        /// <summary>
        /// Add new team
        /// </summary>
        /// <param name="team">team information</param>
        /// <returns>
        /// teamId
        /// </returns>
        public string Add(Team team)
        {

            var listofteams = _context.Team.ToList().OrderByDescending(x => x.ID);

            if (!listofteams.Any())
            {
                team.ID = "Team-0001";

            }

            else
            {

                var originalValue = listofteams.Select(x => x.ID).FirstOrDefault();

                // Get the numeric part (0002)
                var stringValue = Regex.Match(originalValue, @"\d+").Value;
                // Convert to int
                var intValue = Int32.Parse(stringValue);
                // Increase
                intValue++;
                // Convert back to string
                originalValue = "Team-" + intValue.ToString("D4");
                team.ID = originalValue;

            }
            _context.Team.Add(team);

            var emp = _context.Employee.SingleOrDefault(x => x.ID.Equals(team.ManagerID));
            emp.LastUpdated = DateTime.Now;
            emp.UserRoleID = "4";

            _context.Entry(emp).State = EntityState.Modified;



            _context.Entry(emp).Property(x => x.FName).IsModified = false;
            _context.Entry(emp).Property(x => x.LName).IsModified = false;
            _context.Entry(emp).Property(x => x.Email).IsModified = false;
            _context.Entry(emp).Property(x => x.DateHired).IsModified = false;
            _context.Entry(emp).Property(x => x.AccountActive).IsModified = false;
            _context.Entry(emp).Property(x => x.BalanceLimit).IsModified = false;
            _context.Entry(emp).Property(x => x.JobTittle).IsModified = false;
            _context.Entry(emp).Property(x => x.Password).IsModified = false;
            _context.Entry(emp).Property(x => x.VacationBalance).IsModified = false;
            _context.Entry(emp).Property(x => x.CreatedAT).IsModified = false;
            _context.Entry(emp)
                 .State = EntityState.Modified;



            _context.SaveChanges();
            return team.ID;
        }


        /// <summary>
        /// Update team
        /// </summary>
        /// <param name="team">team information</param>
        public void Update(Team team)

        {

            var emp = _context.Employee.SingleOrDefault(x => x.ID.Equals(team.ManagerID));
            if (emp != null)
            {
                emp.LastUpdated = DateTime.Now;
                emp.UserRoleID = "4";

                _context.Entry(emp).State = EntityState.Modified;



                _context.Entry(emp).Property(x => x.FName).IsModified = false;
                _context.Entry(emp).Property(x => x.LName).IsModified = false;
                _context.Entry(emp).Property(x => x.Email).IsModified = false;
                _context.Entry(emp).Property(x => x.DateHired).IsModified = false;
                _context.Entry(emp).Property(x => x.AccountActive).IsModified = false;
                _context.Entry(emp).Property(x => x.BalanceLimit).IsModified = false;
                _context.Entry(emp).Property(x => x.JobTittle).IsModified = false;
                _context.Entry(emp).Property(x => x.Password).IsModified = false;
                _context.Entry(emp).Property(x => x.VacationBalance).IsModified = false;
                _context.Entry(emp).Property(x => x.CreatedAT).IsModified = false;
                _context.Entry(emp)
                    .State = EntityState.Modified;
            }


            _context.Entry(team)
                    .State = EntityState.Modified;

            _context.SaveChanges();
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


