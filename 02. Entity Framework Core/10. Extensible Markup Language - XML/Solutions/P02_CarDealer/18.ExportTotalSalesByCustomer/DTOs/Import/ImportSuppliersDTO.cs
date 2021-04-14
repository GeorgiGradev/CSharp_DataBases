using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Supplier")]
    public class ImportSuppliersDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public bool isImporter { get; set; }
    }
}
