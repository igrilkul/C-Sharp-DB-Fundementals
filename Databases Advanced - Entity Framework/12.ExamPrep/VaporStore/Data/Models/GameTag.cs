using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
   public class GameTag
    {
        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }

        [Required]
        public int TagId { get; set; }
        [Required]
        public Tag Tag { get; set; }


    }
}
