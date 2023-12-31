﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Customer
    {
        //•	Id – integer, Primary Key
        //•	FirstName – text with length[3, 20] (required)
        //•	LastName – text with length[3, 20] (required)
        //•	Age – integer in the range[12, 110] (required)
        //•	Balance - decimal (non-negative, minimum value: 0.01) (required)
        //•	Tickets - collection of type Ticket
        public Customer()
        {
            this.Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string FirstName { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string LastName { get; set; }

        [Range(12, 110), Required]
        public int Age { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335"), Required]
        public decimal Balance { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
