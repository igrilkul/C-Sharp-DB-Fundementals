using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Dtos
{
   public class CategoryDto
    {
        public string category { get; set; }

        public int productsCount { get; set; }

        public string averagePrice { get; set; }

        public string totalRevenue { get; set; }
    }
}
