using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjA.Models;
using System.Security.Cryptography;
using System.Collections;

namespace ProjA.Controllers
{
    public class UsersController : Controller
    {
        private Database1Entities3 db = new Database1Entities3();

        // GET: Users
        public async Task<ActionResult> Index()
        {
            var users = db.Users.Include(u => u.area1);
            return View(await users.ToListAsync());
        }

        public async Task<ActionResult> MyZone()
        {
            ViewBag.manufacturer = new SelectList(db.manufacturer, "manufacturerName", "manufacturerName");
            ViewBag.itemType = new SelectList(db.itemType, "itemTypeName", "itemTypeName");
            return await Task.Run(() => View());
        }

        
        // GET: Users/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.area = new SelectList(db.area, "areaname", "areaname");
            return await Task.Run(() => View());
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "userName,firstName,lastName,mail,area,phoneNumber,isAdmin,password")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(users);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.area = new SelectList(db.area, "areaname", "areaname", users.area);
            return View(users);
        }
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public async Task<JsonResult> Creation(Users data)
        {
            if (ModelState.IsValid)
            {

                Users[] users = await db.Users.ToArrayAsync();
                foreach (Users user in users)
                {
                    if (user.userName.ToLower().Equals(data.userName.ToLower()) && user.password.Equals(data.password))
                    {
                        //Session["user"] = user.userName;
                        return await Task.Run(() => Json(2, JsonRequestBehavior.AllowGet));
                    }
                }
                Users u = new Users();
                u.firstName = data.firstName;
                u.lastName = data.lastName;
                u.mail = data.mail;
                u.area = data.area;
                u.userName = data.userName.ToLower();
                u.phoneNumber = data.phoneNumber;
                u.salt = Users.GenerateRandomSalt();
                u.password = Users.doHash(data.password, u.salt);
                u.isAdmin = "no";
                db.Users.Add(u);
                await db.SaveChangesAsync();
                // Session["user"] = u.userName;
                return await Task.Run(() => Json(1, JsonRequestBehavior.AllowGet));
            }
            return await Task.Run(() => Json(0, JsonRequestBehavior.AllowGet));

        }

        public async Task<ActionResult> Login()
        {
            return await Task.Run(() => View());
        }


        public async Task<JsonResult> Logout()
        {
            Session["user"] = null;
            return await Task.Run(() => Json(1, JsonRequestBehavior.AllowGet));
        }


        public async Task<JsonResult> checkLogin(Users data)
        {
            if (ModelState.IsValid)
            {
                Users[] users = await db.Users.ToArrayAsync();
                foreach (Users u in users)
                {
                    if (u.userName.ToLower().Equals(data.userName.ToLower()) && u.password.Equals(Users.doHash(data.password, u.salt)))
                    {
                        Session["pur"] = await db.Purchases.ToArrayAsync();
                        Session["user1"] = u;
                        Session["user"] = u.userName;
                        Items[] items = await db.Items.ToArrayAsync();
                        Session["items"] = items;
                        return await Task.Run(() => Json(1, JsonRequestBehavior.AllowGet));
                    }
                }
            }
            return await Task.Run(() => Json(0, JsonRequestBehavior.AllowGet));

        }

        public async Task<JsonResult> AddItem(NewItem data)
        {
            if (ModelState.IsValid)
            {
                Items item = new Items();
                item.C_description = data.description;
                item.image = data.image;
                Items[] ic = await db.Items.ToArrayAsync();
                item.itemNumber = 1000 + ic.Length;
                if (ic.Length>0) { 
                int max = ic[0].itemNumber;
                
                    for (int i = 0; i < ic.Length; i++)
                    {
                        if (ic[i].itemNumber > max)
                        {
                            max = ic[i].itemNumber;
                        }
                    }
                    item.itemNumber = max + 1;

                }
                item.itemType = data.itemType;
                item.manufacturer = data.manufacturer;
                item.modelName = data.modelName;
                item.price = data.price;
                item.views = 0;
                item.status = data.status;
                item.userName = Session["user"].ToString();

                // item.Users = (Users)Session["user1"];
                //   itemType it = await db.itemType.FindAsync(data.itemType);
                //  manufacturer m1 = await db.manufacturer.FindAsync(data.manufacturer);
                //   item.manufacturer1 = m1;
                //   item.itemType1 = it;

                db.Items.Add(item);
                await db.SaveChangesAsync();
                Session["items"] = await db.Items.ToArrayAsync();
                Session["search"] = Session["items"];
                return await Task.Run(() => Json(item.itemNumber, JsonRequestBehavior.AllowGet));

            }
            return await Task.Run(() => Json(0, JsonRequestBehavior.AllowGet));

        }


        public async Task<JsonResult> GetItemLength()
        {
            Items[] item = await db.Items.ToArrayAsync();
            if (item == null)
                return await Task.Run(() => Json(item, JsonRequestBehavior.AllowGet));
            return await Task.Run(() => Json(item, JsonRequestBehavior.AllowGet));
        }

        public async Task<JsonResult> removeItem(int data)
        {
            Items[] item = await db.Items.ToArrayAsync();
            if (item == null)
                return await Task.Run(() => Json(0, JsonRequestBehavior.AllowGet));
            foreach(Items it in item)
            {
                if (it.itemNumber == data)
                {
                    db.Items.Remove(it);
                    await db.SaveChangesAsync();
                    Session["items"] = await db.Items.ToArrayAsync();

                    return await Task.Run(() => Json(item, JsonRequestBehavior.AllowGet));
                }
            }
            return await Task.Run(() => Json(item, JsonRequestBehavior.AllowGet));
        }

        public async Task<JsonResult> Buy(int data)
        {
            int counter = 0;
            Purchases purch = new Purchases();
            Items[] it = await db.Items.ToArrayAsync();
            Purchases[] ps = await db.Purchases.ToArrayAsync();
            foreach (Items i in it)
            {
                if (data == i.itemNumber)
                {
                    purch.seller = i.userName;
                    purch.date = DateTime.Now;
                    purch.buyer = (string)Session["user"].ToString();
                    if (ps.Length == 0)
                        purch.dealNumber = 10000;
                    else
                    {
                        int max = 0;
                        foreach (Purchases p in ps)
                        {
                            if (max < p.dealNumber)
                                max = p.dealNumber;
                        }
                        purch.dealNumber = 10000 + max + 1;

                    }
                    purch.itemNumber = data;
                    purch.manufacturerName = i.manufacturer;
                    purch.modelName = i.modelName;

                    Items neww = new Items();
                    neww.C_description = i.C_description;
                    neww.image = i.image;
                    neww.itemNumber = i.itemNumber;
                    neww.itemType = i.itemType;
                    neww.itemType1 = i.itemType1;
                    neww.manufacturer = i.manufacturer;
                    neww.manufacturer1 = i.manufacturer1;
                    neww.modelName = i.modelName;
                    neww.price = i.price;
                    neww.status = i.status;
                    neww.userName = i.userName;
                    neww.Users = i.Users;
                    neww.views = 10;
                    it[counter] = neww;
                    Session["items"] = it;
                    db.Items.Remove(i);
                    db.Items.Add(neww);
                    db.Purchases.Add(purch);
                    await db.SaveChangesAsync();
                    Session["pur"] = await db.Purchases.ToArrayAsync();

                    return await Task.Run(() => Json(1, JsonRequestBehavior.AllowGet));
                }
                counter++;

            }

            return await Task.Run(() => Json(0, JsonRequestBehavior.AllowGet));
        }



    }

}