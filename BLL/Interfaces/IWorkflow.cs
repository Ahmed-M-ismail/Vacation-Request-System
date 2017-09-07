using System;
using System.Collections.Generic;
using BOL;

namespace BLL.Interfaces
{
    public interface IWorkflowRepository : IDisposable
    {
       
        Workflow Get(string workflowId);



        IEnumerable<Workflow> GetByTeamID(string teamId);

        int TeamNumLevels(string teamId); 

        IEnumerable<Workflow> GetAll();


        void Add(Workflow workflow, string teamid);

      
        void Update(Workflow workflow , string userid , string teaamid);


        void delete(string teamid , string userid);




    }
}














