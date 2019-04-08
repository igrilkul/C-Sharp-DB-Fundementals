using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Hall
    {
        public Hall()
        {
            this.Projections = new HashSet<Projection>();
            this.Seats = new HashSet<Seat>();
        }

        public int Id { get; set; }

        [Required]
        [MinLength(3),MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public bool Is4Dx { get; set; }
        [Required]
        public bool Is3D { get; set; }

        public ICollection<Projection> Projections { get; set; }

        public ICollection<Seat> Seats { get; set; }
    }
}
