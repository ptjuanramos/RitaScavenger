using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scavenger.Christmas.Ritas.Web.Models
{
    public class SingleRiddleAnswer : RiddleAnswer
    {
        [Required(AllowEmptyStrings = false)]
        public string Answer { get; set; }
    }
}
