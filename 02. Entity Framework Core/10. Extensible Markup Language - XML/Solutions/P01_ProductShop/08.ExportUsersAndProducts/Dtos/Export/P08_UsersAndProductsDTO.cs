using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Users")]
    public class P08_UsersAndProductsDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("users")]
        public List<UserDTO> Users { get; set; }
    }

    [XmlType("User")]
    public class UserDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }
        [XmlElement("lastName")]
        public string LastName { get; set; }
        [XmlElement("age")]
        public int? Age { get; set; }
        [XmlElement("SoldProducts")]
        public SoldProductsDTO SoldProducts { get; set; }
    }

    [XmlType("SoldProducts")]
    public class SoldProductsDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("products")]
        public List<ProductDTO> Products { get; set; }
    }

    [XmlType("Product")]
    public class ProductDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
