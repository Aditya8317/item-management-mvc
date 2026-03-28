using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Round.Models;
using Round.Helpers;
using Round.Filters;

namespace Round.Controllers
{
    [AuthFilter]
    public class ProcessController : Controller
    {
        public ActionResult Process(int id)
        {
            try
            {
                Item parentItem = DbHelper.GetItemById(id);

                if (parentItem == null)
                {
                    TempData["ErrorMessage"] = "Item not found.";
                    return RedirectToAction("Index", "Item");
                }

                decimal existingChildWeight = DbHelper.GetChildWeightSum(id);
                List<Item> existingChildren = DbHelper.GetChildItems(id);

                var viewModel = new ProcessItemViewModel
                {
                    ParentItemId = parentItem.ItemId,
                    ParentItemName = parentItem.ItemName,
                    ParentWeight = parentItem.Weight,
                    ExistingChildWeightSum = existingChildWeight,
                    ExistingChildren = existingChildren,
                    ChildItems = new List<ChildItemInput>
                    {
                        new ChildItemInput()
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading process form: " + ex.Message;
                return RedirectToAction("Index", "Item");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(ProcessItemViewModel model)
        {
            try
            {
                Item parentItem = DbHelper.GetItemById(model.ParentItemId);

                if (parentItem == null)
                {
                    TempData["ErrorMessage"] = "Parent item not found.";
                    return RedirectToAction("Index", "Item");
                }

                model.ParentItemName = parentItem.ItemName;
                model.ParentWeight = parentItem.Weight;
                model.ExistingChildWeightSum = DbHelper.GetChildWeightSum(model.ParentItemId);
                model.ExistingChildren = DbHelper.GetChildItems(model.ParentItemId);

                if (model.ChildItems != null)
                {
                    model.ChildItems = model.ChildItems
                        .Where(c => !string.IsNullOrWhiteSpace(c.ItemName))
                        .ToList();
                }

                if (model.ChildItems == null || model.ChildItems.Count == 0)
                {
                    ModelState.AddModelError("", "Please add at least one child item.");
                    return View(model);
                }

                for (int i = 0; i < model.ChildItems.Count; i++)
                {
                    var child = model.ChildItems[i];

                    if (string.IsNullOrWhiteSpace(child.ItemName))
                    {
                        ModelState.AddModelError("ChildItems[" + i + "].ItemName",
                            "Child item name is required.");
                    }

                    if (child.Weight <= 0)
                    {
                        ModelState.AddModelError("ChildItems[" + i + "].Weight",
                            "Child item weight must be a positive number.");
                    }
                }

                decimal newChildWeightTotal = model.ChildItems.Sum(c => c.Weight);
                decimal totalWeight = model.ExistingChildWeightSum + newChildWeightTotal;

                if (totalWeight > parentItem.Weight)
                {
                    ModelState.AddModelError("",
                        string.Format(
                            "Total child weight ({0:F2}) exceeds parent weight ({1:F2}). " +
                            "Existing child weight: {2:F2}. Remaining capacity: {3:F2}.",
                            totalWeight, parentItem.Weight,
                            model.ExistingChildWeightSum, model.RemainingWeight));
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                foreach (var child in model.ChildItems)
                {
                    DbHelper.AddItem(child.ItemName, child.Weight, model.ParentItemId);
                }

                DbHelper.MarkAsProcessed(model.ParentItemId);

                TempData["SuccessMessage"] = string.Format(
                    "Item '{0}' has been processed. {1} child item(s) created.",
                    parentItem.ItemName, model.ChildItems.Count);

                return RedirectToAction("ProcessedItems");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error processing item: " + ex.Message);
                model.ExistingChildren = DbHelper.GetChildItems(model.ParentItemId);
                return View(model);
            }
        }

        public ActionResult ProcessedItems()
        {
            try
            {
                List<Item> processedItems = DbHelper.GetProcessedItems();
                var viewModels = new List<ProcessedItemViewModel>();

                foreach (var item in processedItems)
                {
                    viewModels.Add(new ProcessedItemViewModel
                    {
                        ParentItem = item,
                        ChildItems = DbHelper.GetChildItems(item.ItemId)
                    });
                }

                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading processed items: " + ex.Message;
                return View(new List<ProcessedItemViewModel>());
            }
        }

        public ActionResult TreeView()
        {
            try
            {
                List<Item> tree = DbHelper.GetItemTree();
                return View(tree);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading tree view: " + ex.Message;
                return View(new List<Item>());
            }
        }
    }
}
