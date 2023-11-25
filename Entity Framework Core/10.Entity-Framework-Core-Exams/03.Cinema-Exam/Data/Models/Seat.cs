using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cinema.Data.Models
{
    public class Seat
    {
        //•	Id – integer, Primary Key
        //•	HallId – integer, foreign key(required)
        //•	Hall – the seat’s hall

        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Hall)), Required]
        public int HallId { get; set; }
        public Hall Hall { get; set; }
    }
}
