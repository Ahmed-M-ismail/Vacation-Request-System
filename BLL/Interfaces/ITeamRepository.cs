using System;
using System.Collections.Generic;
using BOL;

namespace BLL.Interfaces
{
    public interface ITeamRepository : IDisposable
    {
        /// <summary>
        /// Get team by Id
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Team Get(string teamId);

        /// <summary>
        /// Get All teams
        /// </summary>
        /// <returns>List of teams</returns>
        IEnumerable<Team> GetAll();

        /// <summary>
        /// Add new team
        /// </summary>
        /// <param name="team">team information</param>
        /// <returns>teamId</returns>
        string Add(Team team);

        /// <summary>
        /// Update team
        /// </summary>
        /// <param name="team">team information</param>
        void Update(Team team);

       


    }
}














