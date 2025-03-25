using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteflyUI.Models
{
    public class ApiResponse
    {
        public List<Detection> detections { get; set; }
        public int image_width { get; set; }
        public int image_height { get; set; }
    }
}
