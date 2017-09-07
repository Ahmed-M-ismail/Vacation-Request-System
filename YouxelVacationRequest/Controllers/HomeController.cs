using System;
using System.Web.Mvc;
using BLL.Interfaces;
using BLL.Repositories;
using BOL;
using YouxelVacationRequest.Filters;

namespace YouxelVacationRequest.Controllers
{

    public class HomeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly YouxelVacation _context;


        public HomeController()
        {
            _context = new YouxelVacation();

            _unitOfWork = new UnitOfWork(_context);
        }


        public ActionResult Success()
        {


            return View();
        }



        [HttpPost]
        public JsonResult IsAlreadySigned(string userEmailId)
        {

            return Json(_unitOfWork.Admin.GetByEmail(userEmailId));

        }


        public ActionResult Error()
        {
            return View();
        }


        public ActionResult InternalServer()
        {
            return View();
        }



        public ActionResult NotFound()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]

        public ActionResult Login(Employee emp)
        {
            var Employee = _unitOfWork.Employee.Login(emp);



            if (Employee != null)


            {

                Session["LogedUserID"] = Employee.ID;
                Session["LogedUserFname"] = Employee.FName;
                Session["LogedUserLname"] = Employee.LName;
                Session["UserTypeID"] = Employee.UserRoleID;


              
                Session["BalanceLimit"] = Employee.BalanceLimit;
                if (_unitOfWork.Employee.emptype() == 1)
                {

                    Session["empType"] = "topManager";

                }
                if (_unitOfWork.Employee.emptype() == 2)
                {

                    Session["empType"] = "lowerManager";

                }

                if (_unitOfWork.Employee.emptype() == 3)
                {

                    Session["empType"] = "normalEmp";

                }



                ViewBag.successlogin = "Successful login, welcome   " +Convert.ToString( Session["LogedUserFname"]) +"   "+ Convert.ToString(Session["LogedUserLname"]) ;
                return View("success");

            }

            ViewBag.worngEmailOrPass = "Worng E-mail or password ";
            return View("error");
        }







        public ActionResult ChangePassword()

        {


            return View();
        }


        [HttpPost]
        public ActionResult ChangePassword(Employee employee)

        {
            string userId = Convert.ToString(Session["LogedUserID"]);

            _unitOfWork.Employee.ChangePassword(employee, userId);

            TempData["passwordChanged"] = "changed successfuly , login now with new password ";


            return RedirectToAction("Login");
        }






        public ActionResult LostPassword()

        {


            return View();
        }


        [HttpPost]
        public ActionResult LostPassword(string email)

        {

            if (_unitOfWork.Employee.LostPassword(email))
            {

                return View("EmailSent");
            }

            else
            {

                ViewBag.worngemail = "Email doesn't exist in the system ";
                return View("error");
            }






        }






        public ActionResult EmailSent()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login");
        }







        [Admin]
        public ActionResult EditEmail()
        {

            ViewBag.email = _unitOfWork.Admin.getYouxelEmail();
            ViewBag.pass = _unitOfWork.Admin.getYouxelPass();
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmail(string mail, string pass)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Admin.UpdateEmail(mail, pass);

            }
            ViewBag.emailadded = "Successfuly added the new admin's email";
            return View("success");
        }
















        [Admin]
        public ActionResult EditGeneralBalance()
        {

            ViewBag.balance = _unitOfWork.Admin.getGeneralBalance();
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGeneralBalance(int balance)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Admin.UpdateGeneralBalance(balance);

            }
            ViewBag.balanceadded = "Successfuly added the new general balance";
            return View("success");
        }





        [Admin]
        public ActionResult Youxelhasemail()
        {

            var email = _unitOfWork.Admin.getYouxelEmail();
            if (email == null)
            { 

                

            }

            return View("EditEmail");



        }




      
       
    }
}