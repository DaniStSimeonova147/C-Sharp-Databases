using Cinema.Data.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportMovieDto
    { 
        //[JsonProperty("Title")]
        [MinLength(3), MaxLength(20), Required]
        public string Title { get; set; }

        [Required]
        public Genre Genre { get; set; }
        //public string Genre { get; set; } -> profile

        [Required]
        public TimeSpan Duration { get; set; }
        //public string Duration { get; set; } -> profile

        [Range(1, 10), Required]
        public double Rating { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string Director { get; set; }
    }
}
