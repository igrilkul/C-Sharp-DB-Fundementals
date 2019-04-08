using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Car")]
   public class ImportCarDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray(ElementName = "parts")]
        [XmlArrayItem(ElementName = "partId")]
        public PartIdDto[] parts { get; set; }
    }

    

    [XmlType("partId")]
    public class PartIdDto
    {
        [XmlAttribute("id")]
        public int PartId { get; set; }
    }
}
