using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Round.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Item Name is required")]
        [StringLength(200, ErrorMessage = "Item Name cannot exceed 200 characters")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be a positive number")]
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }

        [Display(Name = "Parent Item")]
        public int? ParentItemId { get; set; }

        [Display(Name = "Processed")]
        public bool IsProcessed { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public string ParentItemName { get; set; }

        public List<Item> Children { get; set; }

        public Item()
        {
            Children = new List<Item>();
            CreatedDate = DateTime.Now;
        }
    }

    public class ProcessItemViewModel
    {
        public int ParentItemId { get; set; }

        [Display(Name = "Parent Item")]
        public string ParentItemName { get; set; }

        [Display(Name = "Parent Weight")]
        public decimal ParentWeight { get; set; }

        public decimal ExistingChildWeightSum { get; set; }

        public decimal RemainingWeight
        {
            get { return ParentWeight - ExistingChildWeightSum; }
        }

        public List<ChildItemInput> ChildItems { get; set; }

        public List<Item> ExistingChildren { get; set; }

        public ProcessItemViewModel()
        {
            ChildItems = new List<ChildItemInput>();
            ExistingChildren = new List<Item>();
        }
    }

    public class ChildItemInput
    {
        [Required(ErrorMessage = "Child Item Name is required")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be a positive number")]
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }
    }

    public class ProcessedItemViewModel
    {
        public Item ParentItem { get; set; }
        public List<Item> ChildItems { get; set; }

        public ProcessedItemViewModel()
        {
            ChildItems = new List<Item>();
        }
    }
}
