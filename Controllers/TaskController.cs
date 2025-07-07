using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaskManagementApp.DAL;
using TaskManagementApp.Models;
using PagedList;

namespace TaskManagementApp.Controllers
{
    public class TaskController : Controller
    {
        private readonly DbHelper db = new DbHelper();

        // GET: Task
        public ActionResult Index(string searchTerm, int? page)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            List<TaskModel> tasks = !string.IsNullOrWhiteSpace(searchTerm)
                ? db.GetTasksByUserIdWithSearch(userId, searchTerm)
                : db.GetTasksByUserId(userId);

            int pageSize = 5;
            int pageNumber = page ?? 1;

            IPagedList<TaskModel> pagedTasks = tasks.ToPagedList(pageNumber, pageSize);
            return View(pagedTasks);
        }

        // GET: Task/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Task/Create
        [HttpPost]
        public ActionResult Create(TaskModel task)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            task.CreatedDate = DateTime.Now;
            task.UserId = Convert.ToInt32(Session["UserId"]);
            task.Status = "Pending";

            db.AddTask(task);

            return RedirectToAction("Index");
        }

        // GET: Task/Edit/{id}
        public ActionResult Edit(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            TaskModel task = db.GetTaskById(id);
            if (task == null)
                return HttpNotFound();

            return View(task);
        }

        // POST: Task/Edit
        [HttpPost]
        public ActionResult Edit(TaskModel task)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.UpdateTask(task);
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Task/Delete/{id}
        public ActionResult Delete(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            db.DeleteTask(id);
            return RedirectToAction("Index");
        }
    }
}
