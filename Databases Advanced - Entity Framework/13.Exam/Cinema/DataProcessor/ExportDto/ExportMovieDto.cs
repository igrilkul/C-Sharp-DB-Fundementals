using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.DataProcessor.ExportDto
{
   public class ExportMovieDto
    {
        public string MovieName { get; set; }

        public string Rating { get; set; }

        public string TotalIncomes { get; set; }

        public List<CustomerDto> Customers { get; set; }
    }

    public class CustomerDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Balance { get; set; }

    }
}
