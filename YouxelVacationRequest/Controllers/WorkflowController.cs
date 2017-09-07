using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BLL.Interfaces;
using BLL.Repositories;
using BOL;
using  YouxelVacationRequest.Filters;


namespace YouxelVacationRequest.Controllers
{
        [Admin]
    public class WorkflowController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly YouxelVacation _context;






        public WorkflowController()
        {
            _context = new  YouxelVacation();
            _unitOfWork = new UnitOfWork(_context);

        }





      
   


        public ActionResult Get(string id)
        {
            var workflow = _unitOfWork.Workflow.GetByTeamID(id);

            return  View(workflow);
        }



        // GET: Teams/Create
        public ActionResult Create(string id )
        {
            ViewBag.EmployeeID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "Email");
            ViewBag.TeamID = new SelectList(_context.Team, "ID", "Name");
            ViewBag.levels = _unitOfWork.Workflow.TeamNumLevels(id);
            Session["workflowteamid"] = id;



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Workflow workflow, FormCollection form)
        {

           
            try
            {

                if (ModelState.IsValid)
                {
                    _unitOfWork.Workflow.Add(workflow, Convert.ToString(Session["workflowteamid"]));

                    return RedirectToAction("Index", "Teams", new { area = "" });
                }
            }




            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewBag.EmployeeID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "Email");
            ViewBag.TeamID = new SelectList(_context.Team, "ID", "Name");


            return View(workflow);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(string id , string teamid ,string   userid)
        {

            ViewBag.EmployeeID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "LName");
            ViewBag.TeamID = new SelectList(_context.Team, "ID", "Name");
            ViewBag.levels = _unitOfWork.Workflow.TeamNumLevels(teamid);
            Session["workflowuserid"] = userid;
            Session["workflowteamid"] = teamid;

            var workflow = _unitOfWork.Workflow.Get(id); 
            if (workflow == null)
            {
                return HttpNotFound();
            }
            return View(workflow);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Workflow workflow)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Workflow.Update(workflow ,Convert.ToString(Session["workflowuserid"])  , Convert.ToString(Session["workflowteamid"]));

                return RedirectToAction("Index", "Teams", new { area = "" });
            }

            ViewBag.EmployeeID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "LName");
            ViewBag.TeamID = new SelectList(_context.Team, "ID", "Name");
            return View(workflow);
        }



       
        public ActionResult Delete(string id, string teamid, string userid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var workflow = _context.Workflow.Find(id);
            if (workflow == null)
            {
                return HttpNotFound();
            }
            Session["workflowuserid"] = userid;
            Session["workflowteamid"] = teamid;

            return View(workflow);
        }
       
        [HttpPost]
        public ActionResult Delete()
        {

            _unitOfWork.Workflow.delete(Convert.ToString(Session["workflowteamid"]), Convert.ToString(Session["workflowuserid"]));
            return RedirectToAction("Index", "Teams", new { area = "" });
        }



    }
}
