﻿using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Models
{
	public class Tag
	{
        public  int Id { get; set; }
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
