using BookShop.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class ImportBookDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3)]  // Може да се валидира само тук
        [MaxLength(30)]
        public string Name { get; set; }

        [XmlElement("Genre")]
        [Range(1, 3)] // Така се валидира дали са верни входните данни
        public int Genre { get; set; }
        //Подава се като INT, за да може да се валидира дали входните данни са верни
        //manually CAST TO ENUM

        [XmlElement("Price")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [XmlElement("Pages")]
        [Range(50, 5000)]
        public int Pages { get; set; }

        
       [XmlElement("PublishedOn")]
        public string PublishedOn { get; set; }
        //подава се като STRING, за да може да се валидира, ако входните данни не са верни
        // DateTime.TryParse()
        // Дата ще се parse-не при проверката)
    }
}
