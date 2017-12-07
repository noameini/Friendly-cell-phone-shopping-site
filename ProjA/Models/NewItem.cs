using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjA.Models
{
    public class NewItem
    {
        [Required]
        [Display(Name = "Manufacturer")]
        public string manufacturer { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public string itemType { get; set; }

        [Required]
        [Display(Name = "Model Name")]
        public string modelName { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string status { get; set; }

        [Required]
        [Display(Name = "Price")]
        public int price { get; set; }

        [Display(Name = "Image")]
        public string image { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }
    }
}