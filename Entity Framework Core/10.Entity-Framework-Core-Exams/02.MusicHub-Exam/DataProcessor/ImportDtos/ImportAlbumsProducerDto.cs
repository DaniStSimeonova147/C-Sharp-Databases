using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.DataProcessor.ImportDtos
{
    public class ImportAlbumsProducerDto
    {
        [MinLength(3), MaxLength(40), Required]
        public string Name { get; set; }

        [Required]
        public string ReleaseDate { get; set; }
    }
}
