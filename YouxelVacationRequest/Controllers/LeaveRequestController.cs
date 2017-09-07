using System;
using System.Data;
using System.Net;
using System.Web.Mvc;
using BLL.Interfaces;
using BLL.Repositories;
using BOL;
using YouxelVacationRequest.Filters;

namespace YouxelVacationRequest.Controllers
{

    public class LeaveRequestController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly YouxelVacation _context;









        public LeaveRequestController()
        {
            _context = new YouxelVacation();
            _unitOfWork = new UnitOfWork(_context);

        }

        public ActionResult Error()

        {

            return View();
        }




        public ActionResult ApplyVacation()

        {







            ViewBag.VacationTypeID = new SelectList(_context.VacationType, "ID", "Type");

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ApplyVacation(Request request, FormCollection form)
        {

            var userId = Convert.ToString(Session["LogedUserID"]);

            var employee = _unitOfWork.Admin.Get(userId);

            Session["VacationBalance"] = employee.VacationBalance;
            Session["BalanceLimit"] = employee.BalanceLimit;

            ViewBag.VacationTypeID = new SelectList(_context.VacationType, "ID", "Type", request.VacationTypeID);
            Session["VacationType"] = form["VacationTypeID"];



            try
            {

                if (_unitOfWork.Vacation.AddVacation(request) == 2)
                {


                    ViewBag.scessrequest = "Request added, it is  now pending for approval ...";


                    return View("success");
                }



                if (_unitOfWork.Vacation.AddVacation(request) == 1)
                {

                    ViewBag.errorBalance = "Error, Your balance of vacation is not allowed, please check your balance in vacation history section.";
                    return View("Error");
                }

                else
                {

                    ViewBag.errorPeriod = "Error, this vacation period is in between or before  other vacations peroid you have requestd before, please check your vacation history for more details.";
                    return View("Error");

                }

            }


            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, End date must be greater than start date and date should be greater than or equal today  ");
            }

            return View(request);






        }









        [ApprovalPrivilege]
        public ActionResult ApproveVacation(string id)

        {


            Session["RequestID"] = id;






            return View();
        }




        [HttpPost]
        [ApprovalPrivilege]
        public ActionResult ApproveVacation(RequestApproval requestapproval)
        {
            var userId = Convert.ToString(Session["LogedUserID"]);
            var requestId = Convert.ToString(Session["RequestID"]);

            try
            {

                _unitOfWork.Vacation.ApproveVacation(requestapproval, userId, requestId);







                ViewBag.sentapproval = "Approval has been sent.";
                return View("success");
            }


            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, comment comment is to long ");
            }


            return View(requestapproval);


        }

















        [ApprovalPrivilege]
        public ActionResult RejectVacation(string id)

        {


            Session["RequestID"] = id;






            return View();
        }




        [HttpPost]
        [ApprovalPrivilege]
        public ActionResult RejectVacation(RequestApproval requestapproval)
        {


            var userId = Convert.ToString(Session["LogedUserID"]);
            var requestId = Convert.ToString(Session["RequestID"]);

            try
            {
                _unitOfWork.Vacation.RejctVacation(requestapproval, userId, requestId);








                ViewBag.sentapproval = "Approval has been sent .";
                return View("success");
            }



            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, comment comment is to long ");
            }

            return View(requestapproval);

        }


















        public ActionResult VacationHistory()

        {
            var userId = Convert.ToString(Session["LogedUserID"]);



            var employee = _unitOfWork.Admin.Get(userId);

            Session["VacationBalance"] = employee.VacationBalance;
            Session["BalanceLimit"] = employee.BalanceLimit;

            var history = _unitOfWork.Vacation.ShowVactionHistory(userId);

            return View(history);
        }











        public ActionResult MYRequestDetails(string id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = Convert.ToString(Session["LogedUserID"]);



            var request = _unitOfWork.Vacation.Get(id, userId);





            if (request == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");
            }



            return View(request);
        }




        public ActionResult RequestDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = _unitOfWork.Vacation.GetById(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }



        [ApprovalPrivilege]
        public ActionResult ManagerShowRequests()

        {
            string userId = Convert.ToString(Session["LogedUserID"]);

            var history = _unitOfWork.Vacation.ManagerShowRequests(userId);
            return View(history);
        }












        public ActionResult UpdateVacation(string id)

        {


            var userId = Convert.ToString(Session["LogedUserID"]);

            ViewBag.VacationTypeID = new SelectList(_context.VacationType, "ID", "Type");
            var request = _unitOfWork.Vacation.Get(id, userId);


            if (Convert.ToString(Session["UserTypeID"]) == "2")
            {

                request = _unitOfWork.Vacation.GetById(id);
            }







            if (request == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");
            }
            return View();
        }


        [HttpPost]
        public ActionResult UpdateVacation(Request request, FormCollection form)

        {


            var userId = Convert.ToString(Session["LogedUserID"]);

            var employee = _unitOfWork.Admin.Get(userId);

            Session["VacationBalance"] = employee.VacationBalance;
            Session["BalanceLimit"] = employee.BalanceLimit;

            ViewBag.VacationTypeID = new SelectList(_context.VacationType, "ID", "Type", request.VacationTypeID);
            Session["VacationType"] = form["VacationTypeID"];
            try
            {
                if (_unitOfWork.Vacation.UpdateVacation(request) == false)
                {
                    ViewBag.fail = Convert.ToString(Session["UserTypeID"]) == "2" ? "The duration is greater than the balance of the employee, please check his balance and try again. " : "Error, Your balance of vacation is not allowed, please check your balance in vacation history section.";
                    return View("error");
                }
                if (Convert.ToString(Session["UserTypeID"]) == "2")
                {
                    ViewBag.VacationUpdated = "Vacation successfully updated  ";

                }
                else
                {


                    return RedirectToAction("VacationHistory");
                }
            }


            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, End date must be greater than start date and date should be greater than or equal today  ");
                return View(request);
            }





            return View();


        }








        public ActionResult CancelVacation(string id)

        {

            var userId = Convert.ToString(Session["LogedUserID"]);


            var request = _unitOfWork.Vacation.Get(id, userId);


            if (Convert.ToString(Session["UserTypeID"]) == "2")
            {

                request = _unitOfWork.Vacation.GetById(id);
            }


            if (request == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");
            }
            return View(request);










        }


        [HttpPost]
        public ActionResult CancelVacation(Request request)

        {
            _unitOfWork.Vacation.CancelVacation(request);
            if (Convert.ToString(Session["UserTypeID"]) == "2")
            {
                return RedirectToAction("ManagerShowRequests");

            }
            else
            {


                return RedirectToAction("VacationHistory");
            }

        }











        public ActionResult Success()
        {
            return View();
        }







    }
}