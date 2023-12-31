﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Department
    {
        //•	Id – integer, Primary Key
        //•	Name – text with min length 3 and max length 25 (required)
        //•	Cells - collection of type Cell
        public Department()
        {
            this.Cells = new HashSet<Cell>();
        }
        [Key]
        public int Id { get; set; }

        [MinLength(3), MaxLength(25), Required]
        public string Name { get; set; }

        public ICollection<Cell> Cells { get; set; }
    }
}
