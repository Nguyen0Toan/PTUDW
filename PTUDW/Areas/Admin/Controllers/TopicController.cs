using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.DAO;
using MyClass.Model;
using PTUDW.Library;

namespace PTUDW.Areas.Admin.Controllers
{
    public class TopicController : Controller
    {
        TopicsDAO topicsDAO = new TopicsDAO();
        LinksDAO linksDAO = new LinksDAO();

        //////////////////////////////////////////////////////////////
        //INDEX
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(topicsDAO.getList("Index"));
        }

        //////////////////////////////////////////////////////////////
        //DETAIL
        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Không tồn tại loại sản phẩm");
                return RedirectToAction("Index");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại loại sản phẩm");
                return RedirectToAction("Index");
            }

            return View(topics);
        }

        //////////////////////////////////////////////////////////////
        //CREATE
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.ListCat = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Topics topics)
        {
            if (ModelState.IsValid)
            {
                //Xử lý tự động: CreateAt
                topics.CreateAt = DateTime.Now;
                //Xử lý tự động: UpdateAt
                topics.UpdateAt = DateTime.Now;
                //Xử lý tự động: ParentId
                if (topics.ParentID == null)
                {
                    topics.ParentID = 0;
                }

                //Xử lý tự động: Order
                if (topics.Order == null)
                {
                    topics.Order = 1;
                }
                else
                {
                    topics.Order += 1;
                }

                //Xử lý tự động: SLug
                topics.Slug = XString.Str_Slug(topics.Name);

                //xu ly cho muc Topics
                if (topicsDAO.Insert(topics) == 1)//khi them du lieu thanh cong
                {
                    Links links = new Links();
                    links.Slug = topics.Slug;
                    links.TableId = topics.Id;
                    links.Type = "category";
                    linksDAO.Insert(links);
                }
                //thông báo them danh mục sản phẩm thành công
                TempData["message"] = new XMessage("success", "Tạo mới loại sản phẩm thành công");
                //trở về trang index
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View(topics);
        }

        //////////////////////////////////////////////////////////////
        //EDIT
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View(topics);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Topics topics)
        {
            if (ModelState.IsValid)
            {
                //xu ly tu dong: Slug
                topics.Slug = XString.Str_Slug(topics.Name);
                //xu ly tu dong: ParentID
                if (topics.ParentID == null)
                {
                    topics.ParentID = 0;
                }
                //xu ly tu dong: Ordẻr
                if (topics == null)
                {
                    topics.Order = 1;
                }
                else
                {
                    topics.Order += 1;
                }

                //xu ly tu dong: UpdateAt
                topics.UpdateAt = DateTime.Now;

                //Cap nhat du lieu, sua them cho phan Links phuc vu cho Topics
                if (topicsDAO.Update(topics) == 1)
                {
                    //Neu trung khop thong tin: Type = category va TableID = topics.ID
                    Links links = linksDAO.getRow(topics.Id, "Category");
                    //cap nhat lai thong tin
                    links.Slug = topics.Slug;
                    linksDAO.Update(links);
                }

                //thong bao thanh cong
                TempData["message"] = new XMessage("success", "Chỉnh sửa loại hàng thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View(topics);
        }

        //////////////////////////////////////////////////////////////
        //DELETE
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            return View(topics);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Topics topics = topicsDAO.getRow(id);

            //tim thay mau tin thi xoa, cap nhat cho Links
            if (topicsDAO.Delete(topics) == 1)
            {
                Links links = linksDAO.getRow(topics.Id, "Category");
                //Xoa luon cho Links
                linksDAO.Delete(links);
            }

            //thông báo thành công
            TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
            return RedirectToAction("Trash");
        }

        //////////////////////////////////////////////////////////////
        //STATUS
        // GET: Admin/Category/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //truy vấn dòng có id  = id yêu cầu
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {

                //chuyển đổi trạng thái của status 1=>2, và ngược lại
                topics.Status = (topics.Status == 1) ? 2 : 1;

                //cập nhật giá trị UpdateAt
                topics.UpdateAt = DateTime.Now;

                //cập nhật lại DB
                topicsDAO.Update(topics);

                //thông báo cập nhật trạng thái thành công
                TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");

                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////
        //DELTRASH
        // GET: Admin/Category/DelTrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin để xóa");
                return RedirectToAction("Index");
            }
            //truy vấn dòng có id  = id yêu cầu
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            else
            {

                //chuyển đổi trạng thái của status 1,2 => 0:không hiển thị ở Index
                topics.Status = 0;

                //cập nhật giá trị UpdateAt
                topics.UpdateAt = DateTime.Now;

                //cập nhật lại DB
                topicsDAO.Update(topics);

                //thông báo cập nhật trạng thái thành công
                TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////
        //TRASH
        // GET: Admin/Category/Trash
        public ActionResult Trash()
        {
            return View(topicsDAO.getList("Trash"));
        }

        //////////////////////////////////////////////////////////////
        //RECOVER
        // GET: Admin/Category/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            //truy vấn dòng có id  = id yêu cầu
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            else
            {

                //chuyển đổi trạng thái của status 0=>2:không xuất bản
                topics.Status = 2;

                //cập nhật giá trị UpdateAt
                topics.UpdateAt = DateTime.Now;

                //cập nhật lại DB
                topicsDAO.Update(topics);

                //thông báo phục hồi dữ liệu thành công
                TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }
    }
}
