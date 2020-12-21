using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scavenger.Christmas.Ritas.Web.Models
{
    public class Riddle
    {
        [Required]
        public int Number { get; set; }

        public SingleRiddleAnswer SingleRiddleAnswer { get; set; }

        public MultipleRiddleAnswer MultipleRiddleAnswer { get; set; }
    }
}
