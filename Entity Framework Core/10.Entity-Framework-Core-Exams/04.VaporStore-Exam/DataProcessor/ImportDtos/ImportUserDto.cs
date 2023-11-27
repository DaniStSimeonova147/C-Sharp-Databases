using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.DataProcessor.ImportDtos
{
   public class ImportUserDto
    {
        [MinLength(3), MaxLength(20), Required]
        public string Username { get; set; }

        [RegularExpression(@"^[A-Z]{1}[a-z]+ [A-Z]{1}[a-z]+$"), Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3, 103), Required]
        public int Age { get; set; }
    
        [MinLength(1)]
        public ICollection<ImportCardDto> Cards { get; set; }
    }
}
