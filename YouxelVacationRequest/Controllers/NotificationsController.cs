using System;
using System.Web.Mvc;
using BLL.Interfaces;
using BLL.Repositories;
using BOL;
using YouxelVacationRequest.Filters;

namespace YouxelVacationRequest.Controllers
{
    public class NotificationsController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly YouxelVacation _context;


        public NotificationsController()
        {
            _context = new YouxelVacation();
            _unitOfWork = new UnitOfWork(_context);

        }


        [ApprovalPrivilege]
        public ActionResult ManagerShownotifications()

        {
            string userId = Convert.ToString(Session["LogedUserID"]);

            var notifications = _unitOfWork.Vacation.ManagerShownotifications(userId);
            return View(notifications);
        }



        public int numOfNotifications()

        {
            string userId = Convert.ToString(Session["LogedUserID"]);

            var num = _unitOfWork.Vacation.numOfNotifications(userId);
            return num;
        }



    }
}