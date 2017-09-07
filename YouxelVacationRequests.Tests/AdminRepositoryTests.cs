using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BLL.Interfaces;
using BLL.Repositories;
using BLL.Services;
using BOL;

namespace YouxelVacationRequests.Tests
{
    [TestClass]
    public class AdminRepositoryTests
    {

        private readonly IGetMd5Hash GetMd5Hash;

        [TestMethod]
        public void AdminRepository_GetEmployee_Call_GetResults_Test()
        {
            var context = new YouxelVacation();
            var employee = context.Employee.FirstOrDefault();
            if (employee == null) return;
            var  employeeId = employee.ID;
            var target = new AdminRepository(context , GetMd5Hash);
            var result = target.Get(employeeId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ID == employeeId);
        }





      





        [TestMethod]
        public void AdminRepository_GetAllEmployees_Call_GetResults_Test()
        {
            var context = new YouxelVacation();
            var expectedCount = context.Employee.Count();

            var target = new AdminRepository(context  , GetMd5Hash);
            var results = target.GetAll().ToList();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
        }














        [TestMethod]
        public void EmployeeRepository_UpdateEmployee_NotFail_Test()
        {
            var context = new  YouxelVacation();
            var employee = context.Employee.FirstOrDefault();
            var target = new AdminRepository(context , GetMd5Hash);

            if (employee != null)
            {
                employee.FName = Guid.NewGuid().ToString(); /**/
                target.Update(employee);

                var actual = target.Get(employee.ID);

                Assert.AreEqual(employee.FName, actual.FName);
            }


         
        }






        [TestMethod]
        public void EmployeeRepository_DeleteEmployee_Deleted_NotFail_Test()
        {
            var context = new  YouxelVacation();
            var employee = context.Employee.FirstOrDefault();
        

            IAdminRepository target = new AdminRepository(context , GetMd5Hash);
            if (employee != null) target.Delete(employee.ID);

            if (employee == null) return;
            var actual = target.Get(employee.ID);

            Assert.AreEqual(employee.AccountActive, actual.AccountActive);

        }














    }
}