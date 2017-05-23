using Galary.Models;
using Galary.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Galary.Controllers
{
    [CustomAuthorize]
    public class VideoTypesController : Controller
    {
        private GalaryContext db = new GalaryContext();

        // GET: VideoTypes
        public ActionResult Index()
        {
            return View(db.VideoTypes.ToList());
        }

        // GET: VideoTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoTypes videoTypes = db.VideoTypes.Find(id);
            if (videoTypes == null)
            {
                return HttpNotFound();
            }
            return View(videoTypes);
        }

        // GET: VideoTypes/Create
        public ActionResult Create()
        {
            VideoTypesViewModel videoTypesViewModel = new VideoTypesViewModel();
            videoTypesViewModel.AllRoles = (from r in db.Roles
                                            select new RolesViewModel
                                            {
                                                RoleID = r.RoleID,
                                                RoleName = r.RoleName,
                                                IsSelected = false
                                            }).ToList();
            videoTypesViewModel.SelectedRoles = new List<int>();
            return View(videoTypesViewModel);
        }

        // POST: VideoTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VideoTypesViewModel videoTypesViewModel)
        {
            if (ModelState.IsValid)
            {
                VideoTypeRoles videoTypeRoles = null;
                videoTypesViewModel.VideoTypes.AssignedRoles = new List<VideoTypeRoles>();

                if (videoTypesViewModel.SelectedRoles == null)
                {
                    videoTypesViewModel.SelectedRoles = new List<int>();
                    videoTypesViewModel.SelectedRoles.Add(1);
                }
                else if (!videoTypesViewModel.SelectedRoles.Contains(1))
                    videoTypesViewModel.SelectedRoles.Add(1);

                if (videoTypesViewModel.SelectedRoles != null)
                {
                    foreach (int role in videoTypesViewModel.SelectedRoles)
                    {
                        videoTypeRoles = new VideoTypeRoles();
                        videoTypeRoles.RoleID = role;
                        videoTypesViewModel.VideoTypes.AssignedRoles.Add(videoTypeRoles);
                    }
                }

                db.VideoTypes.Add(videoTypesViewModel.VideoTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            videoTypesViewModel.AllRoles = (from r in db.Roles
                                            select new RolesViewModel
                                            {
                                                RoleID = r.RoleID,
                                                RoleName = r.RoleName,
                                                IsSelected = false
                                            }).ToList();
            videoTypesViewModel.SelectedRoles = new List<int>();
            return View(videoTypesViewModel);
        }

        // GET: VideoTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            VideoTypesViewModel videoTypesViewModel = new VideoTypesViewModel();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoTypes videoTypes = db.VideoTypes.Find(id);
            if (videoTypes == null)
            {
                return HttpNotFound();
            }
            else
            {
                videoTypesViewModel = new VideoTypesViewModel();
                videoTypesViewModel.VideoTypes = videoTypes;
                videoTypesViewModel.AllRoles = (from r in db.Roles
                                                join vr in db.VideoTypeRoles on r.RoleID equals vr.RoleID into videoRoles
                                                from vrs in videoRoles.DefaultIfEmpty()
                                                orderby r.RoleID
                                                select new RolesViewModel
                                                {
                                                    RoleID = r.RoleID,
                                                    RoleName = r.RoleName,
                                                    IsSelected = (bool?)(vrs.RoleID > 0)
                                                }
                                           ).Distinct().ToList();
                videoTypesViewModel.SelectedRoles = db.VideoTypeRoles.Where(r => r.VideoTypeID == id).Select(r => r.RoleID).ToList();
            }
            return View(videoTypesViewModel);
        }

        // POST: VideoTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VideoTypesViewModel videoTypesViewModel)
        {
            if (ModelState.IsValid)
            {
                List<VideoTypeRoles> lstVideoTypeRoles = db.VideoTypeRoles.Where(u => u.VideoTypeID == videoTypesViewModel.VideoTypes.VideoTypeID).ToList();
                List<int> roleIDs = lstVideoTypeRoles.Select(r => r.RoleID).ToList();

                foreach (Roles role in db.Roles)
                {
                    if (roleIDs.Contains(role.RoleID) && !videoTypesViewModel.SelectedRoles.Contains(role.RoleID))
                    {
                        var videoTypeRoles = db.VideoTypeRoles.Where(r => r.RoleID == role.RoleID && r.VideoTypeID == videoTypesViewModel.VideoTypes.VideoTypeID).FirstOrDefault();
                        db.VideoTypeRoles.Remove(videoTypeRoles);
                    }
                    else if (!roleIDs.Contains(role.RoleID) && videoTypesViewModel.SelectedRoles.Contains(role.RoleID))
                    {
                        db.VideoTypeRoles.Add(new VideoTypeRoles { VideoTypeID = videoTypesViewModel.VideoTypes.VideoTypeID, RoleID = role.RoleID });
                    }
                }
                db.Entry(videoTypesViewModel.VideoTypes).State = EntityState.Modified;
                db.Entry(new VideoTypeRoles()).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(videoTypesViewModel);
        }

        // GET: VideoTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoTypes videoTypes = db.VideoTypes.Find(id);
            if (videoTypes == null)
            {
                return HttpNotFound();
            }
            return View(videoTypes);
        }

        // POST: VideoTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VideoTypes videoTypes = db.VideoTypes.Find(id);
            db.VideoTypes.Remove(videoTypes);
            db.SaveChanges();
            return RedirectToAction("Index");
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
