using Galary.Models;
using Galary.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Galary.Controllers
{
    [CustomAuthorize]
    public class UsersController : Controller
    {
        private GalaryContext db = new GalaryContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            UsersViewModel usersViewModel = new UsersViewModel();
            usersViewModel.AllRoles = (from r in db.Roles
                                       select new RolesViewModel
                                       {
                                           RoleID = r.RoleID,
                                           RoleName = r.RoleName,
                                           IsSelected = false
                                       }).ToList();
            usersViewModel.SelectedRoles = new List<int>();
            return View(usersViewModel);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsersViewModel usersViewModel)
        {
            if (ModelState.IsValid)
            {
                UsersRoles usersRoles = null;
                usersViewModel.User.IsActive = true;
                usersViewModel.User.CreatedDate = DateTime.Now;
                usersViewModel.User.Password = EncryptePassword(usersViewModel.User.Password);
                usersViewModel.User.AssignedRoles = new List<UsersRoles>();

                foreach (int role in usersViewModel.SelectedRoles)
                {
                    usersRoles = new UsersRoles();
                    usersRoles.RoleID = role;
                    usersViewModel.User.AssignedRoles.Add(usersRoles);
                }
                db.Users.Add(usersViewModel.User);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            usersViewModel.AllRoles = (from r in db.Roles
                                       select new RolesViewModel
                                       {
                                           RoleID = r.RoleID,
                                           RoleName = r.RoleName,
                                           IsSelected = false
                                       }).ToList();
            usersViewModel.SelectedRoles = new List<int>();
            return View(usersViewModel);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            UserEditViewModel usersViewModel = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            else
            {
                usersViewModel = new UserEditViewModel();
                usersViewModel.UserID = users.UserID;
                usersViewModel.UserName = users.UserName;
                usersViewModel.FirstName = users.FirstName;
                usersViewModel.LastName = users.LastName;
                usersViewModel.EmailID = users.EmailID;
                usersViewModel.Mobile = users.Mobile;
                usersViewModel.IsActive = users.IsActive;
                usersViewModel.AllRoles = (from r in db.Roles
                                           join ur in db.UsersRoles on r.RoleID equals ur.RoleID into userRoles
                                           from urs in userRoles.DefaultIfEmpty()
                                           orderby r.RoleID
                                           select new RolesViewModel
                                           {
                                               RoleID = r.RoleID,
                                               RoleName = r.RoleName,
                                               IsSelected = (bool?)(urs.RoleID > 0)
                                           }
                                           ).Distinct().ToList();
                usersViewModel.SelectedRoles = db.UsersRoles.Where(r => r.Users.UserID == id).Select(r => r.RoleID).ToList();
            }
            return View(usersViewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditViewModel usersViewModel)
        {
            if (ModelState.IsValid)
            {
                List<UsersRoles> lstUserRole = db.UsersRoles.Where(u => u.UserID == usersViewModel.UserID).ToList();
                List<int> roleIDs = lstUserRole.Select(r => r.RoleID).ToList();
                Users user = db.Users.Where(u => u.UserID == usersViewModel.UserID).First();

                if (usersViewModel.UserID == 1)
                {
                    usersViewModel.IsActive = true;
                    usersViewModel.SelectedRoles = db.Roles.Select(r => r.RoleID).ToList();
                }

                user.UserName = usersViewModel.UserName;
                user.FirstName = usersViewModel.FirstName;
                user.LastName = usersViewModel.LastName;
                user.EmailID = usersViewModel.EmailID;
                user.Mobile = usersViewModel.Mobile;
                user.IsActive = usersViewModel.IsActive;

                foreach (Roles role in db.Roles)
                {
                    if (roleIDs.Contains(role.RoleID) && !usersViewModel.SelectedRoles.Contains(role.RoleID))
                    {
                        var userRole = db.UsersRoles.Where(r => r.RoleID == role.RoleID && r.UserID == usersViewModel.UserID).FirstOrDefault();
                        db.UsersRoles.Remove(userRole);
                    }
                    else if (!roleIDs.Contains(role.RoleID) && usersViewModel.SelectedRoles.Contains(role.RoleID))
                    {
                        db.UsersRoles.Add(new UsersRoles { UserID = usersViewModel.UserID, RoleID = role.RoleID });
                    }
                }
                db.Entry(user).State = EntityState.Modified;
                db.Entry(new UsersRoles()).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                usersViewModel.AllRoles = (from r in db.Roles
                                           join ur in db.UsersRoles on r.RoleID equals ur.RoleID into userRoles
                                           from urs in userRoles.DefaultIfEmpty()
                                           orderby r.RoleID
                                           select new RolesViewModel
                                           {
                                               RoleID = r.RoleID,
                                               RoleName = r.RoleName,
                                               IsSelected = (bool?)(usersViewModel.SelectedRoles.Contains(r.RoleID))
                                           }
                                           ).Distinct().ToList();
            }
            return View(usersViewModel);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            CustomPrincipalSerializeModel currentUser = (CustomPrincipalSerializeModel)Session["User"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (id == currentUser.CurrentUserID)
            {
                TempData["Error"] = "You cannot delete your user account !";
                return RedirectToAction("Index");
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static string EncryptePassword(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(input);
            data = x.ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
