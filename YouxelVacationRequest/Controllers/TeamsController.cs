using System.Data;
using System.Linq;
using System.Web.Mvc;
using BLL.Interfaces;
using BLL.Repositories;
using BOL;
using YouxelVacationRequest.Filters;

namespace YouxelVacationRequest.Controllers
{        


    [Admin]
    public class TeamsController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly YouxelVacation _context;
  



        public TeamsController()
        {
            _context = new YouxelVacation();
            _unitOfWork = new UnitOfWork(_context);
      
        }




        // GET: Teams
        public ActionResult Index()
        {
            var teams = _unitOfWork.Team.GetAll();

            return View(teams);
        }



        // GET: Teams/Create
        public ActionResult Create()
        {
            ViewBag.ManagerID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "Email");



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Team team, FormCollection form)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _unitOfWork.Team.Add(team);

                    return RedirectToAction("Index");
                }
            }


         

            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewBag.ManagerID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "Email", team.ManagerID);
            return View(team);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.ManagerID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "Email");

            var team = _unitOfWork.Team.Get(id); 
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Team team)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Team.Update(team);
             
                return RedirectToAction("Index");
            }
            ViewBag.ManagerID = new SelectList(_context.Employee.Where(x=>x.AccountActive.Equals(true)), "ID", "Email", team.ManagerID);
            return View(team);
        }

     

    }
}
