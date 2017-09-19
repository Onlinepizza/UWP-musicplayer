using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonathanProjectOffline.Models
{
    public class MenuItem
    {
        public string Index { get; set; }
        public string DisplayText { get; set; }
        public Category category { get; set; }
    }

}
