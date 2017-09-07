using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BLL.Interfaces;
using BLL.Services;
using BOL;

namespace BLL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly YouxelVacation _context;
        private readonly IGetMd5Hash _getMd5Hash;


        public AdminRepository(YouxelVacation context, IGetMd5Hash md5Hash)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _getMd5Hash = md5Hash;
        }






        /// <summary>
        /// Gets the specified employee identifier.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns>ُEmployee</returns>
        public Employee Get(string employeeId)
        {
            return _context.Employee.SingleOrDefault(q => q.ID == employeeId);
        }


        /// <summary>
        /// Gets the employee  by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public bool GetByEmail(string email)
        {
            var useremail = _context.Employee.SingleOrDefault(q => q.Email == email);

            var status = useremail == null;

            return status;
        }


        /// <summary>
        /// Gets all employees.
        /// </summary>
        /// <returns>list of emoloyees</returns>
        public IEnumerable<Employee> GetAll()
        {
            return _context.Employee.Where(x => x.AccountActive).ToList();
        }

        /// <summary>
        /// Adds the specified employee.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns>emoloyee ID</returns>
        public string Add(Employee employee)
        {


            var listofemp = _context.Employee.ToList().OrderByDescending(x => x.ID);

            if (!listofemp.Any())
            {
                employee.ID = "User-0001";

            }
            else
            {

                var originalValue = listofemp.Select(x => x.ID).FirstOrDefault();

                // Get the numeric part (0002)
                if (originalValue != null)
                {
                    var stringValue = Regex.Match(originalValue, @"\d+").Value;
                    // Convert to int
                    var intValue = Int32.Parse(stringValue);
                    // Increase
                    intValue++;
                    // Convert back to string
                    originalValue = "User-" + intValue.ToString("D4");
                }
                employee.ID = originalValue;

            }
            employee.CreatedAT = DateTime.Now;
            employee.LastUpdated = DateTime.Now;
            employee.AccountActive = true;

            if (employee.VacationBalance == null)
            {
                employee.VacationBalance =
                     Convert.ToInt32(_context.Configuration.Where(x => x.ID.Equals("GeneralBalance"))
                        .Select(x => x.Value)
                        .FirstOrDefault());
            }


            if (employee.BalanceLimit == null)
            {
                employee.BalanceLimit =
                     Convert.ToInt32(_context.Configuration.Where(x => x.ID.Equals("GeneralBalance"))
                        .Select(x => x.Value)
                        .FirstOrDefault());
            }



            using (MD5 md5Hash = MD5.Create())
            {
                employee.Password = _getMd5Hash.Md5Hash(md5Hash, employee.Password);
            }

            _context.Employee.Add(employee);

            _context.SaveChanges();
            return employee.ID;
        }


        /// <summary>
        /// Updates the specified employee.
        /// </summary>
        /// <param name="employee">The employee.</param>
        public void Update(Employee employee)

        {



            employee.LastUpdated = DateTime.Now;


            if (employee.VacationBalance == null)
            {
                employee.VacationBalance =
                     Convert.ToInt32(_context.Configuration.Where(x => x.ID.Equals("GeneralBalance"))
                        .Select(x => x.Value)
                        .FirstOrDefault());
            }


            if (employee.BalanceLimit == null)
            {
                employee.BalanceLimit =
                     Convert.ToInt32(_context.Configuration.Where(x => x.ID.Equals("GeneralBalance"))
                        .Select(x => x.Value)
                        .FirstOrDefault());
            }


            _context.Entry<Employee>(employee)
                    .State = EntityState.Modified;

            _context.Entry(employee).Property(x => x.AccountActive).IsModified = false;
            _context.Entry(employee).Property(x => x.Password).IsModified = false;

            _context.SaveChanges();
        }




        /// <summary>
        /// Deletes the specified employee identifier.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        public void Delete(string employeeId)
        {
            var employee = _context.Employee
                .Find(employeeId);

            if (employee != null)
            {
                _context.Entry(employee).Property(x => x.FName).IsModified = false;
                _context.Entry(employee).Property(x => x.LName).IsModified = false;
                _context.Entry(employee).Property(x => x.JobTittle).IsModified = false;
                _context.Entry(employee).Property(x => x.DateHired).IsModified = false;
                _context.Entry(employee).Property(x => x.Password).IsModified = false;
                _context.Entry(employee).Property(x => x.Email).IsModified = false;
                _context.Entry(employee).Property(x => x.UserRoleID).IsModified = false;
                _context.Entry(employee).Property(x => x.VacationBalance).IsModified = false;
                _context.Entry(employee).Property(x => x.BalanceLimit).IsModified = false;
                _context.Entry(employee).Property(x => x.CreatedAT).IsModified = false;

                employee.LastUpdated = DateTime.Now;
                employee.AccountActive = false;

                _context.Entry<Employee>(employee)
                        .State = EntityState.Modified;
                _context.SaveChanges();
            }
        }













        public void UpdateEmail(string mail, string pass)
        {


            var adminEmail = _context.Configuration.FirstOrDefault(x => x.ID.Equals("AdminEmail"));

            var adminPass = _context.Configuration.FirstOrDefault(x => x.ID.Equals("AdminPass"));


            if (adminEmail != null)
            {
                adminEmail.Value = mail;
                if (adminPass != null) adminPass.Value = pass;
                _context.Entry<Configuration>(adminEmail)
                    .State = EntityState.Modified;
            }


            _context.Entry<Configuration>(adminPass)
                .State = EntityState.Modified;



            _context.SaveChanges();


        }





        /// <summary>
        /// Updates the general balance.
        /// </summary>
        /// <param name="balance">The balance.</param>
        public void UpdateGeneralBalance(int balance)
        {

            var generalBalance = _context.Configuration.FirstOrDefault(x => x.ID.Equals("GeneralBalance"));


            if (generalBalance != null)
            {
                generalBalance.Value = Convert.ToString(balance);




                _context.Entry(generalBalance)
                    .State = EntityState.Modified;
            }


            _context.SaveChanges();


        }




        public string getYouxelEmail()
        {

            return _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

        }


        public string getYouxelPass()
        {
            return _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();


        }


        public string getGeneralBalance()
        {
            return _context.Configuration.Where(x => x.ID.Equals("GeneralBalance")).Select(x => x.Value).FirstOrDefault();

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




















