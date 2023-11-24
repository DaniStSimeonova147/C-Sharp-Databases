using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        public Performer()
        {
            this.PerformerSongs = new HashSet<SongPerformer>();
        }

        [Key]
        public int Id { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string FirstName { get; set; }

        [MinLength(3), MaxLength(20), Required]
        public string LastName { get; set; }

        [Range(18, 70), Required]
        public int Age { get; set; }

        [Range(0, double.MaxValue), Required]
        public decimal NetWorth  { get; set; }

        public ICollection<SongPerformer> PerformerSongs  { get; set; }
    }
}
