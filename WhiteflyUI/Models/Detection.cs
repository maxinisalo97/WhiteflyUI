using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteflyUI.Models
{
    public class Detection
    {
        public double[] bbox { get; set; }
        public double confidence { get; set; }
        public int classId { get; set; }

        // Propiedades para facilitar el binding en la vista
        public double X1 => bbox != null && bbox.Length > 0 ? bbox[0] : 0;
        public double Y1 => bbox != null && bbox.Length > 1 ? bbox[1] : 0;
        public double X2 => bbox != null && bbox.Length > 2 ? bbox[2] : 0;
        public double Y2 => bbox != null && bbox.Length > 3 ? bbox[3] : 0;
        public double Width => X2 - X1;
        public double Height => Y2 - Y1;
    }
}
