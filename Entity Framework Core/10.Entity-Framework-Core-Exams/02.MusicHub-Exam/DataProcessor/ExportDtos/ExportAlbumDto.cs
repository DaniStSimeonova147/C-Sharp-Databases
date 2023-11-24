using System;
using System.Collections.Generic;
using System.Text;

namespace MusicHub.DataProcessor.ExportDtos
{
   public class ExportAlbumDto
    {
        public string AlbumName { get; set; }

        public string ReleaseDate { get; set; }

        public string ProducerName { get; set; }

        public ICollection<ExportSongDto> Songs { get; set; }

        public string AlbumPrice { get; set; }
    }
}
