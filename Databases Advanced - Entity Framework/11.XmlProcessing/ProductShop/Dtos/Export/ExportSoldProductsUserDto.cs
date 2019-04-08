using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
   public class ExportSoldProductsUserDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string lastName { get; set; }

        [XmlArray("soldProducts")]
        public HashSet<ExportSoldProductsProductDto> SoldProducts { get; set; } = new HashSet<ExportSoldProductsProductDto>();
    }
}
