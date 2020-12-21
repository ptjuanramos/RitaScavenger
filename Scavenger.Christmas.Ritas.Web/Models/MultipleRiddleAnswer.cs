using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scavenger.Christmas.Ritas.Web.Models
{
    public class MultipleRiddleAnswer : RiddleAnswer
    {
        public Dictionary<int, string> Answers { get; set; }
    }
}
