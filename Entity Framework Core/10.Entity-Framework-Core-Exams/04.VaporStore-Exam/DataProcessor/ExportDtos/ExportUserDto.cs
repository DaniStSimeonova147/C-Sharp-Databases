﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDtos
{
    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public ExportPurchaseUserDto[] Purchases { get; set; }

        [XmlElement("TotalSpent")]
        public decimal TotalSpent { get; set; }
    }
}
