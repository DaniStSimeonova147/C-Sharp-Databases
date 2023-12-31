﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enum;

namespace VaporStore.DataProcessor.ImportDtos
{
    public class ImportCardDto
    {
        [RegularExpression(@"^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$"), Required]
        public string Number { get; set; }

        [RegularExpression(@"[0-9]{3}"), Required]
        public string CVC { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
