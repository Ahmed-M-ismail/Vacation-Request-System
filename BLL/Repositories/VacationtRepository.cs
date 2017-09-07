using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using BLL.Interfaces;
using BLL.Services;
using BOL;


namespace BLL.Repositories
{
    public class VacationRepository : IVacationRepository
    {


        private readonly /*  mean that  you can't  change it from another part of that class after it is initialized. The readonly modifier ensures the field can only be given a value during its initialization or in its class constructor.*/ YouxelVacation _context;
        private readonly IWorkingDaysCalculator _workingDaysCalculator;
        private readonly INotificationRepository _notificationRepository;
        private readonly ISendEmail _SendEmail;



        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>


        public VacationRepository(YouxelVacation context, IWorkingDaysCalculator workingDaysCalculator, ISendEmail sendEmail)
        {

            _workingDaysCalculator = workingDaysCalculator;/*dependency injection : inject dependent classes inside constructor to be the initialize responsiblity for the class how call VacationRepository */

            if (context == null)
                throw new ArgumentNullException("context");




            _context = context;
            _workingDaysCalculator = workingDaysCalculator;
            _notificationRepository = new NotificationRepository(_context);
            _SendEmail = sendEmail;
        }



        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Request Get(string id, string userId)
        {
            return _context.Request.SingleOrDefault(x => x.ID.Equals(id) && x.EmployeeID.Equals(userId));

        }


        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Request GetById(string id)
        {
            return _context.Request.SingleOrDefault(x => x.ID.Equals(id));

        }




        /// <summary>
        /// Adds the vacation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>true or false </returns>
        public int AddVacation(Request request)
        {





            var empID =
               (string)HttpContext.Current.Session["LogedUserID"];



            var fromvacations = /*select  start date for all  logined user's vacations which is not rejected */
                _context.Request.Where(x => x.EmployeeID.Equals(empID) &&
                (x.RequestStatusID.Equals("1") || x.RequestStatusID.Equals("2"))).Select(x => x.DurationFrom).ToList();




            var tovacations =/*select  start date for all  logined user's vacations which is not rejected */
                          _context.Request.Where(x => x.EmployeeID.Equals(empID) && (x.RequestStatusID.Equals("1") || x.RequestStatusID.Equals("2"))).Select(x => x.DurationTo).ToList();



            if (request.DurationFrom <= fromvacations.Max() || request.DurationTo <= tovacations.Max())
            { /*if this vacation period in between or before another vaation this user had requested before , then return false */
                return 0;

            }


            var teamId =
         _context.Workflow.Where(x => x.EmployeeID == empID).Select(x => x.TeamID).ToList();

            int levelnum = /*level el logined user */
               _context.Workflow.Where(x => teamId.Contains(x.TeamID) && x.EmployeeID == empID).Select(x => x.LevelNum).FirstOrDefault();








            int numlevel = /*level el team's user */
               _context.Team.Where(x => teamId.Contains(x.ID)).Select(x => x.NumLevel).FirstOrDefault();








            var teamMembers /*all members in my teams */ = _context.Workflow.Where(x => teamId.Contains(x.TeamID)).Select(x => x.EmployeeID).ToList();





            var lowermanager /*the first managers bottom of logined user */  = _context.Workflow.Where(x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum - 1)).Select(x => x.Employee.Email).FirstOrDefault();


            var lowerlowermanager =/*the manager of the manger of the logined user */
        _context.Workflow.Where(
            x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum - 2))
            .Select(x => x.EmployeeID)
            .FirstOrDefault();




