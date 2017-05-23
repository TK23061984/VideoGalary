using Galary.Models;
using Galary.Security;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System;

namespace Galary.Controllers
{
    [CustomAuthorize]
    public class CatalogController : Controller
    {
        private GalaryContext db = new GalaryContext();

        // GET: Catalog
        public ActionResult Index()
        {
            GalaryViewModel galary = new GalaryViewModel();
            galary.VideoTypes = new List<VideoTypes>();
            galary.Videos = new List<Videos>();

            if (Session["User"] != null)
            {
                CustomPrincipalSerializeModel currentUser = (CustomPrincipalSerializeModel)Session["User"];
                foreach (int role in currentUser.CurrentRoles)
                {
                    var videoType = db.VideoTypes.Where(v => v.AssignedRoles.Any(r => r.RoleID == role)).ToList();
                    if (videoType != null)
                    {
                        galary.VideoTypes.AddRange(videoType);
                        galary.VideoTypes = galary.VideoTypes.Where(i => i.VideoTypeID > 0).Distinct().ToList();
                    }
                    if (galary.VideoTypes.Count > 0)
                    {
                        galary.SelectedVideoType = galary.VideoTypes[0].VideoTypeID;
                        galary.Videos = db.Videos.Where(v => v.VideoTypeID.Equals(galary.SelectedVideoType)).ToList();
                    }
                }
            }
            else
            {
                return RedirectToAction("Logout", "Authentication");
            }
            return View(galary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GalaryViewModel galary)
        {
            galary.VideoTypes = new List<VideoTypes>();
            galary.Videos = new List<Videos>();

            CustomPrincipalSerializeModel currentUser = (CustomPrincipalSerializeModel)Session["User"];
            foreach (int role in currentUser.CurrentRoles)
            {
                var videoType = db.VideoTypes.Where(v => v.AssignedRoles.Any(r => r.RoleID == role)).ToList();
                if (videoType != null)
                {
                    galary.VideoTypes.AddRange(videoType);
                    galary.VideoTypes = galary.VideoTypes.Where(i => i.VideoTypeID > 0).Distinct().ToList();
                }
            }

            galary.Videos = db.Videos.Where(v => v.VideoTypeID.Equals(galary.SelectedVideoType)).ToList();

            return View(galary);
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