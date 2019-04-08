using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BillsPaymentSystem.Models
{
   public class User
    {
        public int UserId { get; set; }

        [Required]
        [MinLength(3),MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3),MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
        public string Email { get; set; }

        [Required]
        [MinLength(6),MaxLength(20)]
        public string Password { get; set; }

        //public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();

        //public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    }
}
