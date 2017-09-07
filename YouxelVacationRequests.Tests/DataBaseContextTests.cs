using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BLL;
using BLL.Repositories;
using BOL;


namespace YouxelVacationRequests.Tests
{
    [TestClass]
    public class DatabaseContextTests
    {
        [TestMethod]
        public void DatabaseContext()
        {
            var context = new  YouxelVacation();
            context.Database.Initialize(true);
        }
    }
}