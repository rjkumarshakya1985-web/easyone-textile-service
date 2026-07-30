using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Textile.Core.Entities.DbEnitites
{
    public class Gst: DatabaseEntity<int>
    {
        public int GstValue { get; set; }
    }
}