            var hisDirectManagetHasVacation = /*if direct manager for logined user has a vacation*/
 _context.Request
     .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
         x.Employee.Email.Equals(lowermanager));



            var hisSecondDirectManagetHasVacation = /*if direct manager for the direct manager for  logined user has a vacation*/
           _context.Request
               .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
                   x.EmployeeID.Equals(lowerlowermanager));


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail = _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass = _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();






            Request Vacation = new Request();
            var listofvc = _context.Request.ToList().OrderByDescending(x => x.ID);




            if (!listofvc.Any())
            {
                Vacation.ID = "Re-000001";
            }
            else
            {
                var originalValue = listofvc.Select(x => x.ID).FirstOrDefault();

                // Get the numeric part (0002)
                var stringValue = Regex.Match(originalValue, @"\d+").Value;
                // Convert to int
                var intValue = Int32.Parse(stringValue);
                // Increase
                intValue++;
                // Convert back to string
                originalValue = "Re-" + intValue.ToString("D6");




                Vacation.ID = originalValue;

            }
            string comment = (string)HttpContext.Current.Session["LogedUserFname"] + "  " + (string)HttpContext.Current.Session["LogedUserLname"] + "  " + "Comment : " + request.Comment + "  ,            ";
            Vacation.Comment = comment;



            Vacation.DurationFrom = request.DurationFrom;
            Vacation.DurationTo = request.DurationTo;



            // Difference in days.

            Vacation.CountDays = _workingDaysCalculator.GetWorkingDays(request.DurationFrom, request.DurationTo);







            var userID = (string)HttpContext.Current.Session["LogedUserID"];

            if (levelnum == 1 || (levelnum == 2 && hisDirectManagetHasVacation != null) || (levelnum == 3 && hisDirectManagetHasVacation != null && hisSecondDirectManagetHasVacation != null))
            {
                Vacation.RequestStatusID = "2";




            }

            else
            {

                Vacation.RequestStatusID = "1";
            }


            Vacation.VacationORWorkHome = true;
            Vacation.VacationTypeID = (string)HttpContext.Current.Session["VacationType"];
            Vacation.EmployeeID = userID;
            Vacation.CreatedAT = DateTime.Now;
            Vacation.LastModified = DateTime.Now;






            var pendingVacations = _context.Request.Where(
                  x => x.RequestStatusID.Equals("1") && x.VacationORWorkHome.Equals(true) && x.EmployeeID.Equals(userID) && x.VacationTypeID.Equals("2"))
                  .Select(x => x.CountDays)
                  .Sum();


            if (Convert.ToInt32(Vacation.VacationTypeID) == 2 &&
                (int)HttpContext.Current.Session["VacationBalance"] < (Vacation.CountDays + pendingVacations))
            {


                return 1;

            }


            else
            {
                _context.Request.Add(Vacation);

                _context.SaveChanges();


                if (levelnum != 1)
                {




                    _SendEmail.SendingEmail("vacation request",
                        string.Format(body, "Youxel", "Admin",
                            "" + (string)HttpContext.Current.Session["LogedUserFname"] + "  " +
                            (string)HttpContext.Current.Session["LogedUserLname"] +
                            "   request a  new vacation (" + Vacation.CountDays + " ) days from " + Vacation.DurationFrom.Value.ToString("yyyy-MM-dd") + "       to" + "    " + Vacation.DurationTo.Value.ToString("yyyy-MM-dd") + ", navigate to  the system for more details. "), lowermanager,
                        adminEmail, adminPass);



                    return 2;
                }

                return 2;
            }





        }

        /// <summary>
        /// Shows the vaction history.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns>list of requests </returns>
        public IEnumerable<Request> ShowVactionHistory(string employeeId)
        {

            return _context.Request
              .Where(g =>
                  g.EmployeeID == employeeId && g.VacationORWorkHome == true)
              .ToList();
        }



















        /// <summary>
        /// Managers the show requests.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns>list of requestd </returns>
        public IEnumerable<Request> ManagerShowRequests(string employeeId)
        {


            if ((string)HttpContext.Current.Session["UserTypeID"] == "2")
            {


                return
                    _context.Request
                        .Where(x => x.VacationORWorkHome.Equals(true))
                        .ToList();

            }











            if ((string)HttpContext.Current.Session["UserTypeID"] == "3")
            {


                return
                    _context.Request.Where(x => x.RequestStatusID == "2" || x.RequestStatusID == "3")
                        .Where(x => x.VacationORWorkHome.Equals(true))
                        .ToList();

            }















            var teamId =
                _context.Workflow.Where(x => x.EmployeeID == employeeId).Select(x => x.TeamID).ToList();

            int levelnum = /*level el logined user */
                _context.Workflow.Where(x => teamId.Contains(x.TeamID) && x.EmployeeID == employeeId)
                    .Select(x => x.LevelNum)
                    .FirstOrDefault();








            int numlevel = /*level el team's user */
                _context.Team.Where(x => teamId.Contains(x.ID)).Select(x => x.NumLevel).FirstOrDefault();

            var requestsHaveSeen /*requests which logined user  took decion on it */ =
                _context.RequestApproval.Where(x => x.ApprovalBy == employeeId).Select(x => x.RequestID).ToList();


            var teamMembers /*all members in my teams */ =
                _context.Workflow.Where(x => teamId.Contains(x.TeamID)).Select(x => x.EmployeeID).ToList();

            var teamworkflow /*my team members and bottom of me  */ =
                _context.Workflow.Where(
                    x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum > levelnum)
                    .Select(x => x.EmployeeID)
                    .ToList();





            var lowermanager /*the first managers bottom of logined user */  =
                _context.Workflow.Where(
                    x => teamMembers.Contains(x.EmployeeID) && x.LevelNum != numlevel && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum + 1))
                    .Select(x => x.EmployeeID)
                    .FirstOrDefault();


            var lowerlowermanager =
            _context.Workflow.Where(
                x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum + 2))
                .Select(x => x.EmployeeID)
                .FirstOrDefault();


            /*if lower manager have vacation then i will replace him  */







            var hisDirectManagetHasVacation = /*if direct manager for logined user has a vacation*/
                _context.Request
                    .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
                        x.EmployeeID.Equals(lowermanager));



            var hisSecondDirectManagetHasVacation = /*if direct manager for logined user has a vacation*/
              _context.Request
                  .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
                      x.EmployeeID.Equals(lowerlowermanager));

            if (hisDirectManagetHasVacation != null)
            {
                var firstapproval = _context.Request.Where(x => /*approvalforlastmanager */
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(true) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).ToList();


                var used = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(true) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).Select(x => x.ID).ToList();



                var lowerlist /*all request from first managers bottom of logined user */ = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(true) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID) && lowerlowermanager.Contains(x.EmployeeID)).ToList();




                var /*list of the IDS of  requests approved by all bottom managers*/
                    requestapprovedbybottommanagers =
                        _context.RequestApproval.Where(
                            x =>
                                used.Contains(x.RequestID) && x.Workflow.LevelNum.Equals(levelnum + 2) &&
                                x.StatusID == "2").Select(x => x.RequestID).ToList();


                var /*list of all requests approved by all bottom managers*/
                    totalrequests = _context.Request.Where(x => requestapprovedbybottommanagers.Contains(x.ID));

                var result = Enumerable.Concat(totalrequests, lowerlist);
                /*merge all request by lower manager and all request approved by lower manager */



                if (levelnum == numlevel - 1 || hisSecondDirectManagetHasVacation != null)
                {

                    return firstapproval;

                }

                return result;
            }


            else
            {

                var firstapproval = _context.Request.Where(x => /*approvalforlastmanager */
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(true) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).ToList();


                var used = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(true) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).Select(x => x.ID).ToList();



                var lowerlist /*all request from first managers bottom of logined user */ = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(true) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID) && lowermanager.Contains(x.EmployeeID)).ToList();




                var /*list of the IDS of  requests approved by all bottom managers*/
                    requestapprovedbybottommanagers =
                        _context.RequestApproval.Where(
                            x =>
                                used.Contains(x.RequestID) && x.Workflow.LevelNum.Equals(levelnum + 1) &&
                                x.StatusID == "2").Select(x => x.RequestID).ToList();


                var /*list of all requests approved by all bottom managers*/
                    totalrequests = _context.Request.Where(x => requestapprovedbybottommanagers.Contains(x.ID));

                var result = Enumerable.Concat(totalrequests, lowerlist);
                /*merge all request by lower manager and all request approved by lower manager */


                return levelnum == numlevel - 1 ? firstapproval : result;


            }
        }
















        public IEnumerable<Notifications> ManagerShownotifications(string employeeId)
        {


            var notifications =
                _context.Notifications.Where(x => x.seen == false && x.EmployeeID == employeeId).ToList();



            foreach (var notify in notifications)
            {

                notify.seen = true;

                _context.Entry(notify).State = EntityState.Modified;



                _context.Entry(notify).Property(x => x.CreatedAT).IsModified = false;
                _context.Entry(notify).Property(x => x.LastUpdated).IsModified = false;
                _context.Entry(notify).Property(x => x.text).IsModified = false;
                _context.Entry(notify).Property(x => x.EmployeeID).IsModified = false;
                _context.SaveChanges();

            }


            return notifications;

        }



        public int numOfNotifications(string employeeId)
        {

            return _context.Notifications.Count(x => x.EmployeeID.Equals(employeeId) && x.seen.Equals(false));

        }




        /// <summary>
        /// Updates the vacation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>true or false </returns>
        public bool UpdateVacation(Request request)
        {
            Request Vacation = new Request();

            Vacation.ID = request.ID;

            var userID = (string)HttpContext.Current.Session["LogedUserID"];



            var oldStatusID = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.RequestStatusID).FirstOrDefault();
            var oldVacationType = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.VacationTypeID).FirstOrDefault();

            var oldEmpID = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.EmployeeID).FirstOrDefault();



            var oldcountDays = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.CountDays).FirstOrDefault();


            var userinfo = _context.Employee.FirstOrDefault(x => x.ID.Equals(oldEmpID));





            Vacation.DurationFrom = request.DurationFrom;
            Vacation.DurationTo = request.DurationTo;


            Vacation.CountDays = _workingDaysCalculator.GetWorkingDays(request.DurationFrom, request.DurationTo);


            Vacation.VacationORWorkHome = true;


            Vacation.VacationTypeID = (string)HttpContext.Current.Session["VacationType"];

            string comment =
                (string)HttpContext.Current.Session["LogedUserFname"] + "  " +
                (string)HttpContext.Current.Session["LogedUserLname"] + "  " + "Comment : " +
                request.Comment + "  ,            ";
            Vacation.Comment = comment;



            Vacation.LastModified = DateTime.Now;




            var pendingVacations = _context.Request.Where(
                x =>
                    x.RequestStatusID.Equals("1") && x.VacationORWorkHome.Equals(true) && x.EmployeeID.Equals(userID) &&
                    x.VacationTypeID.Equals("2"))
                .Select(x => x.CountDays)
                .Sum();

            if (Convert.ToInt32(Vacation.VacationTypeID) == 2 &&
                (int)HttpContext.Current.Session["VacationBalance"] < (Vacation.CountDays + pendingVacations))
            {

                return false;
            }

            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail =
                _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass =
                _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();





            if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) == "2")
            {


                if (userinfo != null)
                {
                    userinfo.LastUpdated = DateTime.Now;
                    if (/*if old status was  Non Paid and  new one  is Annual Vacation, then subtract new duration from user balance  */
                        oldStatusID == "2" && oldVacationType == "1" && Vacation.VacationTypeID ==
                        "2")
                    {
                        userinfo.VacationBalance = (int)(userinfo.VacationBalance - Vacation.CountDays);
                        if (userinfo.VacationBalance < 0)
                            return false;
                    }


                    if (/*if old status was  Annual Vacation and  new one  is Annual Vacation, then subtract new duration from user balance  and add old duration to balance  */
                        oldStatusID == "2" && oldVacationType == "2" && Vacation.VacationTypeID ==
                        "2")

                    {
                        userinfo.VacationBalance = (int)(userinfo.VacationBalance + oldcountDays - Vacation.CountDays);

                        if (userinfo.VacationBalance < 0)
                            return false;

                    }



                    if (/*if old status was  Annual Vacation and  new one  is  Non Paid, then  add old duration to balance  */
                        oldStatusID == "2" && oldVacationType == "2" && Vacation.VacationTypeID ==
                        "1")
                    {

                        userinfo.VacationBalance = (int)(userinfo.VacationBalance + oldcountDays);
                    }




                    _context.Entry(userinfo).Property(x => x.FName).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.LName).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.JobTittle).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.DateHired).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.Password).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.Email).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.UserRoleID).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.BalanceLimit).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.AccountActive).IsModified = false;
                    _context.Entry(userinfo).Property(x => x.CreatedAT).IsModified = false;






                    _context.Entry(userinfo).State = EntityState.Modified;



                }



                var createdAt =
                    _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.CreatedAT).FirstOrDefault();
                var employeeId =
                    _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.EmployeeID).FirstOrDefault();



                var EmployeeEmail =
                    _context.Employee.Where(x => x.ID.Equals(employeeId)).Select(x => x.Email).FirstOrDefault();
                var RequestStatusID =
                    _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.RequestStatusID).FirstOrDefault();

                Vacation.CreatedAT = createdAt;
                Vacation.EmployeeID = employeeId;
                Vacation.RequestStatusID = RequestStatusID;




                _context.RequestApproval.RemoveRange(_context.RequestApproval
                    .Where(g =>
                        g.RequestID == request.ID)
                    .ToList());


                _context.Entry<Request>(Vacation)
                    .State = EntityState.Modified;



                _SendEmail.SendingEmail("Vacation  request",
                    string.Format(body, "Youxel", "Admin",
                        "Your vacation request has been updated by admin to be (" + Vacation.CountDays + " ) days from   " +
                        Vacation.DurationFrom.Value.ToString("yyyy-MM-dd") +
                        "     to    " + Vacation.DurationTo.Value.ToString("yyyy-MM-dd") +
                        ", navigate to the system for more details.  "), EmployeeEmail, adminEmail, adminPass);





                try
                {



                    _context.SaveChanges();




                    var IDs =
                        _context.RequestApproval.Where(g => g.RequestID == Vacation.ID)
                            .Select(x => x.ApprovalBy)
                            .ToList();

                    foreach (string id in IDs)
                    {
                        var email =
                            _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                        _SendEmail.SendingEmail("Vacation  request",
                            string.Format(body, "Youxel", "Admin",
                                "vacation request from " + Vacation.Employee.FName + "  " +
                                Vacation.Employee.LName + "  (" + Vacation.CountDays + ")  days from   " + Vacation.DurationFrom.Value.ToString("yyyy-MM-dd") +
                                "     to    " + Vacation.DurationTo.Value.ToString("yyyy-MM-dd") +
                                " has been updated, navigate to the system for more details.  " +
                                Vacation.ID), email, adminEmail, adminPass);
                    }
                }

                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }



                return true;

            }



            else
            {




                Vacation.EmployeeID = (string)HttpContext.Current.Session["LogedUserID"];


                Vacation.RequestStatusID = "1";










                var seenrequests = _context.RequestApproval.Where(x => x.RequestID.Equals(request.ID)).ToList();

                if (seenrequests.Count != 0)
                {




                    var text = "Employee " + (string)HttpContext.Current.Session["LogedUserFname"] + "  " +
                               (string)HttpContext.Current.Session["LogedUserLname"] +
                               "      has updated his vacation request  , request ID :  " + request.ID +
                               "     Go to approval section to show his new request ";

                    _notificationRepository.AddNotification(text, request.ID);


                }


                _context.RequestApproval.RemoveRange(_context.RequestApproval
                    .Where(g =>
                        g.RequestID == request.ID)
                    .ToList());


                _context.Entry<Request>(Vacation)
                    .State = EntityState.Modified;

                _context.Entry(Vacation).Property(x => x.CreatedAT).IsModified = false;



                _context.SaveChanges();

                var IDs =
                          _context.RequestApproval.Where(g => g.RequestID == Vacation.ID)
                              .Select(x => x.ApprovalBy)
                              .ToList();

                foreach (string id in IDs)
                {


                    var email =
                        _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                    _SendEmail.SendingEmail("Vacation  request",
                        string.Format(body, "Youxel", "Admin",
                            "vacation request from " + Vacation.Employee.FName + "  " +
                            Vacation.Employee.LName + "( " + Vacation.CountDays + ") days from   " + Vacation.DurationFrom.Value.ToString("yyyy-MM-dd") +
                           "     to    " + Vacation.DurationTo.Value.ToString("yyyy-MM-dd") +
                            " has been updated, navigate to the system for more details.  " +
                            Vacation.ID), email, adminEmail, adminPass);
                }



                return true;
            }
        }









        /// <summary>
        /// Cancels the vacation.
        /// </summary>
        /// <param name="request">The request.</param>
        public void CancelVacation(Request request)


        {


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail =
                _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass =
                _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();

            var canceledrequest = _context.Request.FirstOrDefault(x => x.ID.Equals(request.ID));
            var seenrequests = _context.RequestApproval.Where(x => x.RequestID.Equals(request.ID)).ToList();

            var EmployeeID = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.EmployeeID).FirstOrDefault();



            var EmployeeEmail = _context.Employee.Where(x => x.ID.Equals(EmployeeID)).Select(x => x.Email).FirstOrDefault();
            if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) == "2")
            {

                if (canceledrequest.RequestStatusID == "2" && canceledrequest.VacationTypeID == "2")
                {
                    var userinfo = _context.Employee.FirstOrDefault(x => x.ID.Equals(canceledrequest.EmployeeID));


                    if (userinfo != null)
                    {
                        userinfo.LastUpdated = DateTime.Now;
                        userinfo.VacationBalance = (int)(userinfo.VacationBalance + canceledrequest.CountDays);

                        _context.Entry(userinfo).Property(x => x.FName).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.LName).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.JobTittle).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.DateHired).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.Password).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.Email).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.UserRoleID).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.BalanceLimit).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.AccountActive).IsModified = false;
                        _context.Entry(userinfo).Property(x => x.CreatedAT).IsModified = false;



                        _context.Entry(userinfo).State = EntityState.Modified;

                    }

                }

                _context.SaveChanges();
                _SendEmail.SendingEmail("Vacation  request",
                    string.Format(body, "Youxel", "Admin",
                        "Your vacation request from  " + canceledrequest.DurationFrom.Value.ToString("yyyy-MM-dd") +
                        "     to    " + canceledrequest.DurationTo.Value.ToString("yyyy-MM-dd") +
                        " has been cancled by admin, navigate to the system for more details.  "),
                    EmployeeEmail, adminEmail, adminPass);




            }

            if (seenrequests.Count != 0)
            {
                if (canceledrequest != null)
                {
                    var text = "vacation request from " + canceledrequest.Employee.FName + "  " + canceledrequest.Employee.LName +
                               " Has been canceld     , request ID :  " + request.ID;
                    _notificationRepository.AddNotification(text, canceledrequest.ID);
                }

















                _context.RequestApproval.RemoveRange(_context.RequestApproval
                        .Where(g =>
                            g.RequestID == request.ID)
                        .ToList());





            }
            _context.Request.Remove(_context.Request
                .FirstOrDefault(g => g.ID == request.ID));


            _context.SaveChanges();

            var IDs =
                       _context.RequestApproval.Where(g => g.RequestID == request.ID)
                           .Select(x => x.ApprovalBy)
                           .ToList();

            foreach (string id in IDs)
            {


                var email =
                    _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                _SendEmail.SendingEmail("Vacation  request",
                    string.Format(body, "Youxel", "Admin",
                        "vacation request from " + request.Employee.FName + "  " +
                        request.Employee.LName + "(" + request.CountDays + ") days from   " + request.DurationFrom.Value.ToString("yyyy-MM-dd") +
                       "     to    " + request.DurationTo.Value.ToString("yyyy-MM-dd") +
                        " has been  canceled, navigate to the system for more details.  " +
                        request.ID), email, adminEmail, adminPass);
            }



        }






        /// <summary>
        /// Approves the vacation.
        /// </summary>
        /// <param name="requestapproval">The requestapproval.</param>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="requestid">The requestid.</param>
        public void ApproveVacation(RequestApproval requestapproval, string employeeId, string requestid)
        {


            Request acceptedRequest = _context.Request.FirstOrDefault(x => x.ID == requestid);


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail =
                _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass =
                _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();

            string workflowId =
                _context.Workflow.Where(x => x.EmployeeID == employeeId).Select(x => x.ID).FirstOrDefault();



            string teamId =
                _context.Workflow.Where(x => x.EmployeeID == employeeId).Select(x => x.TeamID).FirstOrDefault();

            int levelnum = /*level el logined user */
                _context.Workflow.Where(x => x.TeamID == teamId && x.EmployeeID == employeeId)
                    .Select(x => x.LevelNum)
                    .FirstOrDefault();














            int numlevel = /*level el team's user */
                _context.Team.Where(x => teamId.Contains(x.ID)).Select(x => x.NumLevel).FirstOrDefault();






            var teamMembers /*all members in my teams */ =
                _context.Workflow.Where(x => teamId.Contains(x.TeamID)).Select(x => x.EmployeeID).ToList();








            var topmanager /*awl manager fo2 el  logined user */  =
                _context.Workflow.Where(
                    x =>
                        teamMembers.Contains(x.EmployeeID) && x.LevelNum == 1 && x.LevelNum <= numlevel &&
                        x.LevelNum.Equals(levelnum - 1))
                    .Select(x => x.EmployeeID)
                    .FirstOrDefault();

            var adminhasvacation = _context.Request.Where(x => x.EmployeeID.Equals(topmanager) &&
                                                               x.DurationFrom.Value < acceptedRequest.DurationFrom.Value &&
                                                               acceptedRequest.DurationTo.Value < x.DurationTo.Value)
                .FirstOrDefault(); /*awl manager fo2 el  logined user */













            if (levelnum == 1 || adminhasvacation != null ||
                Convert.ToString(HttpContext.Current.Session["UserTypeID"]) == "2")
            /*lw kan el 3aml login awl wa7d fl workflow aw en awl wa7d fl workflow wa5d agaza aw kan admin  */
            {


                if (acceptedRequest != null)
                {
                    acceptedRequest.RequestStatusID = "2";






                    acceptedRequest.LastModified = DateTime.Now;


                    _context.Entry(acceptedRequest).Property(x => x.EmployeeID).IsModified = false;
                    _context.Entry(acceptedRequest).Property(x => x.DurationFrom).IsModified = false;
                    _context.Entry(acceptedRequest).Property(x => x.VacationORWorkHome).IsModified = false;
                    _context.Entry(acceptedRequest).Property(x => x.CreatedAT).IsModified = false;
                    _context.Entry(acceptedRequest).State = EntityState.Modified;
                    if (acceptedRequest.VacationTypeID == "2")
                    {


                        var userinfo = _context.Employee.FirstOrDefault(x => x.ID.Equals(acceptedRequest.EmployeeID));

                        if (userinfo != null)
                        {
                            userinfo.LastUpdated = DateTime.Now;
                            userinfo.VacationBalance = (int)(userinfo.VacationBalance - acceptedRequest.CountDays);

                            _context.Entry(userinfo).Property(x => x.FName).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.LName).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.JobTittle).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.DateHired).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.Password).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.Email).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.UserRoleID).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.BalanceLimit).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.AccountActive).IsModified = false;
                            _context.Entry(userinfo).Property(x => x.CreatedAT).IsModified = false;



                            _context.Entry(userinfo).State = EntityState.Modified;
                        }
                    }


                    if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) != "2")
                    {

                        var text = "vacation request from " + acceptedRequest.Employee.FName + "  " +
                                    acceptedRequest.Employee.LName + "(" + acceptedRequest.CountDays + ") days from   " + acceptedRequest.DurationFrom.Value.ToString("yyyy-MM-dd") +
                                   "     to    " + acceptedRequest.DurationTo.Value.ToString("yyyy-MM-dd") +
                                    " completely approved  ";
                        _notificationRepository.AddNotification(text, acceptedRequest.ID);


                        var allmanagers /*all managers  in my teams */ =
                                      _context.Workflow.Where(x => teamId.Contains(x.TeamID) && x.LevelNum > levelnum).Select(x => x.EmployeeID).ToList();

                        foreach (string id in allmanagers)
                        {

                            var email =
                                  _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                            _SendEmail.SendingEmail("Vacation  request",
                                string.Format(body, "Youxel", "Admin",
                                    "vacation request from " + acceptedRequest.Employee.FName + "  " +
                                    acceptedRequest.Employee.LName + "(" + acceptedRequest.CountDays + ") days from   " + acceptedRequest.DurationFrom.Value.ToString("yyyy-MM-dd") +
                                   "     to    " + acceptedRequest.DurationTo.Value.ToString("yyyy-MM-dd") +
                                    " completely approved.  "), email, adminEmail, adminPass);
                        }


                    }



                    _SendEmail.SendingEmail("Vacation  request",
                        string.Format(body, "Youxel", "Admin",
                            "  your vacation request  from    " + acceptedRequest.DurationFrom.Value.ToString("yyyy-MM-dd") + "   to    " + acceptedRequest.DurationTo.Value.ToString("yyyy-MM-dd") + " has been completely approved, navigate to  the system for more details. "),
                        acceptedRequest.Employee.Email, adminEmail, adminPass);
                    /*send to user how send vacation that a manger approve his request */



                }


                _context.SaveChanges();



            }



            else
            {






                var lowermanager /*the first managers bottom of logined user */  =
                    _context.Workflow.Where(
                        x =>
                            teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel &&
                            x.LevelNum.Equals(levelnum - 1)).FirstOrDefault();






                _SendEmail.SendingEmail("Vacation  request",
                    string.Format(body, "Youxel", "Admin",
                        "" + acceptedRequest.Employee.FName + "      " + acceptedRequest.Employee.LName +
                        "   request a  new vacation (" + acceptedRequest.CountDays + " ) days from   " + acceptedRequest.DurationFrom.Value.ToString("yyyy-MM-dd") + "   to   " + acceptedRequest.DurationTo.Value.ToString("yyyy-MM-dd") + " , navigate to  the system for more details. "), lowermanager.Employee.Email,
                    adminEmail, adminPass);


                _SendEmail.SendingEmail("Vacation  request",
                    string.Format(body, "Youxel", "Admin",
                        "" + HttpContext.Current.Session["LogedUserFname"] + "      " +
                        (string)HttpContext.Current.Session["LogedUserLname"] +
                        "  approved your vacation request ,it is  now pending for approval from" + "   " + lowermanager.Employee.FName + "  " + lowermanager.Employee.LName + ", navigate to  the system for more details. "),
                    acceptedRequest.Employee.Email, adminEmail, adminPass);
                /*send to user how send vacation that a manger approve his request */








            }

            if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) != "2")
            {
                RequestApproval vacation = new RequestApproval();

                var listofapprovals = _context.RequestApproval.ToList().OrderByDescending(x => x.ID);

                if (!listofapprovals.Any())
                {

                    vacation.ID = "Approval-000001";

                }

                else
                {

                    var originalValue = listofapprovals.Select(x => x.ID).FirstOrDefault();

                    // Get the numeric part (0002)
                    var stringValue = Regex.Match(originalValue, @"\d+").Value;
                    // Convert to int
                    var intValue = Int32.Parse(stringValue);
                    // Increase
                    intValue++;
                    // Convert back to string
                    originalValue = "Approval-" + intValue.ToString("D6");










                    vacation.ID = originalValue;

                }

                vacation.WorkflowID = workflowId;
                vacation.RequestID = requestid;
                vacation.StatusID = "2";
                vacation.Comment = requestapproval.Comment;











                vacation.ApprovalBy = (string)HttpContext.Current.Session["LogedUserID"];
                vacation.ApprovalAT = DateTime.Now;
                vacation.LastModified = DateTime.Now;



                _context.RequestApproval.Add(vacation);









                Request requestcomment = _context.Request.FirstOrDefault(x => x.ID == requestid);


                if (requestcomment != null)
                {
                    requestcomment.Comment = requestcomment.Comment +
                                             "   \n " + HttpContext.Current.Session["LogedUserFname"] + "  " +
                                             (string)HttpContext.Current.Session["LogedUserLname"] + "  " +
                                             "Comment : " +
                                             requestapproval.Comment + "  ,            ";
                    requestcomment.LastModified = DateTime.Now;


                    _context.Entry(requestcomment).Property(x => x.EmployeeID).IsModified = false;
                    _context.Entry(requestcomment).Property(x => x.DurationFrom).IsModified = false;
                    _context.Entry(requestcomment).Property(x => x.VacationORWorkHome).IsModified = false;
                    _context.Entry(requestcomment).Property(x => x.CreatedAT).IsModified = false;
                    _context.Entry(requestcomment).State = EntityState.Modified;
                }


                _context.SaveChanges();






            }
        }




        public void RejctVacation(RequestApproval requestapproval, string employeeId, string requestid)
        {
            Request rejectedRequest = _context.Request.FirstOrDefault(x => x.ID == requestid);
            if (rejectedRequest != null)
            {
                rejectedRequest.RequestStatusID = "3";


                rejectedRequest.Comment = rejectedRequest.Comment +
                                          HttpContext.Current.Session["LogedUserFname"] + "  " +
                                          (string)HttpContext.Current.Session["LogedUserLname"] + "Comment : " +
                                          requestapproval.Comment + "  ,            ";



                rejectedRequest.LastModified = DateTime.Now;


                _context.Entry(rejectedRequest).Property(x => x.EmployeeID).IsModified = false;
                _context.Entry(rejectedRequest).Property(x => x.DurationFrom).IsModified = false;
                _context.Entry(rejectedRequest).Property(x => x.VacationORWorkHome).IsModified = false;
                _context.Entry(rejectedRequest).Property(x => x.CreatedAT).IsModified = false;
                _context.Entry(rejectedRequest).State = EntityState.Modified;
            }
            _context.SaveChanges();


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail = _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass = _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();


            var text = "vacation request from " + rejectedRequest.Employee.FName + "  " + rejectedRequest.Employee.LName + " completely  rejected  by  his  team hierarchy    , request ID :  " + rejectedRequest.ID;
            _notificationRepository.AddNotification(text, rejectedRequest.ID);



            _SendEmail.SendingEmail("Vacation  request", string.Format(body, "Youxel", "Admin", "" + "your vacation request     from     " + rejectedRequest.DurationFrom.Value.ToString("yyyy-MM-dd") + "   to  " + rejectedRequest.DurationTo.Value.ToString("yyyy-MM-dd") + "  has been completely rejected, navigate to  the system for more details. "), rejectedRequest.Employee.Email, adminEmail, adminPass);







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
