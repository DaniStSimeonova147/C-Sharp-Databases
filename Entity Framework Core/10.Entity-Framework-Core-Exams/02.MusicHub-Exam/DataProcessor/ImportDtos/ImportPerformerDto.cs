using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Performer")]
    public class ImportPerformerDto
    {
        [XmlElement("FirstName")]
        [MinLength(3), MaxLength(20), Required]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        [MinLength(3), MaxLength(20), Required]
        public string LastName { get; set; }

        [XmlElement("Age")]
        [Range(18, 70), Required]
        public int Age { get; set; }

        [XmlElement("NetWorth")]
        [Range(0, double.MaxValue), Required]
        public decimal NetWorth { get; set; }

        [XmlArray("PerformersSongs")]
        public ImportPerformerSongsDto[] PerformersSongs { get; set; }
    }
}
