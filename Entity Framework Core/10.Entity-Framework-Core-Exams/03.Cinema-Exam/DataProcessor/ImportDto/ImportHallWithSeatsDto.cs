using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportHallWithSeatsDto
    {
        [MinLength(3), MaxLength(20), Required]
        public string Name { get; set; }

        public bool Is4Dx { get; set; }

        public bool Is3D { get; set; }

        [JsonProperty("Seats")]
        [Range(1, int.MaxValue)]
        public int SeatsCount { get; set; }
    }
}
