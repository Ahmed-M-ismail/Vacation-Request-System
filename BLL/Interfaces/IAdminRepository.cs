using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BOL;

namespace BLL.Interfaces
{
    public interface IAdminRepository : IDisposable
    {

     
        Employee Get(string employeeId);
     
       bool  GetByEmail(string emailId);


        IEnumerable<Employee> GetAll();

        string Add(Employee employee);


  


        void Update(Employee employee);

   
        void Delete(string employeeId);

    
   


        void UpdateEmail(string mail , string pass);


        void UpdateGeneralBalance(int balance);

   



        string getYouxelEmail();
        string getYouxelPass();

        string getGeneralBalance();




    }
}










    



