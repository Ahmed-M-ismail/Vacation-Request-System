using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using BLL.Interfaces;
using BLL.Services;
using BOL;

namespace BLL.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private readonly YouxelVacation _context;
        private readonly ISendEmail _SendEmail;
        private readonly IGetMd5Hash _getMd5Hash;


        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>
        /// <param name="sendEmail"></param>
        public EmployeeRepository(YouxelVacation context, ISendEmail sendEmail, IGetMd5Hash md5Hash)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _SendEmail = sendEmail;
            _getMd5Hash = md5Hash;
        }




        /// <summary>
        /// Logins the specified emp.
        /// </summary>
        /// <param name="emp">The emp.</param>
        /// <returns>employee data</returns>
        public Employee Login(Employee emp)
        {




            using (MD5 md5Hash = MD5.Create())
            {
                emp.Password = _getMd5Hash.Md5Hash(md5Hash, emp.Password);
            }



            var encryptpass =
                _context.Employee.Where(x => x.Email.Equals(emp.Email) && x.AccountActive.Equals(true)).Select(x => x.Password).FirstOrDefault();


            if (encryptpass != null)
            {


                if (encryptpass == emp.Password)
                {

                    Employee Result =
                        _context.Employee
                            .FirstOrDefault(x => x.Email.Equals(emp.Email));
                    if (Result == null) throw new ArgumentNullException(nameof(Result));

                    return Result;
                }

                else
                {
                    return null;


                }




            }

            else
            {


                return null;
            }


        }







        public bool LostPassword(string email)
        {
            var employee = _context.Employee.FirstOrDefault(x => x.Email.Equals(email));

            if (employee == null)
            {
                return false;
            }

            Random rnd = new Random();
            string decrypttedpass = rnd.Next(100000000).ToString();

            using (MD5 md5Hash = MD5.Create())
            {
                employee.Password = _getMd5Hash.Md5Hash(md5Hash, decrypttedpass);
            }





            employee.LastUpdated = DateTime.Now;

            _context.Entry(employee).Property(x => x.FName).IsModified = false;
            _context.Entry(employee).Property(x => x.LName).IsModified = false;
            _context.Entry(employee).Property(x => x.JobTittle).IsModified = false;
            _context.Entry(employee).Property(x => x.DateHired).IsModified = false;
            _context.Entry(employee).Property(x => x.Email).IsModified = false;
            _context.Entry(employee).Property(x => x.UserRoleID).IsModified = false;
            _context.Entry(employee).Property(x => x.VacationBalance).IsModified = false;
            _context.Entry(employee).Property(x => x.CreatedAT).IsModified = false;
            _context.Entry(employee).Property(x => x.BalanceLimit).IsModified = false;
            _context.Entry(employee).Property(x => x.AccountActive).IsModified = false;
            _context.Entry(employee).State = EntityState.Modified;
            _context.SaveChanges();


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail = _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass = _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();


            return _SendEmail.SendingEmail("Your password", string.Format(body, "Youxel", "Admin", "Your new  password is \" " + decrypttedpass + " \"    you can change it later "), email, adminEmail, adminPass);
        }


        public int emptype()
        {


            var empID =
            (string)HttpContext.Current.Session["LogedUserID"];
            var teamId =
         _context.Workflow.Where(x => x.EmployeeID == empID).Select(x => x.TeamID).ToList();

            int levelnum = /*level el logined user */
               _context.Workflow.Where(x => teamId.Contains(x.TeamID) && x.EmployeeID == empID).Select(x => x.LevelNum).FirstOrDefault();








            int numlevel = /*level el team's user */
               _context.Team.Where(x => teamId.Contains(x.ID)).Select(x => x.NumLevel).FirstOrDefault();






            if (levelnum == 1)
            {
                return 1;

            }

            if (levelnum != 1 && levelnum != numlevel)
            {
                return 2;

            }


            if (levelnum == numlevel && numlevel != 0 && levelnum != 0)
            {
                return 3;
            }

            return 4;
        }






        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <param name="empid">The empid.</param>
        public void ChangePassword(Employee employee, string empid)
        {
            Employee emp = _context.Employee.FirstOrDefault(g => g.ID == empid);



            if (emp != null)
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    emp.Password = _getMd5Hash.Md5Hash(md5Hash, employee.Password);
                }

                emp.LastUpdated = DateTime.Now;
                _context.Entry(emp).State = EntityState.Modified;



                _context.Entry(emp).Property(x => x.FName).IsModified = false;
                _context.Entry(emp).Property(x => x.LName).IsModified = false;
                _context.Entry(emp).Property(x => x.Email).IsModified = false;
                _context.Entry(emp).Property(x => x.DateHired).IsModified = false;
                _context.Entry(emp).Property(x => x.AccountActive).IsModified = false;
                _context.Entry(emp).Property(x => x.BalanceLimit).IsModified = false;
                _context.Entry(emp).Property(x => x.JobTittle).IsModified = false;
                _context.Entry(emp).Property(x => x.UserRoleID).IsModified = false;
                _context.Entry(emp).Property(x => x.VacationBalance).IsModified = false;
                _context.Entry(emp).Property(x => x.CreatedAT).IsModified = false;
            }


            _context.SaveChanges();
        }




























        /// <summary>
        /// Dispose all resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose all resource
        /// </summary>
        /// <param name="disposing">Dispose managed resources check</param>
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
