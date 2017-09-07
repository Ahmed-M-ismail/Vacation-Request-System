using System.Web.Mvc;
using BOL;
using System.Data;
using System.Net;
using BLL.Interfaces;
using BLL.Repositories;
using YouxelVacationRequest.Filters;

namespace YouxelVacationRequest.Controllers
{

    [Admin]
    public class EmployeesController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly YouxelVacation _context;
    




        public EmployeesController()
        {
            _context = new YouxelVacation();
            _unitOfWork = new UnitOfWork(_context);

        }

        // GET: Teams
        public ActionResult Index()
        {
            var employee = _unitOfWork.Admin.GetAll() ;

            return View(employee);
        }









     



        // GET: Teams/Create
        public ActionResult Create()
        {
            ViewBag.UserRoleID = new SelectList(_context.EmployeeRole, "ID", "Name");



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee, FormCollection form)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _unitOfWork.Admin.Add(employee);
                   

                    return RedirectToAction("Index");
                }
            }




            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, Email is already exists or you had entered invalid data ");
            }

            ViewBag.UserRoleID = new SelectList(_context.EmployeeRole, "ID", "Name" , employee.UserRoleID);
            return View(employee);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(string id)
        {






            ViewBag.UserRoleID = new SelectList(_context.EmployeeRole, "ID", "Name");




            var employee = _unitOfWork.Admin.Get(id); 
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.Admin.Update(employee);
                    return RedirectToAction("Index");
                }
            }

            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes, Email is already exists or you had entered invalid data ");
            }
            ViewBag.UserRoleID = new SelectList(_context.EmployeeRole, "ID", "Name", employee.UserRoleID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
      
        
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
           _unitOfWork.Admin.Delete(id);
            return RedirectToAction("Index");
        }




       





    }
}
