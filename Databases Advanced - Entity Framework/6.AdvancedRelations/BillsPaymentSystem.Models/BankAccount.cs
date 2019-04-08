using System;
using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [Required]
        [MinLength(3),MaxLength(50)]
        public string BankName { get; set; }

        [Required]
        [MinLength(3),MaxLength(20)]
        public string SwiftCode { get; set; }

        

        public PaymentMethod PaymentMethod { get; set; }
    }
}
