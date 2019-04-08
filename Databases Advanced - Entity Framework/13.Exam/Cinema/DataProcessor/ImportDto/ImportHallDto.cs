using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportHallDto
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public bool Is4Dx { get; set; }

        [Required]
        public bool Is3D { get; set; }

        [Required]
        [Range(typeof(int),"1", "2147483647")]
        public int Seats { get; set; }
    }
}
