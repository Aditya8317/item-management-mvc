using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Round.Models;
using Round.Helpers;
using Round.Filters;

namespace Round.Controllers
{
    [AuthFilter]
    public class ItemController : Controller
    {
        public ActionResult Index(string searchTerm)
        {
            try
            {
                List<Item> items;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    items = DbHelper.SearchItems(searchTerm);
                    ViewBag.SearchTerm = searchTerm;
                }
                else
                {
                    items = DbHelper.GetAllItems();
                }

                return View(items);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading items: " + ex.Message;
                return View(new List<Item>());
            }
        }

        public ActionResult Create()
        {
            return View(new Item());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Item model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                DbHelper.AddItem(model.ItemName, model.Weight);
                TempData["SuccessMessage"] = "Item '" + model.ItemName + "' has been created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating item: " + ex.Message);
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                Item item = DbHelper.GetItemById(id);

                if (item == null)
                {
                    TempData["ErrorMessage"] = "Item not found.";
                    return RedirectToAction("Index");
                }

                return View(item);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading item: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Item model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                DbHelper.UpdateItem(model.ItemId, model.ItemName, model.Weight);
                TempData["SuccessMessage"] = "Item '" + model.ItemName + "' has been updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating item: " + ex.Message);
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Item item = DbHelper.GetItemById(id);

                if (item == null)
                {
                    TempData["ErrorMessage"] = "Item not found.";
                    return RedirectToAction("Index");
                }

                DbHelper.DeleteItem(id);
                TempData["SuccessMessage"] = "Item '" + item.ItemName + "' and all its children have been deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting item: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
