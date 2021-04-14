using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("sale")]
    public class ExportSalesWithAppliedDiscountDTO
    {
        [XmlElement("car")]
        public CarDTO Car { get; set; }
        [XmlElement("discount")]
        public decimal Discount { get; set; }
        [XmlElement("customer-name")]
        public string CustomerName { get; set; }
        [XmlElement("price")]
        public decimal Price { get; set; }
        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }

    }
    [XmlType("car")]
    public class CarDTO
    {
        [XmlAttribute("make")]
        public string Make { get; set; }
        [XmlAttribute("model")]
        public string Model { get; set; }
        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

    }
}
