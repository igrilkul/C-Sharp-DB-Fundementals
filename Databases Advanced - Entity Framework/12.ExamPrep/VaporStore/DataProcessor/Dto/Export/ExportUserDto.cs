using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
   public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        [XmlArrayItem("Purchase")]
        public List<ExportPurchaseDto> Purchases { get; set; }

        [XmlElement("TotalSpent")]
        public decimal TotalSpent { get; set; }
    }

    public class ExportPurchaseDto
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Game")]
        public GameDto Game { get; set; }
        
    }

    public class GameDto
    {
        [XmlAttribute("title")]
        public string GameTitle { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
