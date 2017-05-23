using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Galary.Models;
using Galary.Security;
using System.Web.Security;
using Newtonsoft.Json;

namespace Galary.Controllers
{
    public class AuthenticationController : Controller
    {
        private GalaryContext db = new GalaryContext();

        // GET: Authentication/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Authentication/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel, string returnURL = "")
        {
            if (ModelState.IsValid)
            {

                string encryptedPassword = EncryptePassword(loginViewModel.Password);

                var user = db.Users.Where(u => u.UserName == loginViewModel.UserName && u.Password == encryptedPassword).FirstOrDefault();

                if (user != null)
                {
                    if (user.IsActive)
                    {
                        var roles = (from r in db.Roles
                                     join ur in db.UsersRoles on r.RoleID equals ur.RoleID
                                     where user.UserID == ur.UserID
                                     select r.RoleID).ToList();

                        CustomPrincipalSerializeModel customPrincipalSerializeModel = new CustomPrincipalSerializeModel();
                        customPrincipalSerializeModel.CurrentUserName = loginViewModel.UserName;
                        customPrincipalSerializeModel.CurrentUserID = user.UserID;
                        customPrincipalSerializeModel.CurrentRoles = roles;
                        Session["User"] = customPrincipalSerializeModel;

                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(15), false, JsonConvert.SerializeObject(customPrincipalSerializeModel));
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                        Response.Cookies.Add(authCookie);
                        return RedirectToAction("Index", "Catalog");
                    }
                    else
                    {
                        TempData["LoginFailed"] = "Your user account is inactive !";
                    }
                }
                else
                {
                    TempData["LoginFailed"] = "Invalid user name or password !";
                }
            }

            return View(loginViewModel);
        }

        public static string EncryptePassword(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(input);
            data = x.ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        [ChildActionOnly]
        public PartialViewResult GetMenusForUser()
        {
            List<MenuViewModel> lstMenuViewModel = new List<MenuViewModel>();
            MenuViewModel menuViewModel = null;
            if (((CustomPrincipal)System.Web.HttpContext.Current.User).CurrentRoles.Contains(1))
            {

                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "Catalog";
                menuViewModel.Controller = "Catalog";
                menuViewModel.Action = "Index";
                lstMenuViewModel.Add(menuViewModel);

                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "Roles";
                menuViewModel.Controller = "Roles";
                menuViewModel.Action = "Index";
                lstMenuViewModel.Add(menuViewModel);

                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "Users";
                menuViewModel.Controller = "Users";
                menuViewModel.Action = "Index";
                lstMenuViewModel.Add(menuViewModel);

                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "VideoTypes";
                menuViewModel.Controller = "VideoTypes";
                menuViewModel.Action = "Index";
                lstMenuViewModel.Add(menuViewModel);

                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "Videos";
                menuViewModel.Controller = "Videos";
                menuViewModel.Action = "Index";
                lstMenuViewModel.Add(menuViewModel);

                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "SignOut";
                menuViewModel.Controller = "Authentication";
                menuViewModel.Action = "Logout";
                lstMenuViewModel.Add(menuViewModel);
            }
            else
            {
                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "Catalog";
                menuViewModel.Controller = "Catalog";
                menuViewModel.Action = "Index";
                lstMenuViewModel.Add(menuViewModel);

                menuViewModel = new MenuViewModel();
                menuViewModel.MenuName = "SignOut";
                menuViewModel.Controller = "Authentication";
                menuViewModel.Action = "Logout";
                lstMenuViewModel.Add(menuViewModel);
            }
            return PartialView("_GalaryMenu", lstMenuViewModel);
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
