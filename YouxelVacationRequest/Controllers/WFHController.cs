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
    public class WfhController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly YouxelVacation _context;




        public WfhController()
        {
            _context = new YouxelVacation();
            _unitOfWork = new UnitOfWork(_context);
       
        }



        public ActionResult ApplyWFH()

        {

       

            return View();
        }




        [HttpPost]

        public ActionResult ApplyWFH(Request request)
        {






            try
            {

                if (_unitOfWork.Wfh.AddWfh(request) == true)
                {


                    ViewBag.scessrequest = "Request addedd its now pending ...";

                    return View("success");
                }


                else
                {
                   
                    return View("error");


                }


            }


            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes,  date should be greater than or equal today  ");
            }

            return View(request);

        }



        public ActionResult WfhHistory()

        {
            var userId = Convert.ToString(Session["LogedUserID"]);

            var history = _unitOfWork.Wfh.ShowWfhHistory(userId);
            return View(history);
        }













        public ActionResult UpdateWFH(string id)

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
            return View();





     
        }


        [HttpPost]
        public ActionResult UpdateWFH(Request request)

        {
            try
            {
                _unitOfWork.Wfh.UpdateWfh(request);
                if (Convert.ToString(Session["UserTypeID"]) == "2")
                {
                    ViewBag.WFHUpdated = "WFH successfully updated  "; 

                }
                else
                {


                    return RedirectToAction("WfhHistory");
                }
            }

            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes,  date should be greater than or equal today  ");
              
            }


            return View(request);

        }








        public ActionResult CancelWFH(string id)

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
        public ActionResult CancelWFH(Request request)

        {
            _unitOfWork.Wfh.CancelWfh(request);
            if (Convert.ToString(Session["UserTypeID"]) == "2")
            {
                return RedirectToAction("ManagerShowRequests");

            }
            else
            {


                return RedirectToAction("WfhHistory");
            }

        }


        [ApprovalPrivilege]
        public ActionResult ManagerShowRequests()

        {
            var userId = Convert.ToString(Session["LogedUserID"]);

            var history = _unitOfWork.Wfh.ManagerShowRequests(userId);
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
        public ActionResult ApproveWfh(string id)

        {


            Session["RequestID"] = id;






            return View();
        }






        [HttpPost]
        [ApprovalPrivilege]
        public ActionResult ApproveWfh(RequestApproval requestapproval)
        {
            var userId = Convert.ToString(Session["LogedUserID"]);
            var requestId = Convert.ToString(Session["RequestID"]);


            _unitOfWork.Wfh.ApproveWfh(requestapproval, userId, requestId);





            ViewBag.sentapproval = "Approval has been sent .";


            return View("success");
        }

















        [ApprovalPrivilege]
        public ActionResult RejectWfh(string id)

        {


            Session["RequestID"] = id;






            return View();
        }




        [HttpPost]
        [ApprovalPrivilege]
        public ActionResult RejectWfh(RequestApproval requestapproval)
        {
            var userId = Convert.ToString(Session["LogedUserID"]);
            var requestId = Convert.ToString(Session["RequestID"]);


            _unitOfWork.Wfh.RejctWfh(requestapproval, userId, requestId);








            ViewBag.sentapproval = "Approval has been sent.";


            return View("success");
        }





        public ActionResult error()

        {
    
            return View();
        }







        public ActionResult Success()
        {
            return View();
        }










    }
}