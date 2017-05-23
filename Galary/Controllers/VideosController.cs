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
    public class VideosController : Controller
    {
        private GalaryContext db = new GalaryContext();

        // GET: Videos
        public ActionResult Index()
        {
            GalaryViewModel galary = new GalaryViewModel();
            galary.VideoTypes = new List<VideoTypes>();
            galary.Videos = new List<Videos>();

            CustomPrincipalSerializeModel currentUser = (CustomPrincipalSerializeModel)Session["User"];
            List<VideoTypes> lstVideoType = db.VideoTypes.OrderBy(r => r.VideoTypeName).ToList();

            foreach(VideoTypes videoType in lstVideoType)
            {
                if(videoType.AssignedRoles.Where(r => currentUser.CurrentRoles.Contains(r.RoleID)).FirstOrDefault() != null)
                {
                    galary.VideoTypes.Add(videoType);
                    galary.Videos.AddRange(videoType.VideosCollection);
                }
            }

            /*
            foreach (int role in currentUser.CurrentRoles)
            {
                var videoType = db.VideoTypes.Where(v => v.AssignedRoles.Any(r => r.RoleID == role)).ToList();
                if (videoType != null)
                {
                    galary.VideoTypes.AddRange(videoType);
                    foreach (VideoTypes video in videoType)
                    {
                       galary.Videos.AddRange(video.VideosCollection);
                    }
                }
            }
            */
            return View(galary);
        }

        // GET: Videos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Videos videos = db.Videos.Find(id);
            if (videos == null)
            {
                return HttpNotFound();
            }
            return View(videos);
        }

        // GET: Videos/Create
        public ActionResult Create()
        {
            VideosViewModel videosViewModel = new VideosViewModel();
            videosViewModel.VideoTypeList = db.VideoTypes.ToList<VideoTypes>();
            return View(videosViewModel);
        }

        // POST: Videos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VideoID,VideoTypeID,VideoDescription,VideoURL")] VideosViewModel videosViewModel)
        {
            CustomPrincipalSerializeModel currentUser = (CustomPrincipalSerializeModel)Session["User"];

            Videos videos = new Videos();
            videos.VideoTypeID = videosViewModel.VideoTypeID;
            videos.VideoURL = videosViewModel.VideoURL;
            videos.VideoDescription = videosViewModel.VideoDescription;
            videos.CreatedBy = currentUser.CurrentUserID;
            videos.CreatedOn = DateTime.Now;
            videos.UpdatedBy = currentUser.CurrentUserID;
            videos.UpdatedOn = DateTime.Now;

            if (ModelState.IsValid)
            {

                db.Videos.Add(videos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(videosViewModel);
        }

        // GET: Videos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideosViewModel videosViewModel = (from v in db.Videos
                                               where v.VideoID == id.Value
                                               select new VideosViewModel
                                               {
                                                   VideoID = v.VideoID,
                                                   VideoDescription = v.VideoDescription,
                                                   VideoTypeID = v.VideoTypeID,
                                                   VideoURL = v.VideoURL
                                               }).FirstOrDefault();

            if (videosViewModel == null)
            {
                return HttpNotFound();
            }

            videosViewModel.VideoTypeList = db.VideoTypes.ToList<VideoTypes>();
            return View(videosViewModel);
        }

        // POST: Videos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VideoID,VideoTypeID,VideoDescription,VideoURL")] VideosViewModel videosViewModel)
        {
            CustomPrincipalSerializeModel currentUser = (CustomPrincipalSerializeModel)Session["User"];

            Videos video = db.Videos.Where(v => v.VideoID == videosViewModel.VideoID).First();
            video.VideoDescription = videosViewModel.VideoDescription;
            video.VideoTypeID = videosViewModel.VideoTypeID;
            video.VideoURL = videosViewModel.VideoURL;
            video.UpdatedBy = currentUser.CurrentUserID;
            video.UpdatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(videosViewModel);
        }

        // GET: Videos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Videos videos = db.Videos.Find(id);
            if (videos == null)
            {
                return HttpNotFound();
            }
            return View(videos);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Videos videos = db.Videos.Find(id);
            db.Videos.Remove(videos);
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
