using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.ImportDtos
{
    public class ImportGameDto
    {
        [Required]
        public string Name { get; set; }

        [Range(0, double.MaxValue), Required]
        public decimal Price { get; set; }

        [Required]
        public string ReleaseDate { get; set; }

        [Required]
        public string Developer { get; set; }

        [Required]
        public string Genre { get; set; }

        [MinLength(1)]
        public ICollection<string> Tags { get; set; }
    }
}
