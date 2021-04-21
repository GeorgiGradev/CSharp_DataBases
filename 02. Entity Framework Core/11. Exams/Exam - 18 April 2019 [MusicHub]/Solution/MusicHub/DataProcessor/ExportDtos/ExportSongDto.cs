using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ExportDtos
{
    [XmlType("Song")]
    public class ExportSongDto
    {
        [XmlElement("SongName")]
        public string SongName { get; set; }

        [XmlElement("Writer")]
        public string WriterName { get; set; }

        [XmlElement("Performer")]
        public string PerformerName { get; set; }

        [XmlElement("AlbumProducer")]
        public string ProducerName { get; set; }

        [XmlElement("Duration")]
        public string Duration { get; set; }
    }
}
