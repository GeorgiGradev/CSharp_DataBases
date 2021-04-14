using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("customer")]
    public class ExportTotalSalesByCustomer
    {
        [XmlAttribute("full-name")]
        public string FullName { get; set; }
        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }
        [XmlAttribute("spent-money")]
        public decimal SpentMoney { get; set; }
    }
}
