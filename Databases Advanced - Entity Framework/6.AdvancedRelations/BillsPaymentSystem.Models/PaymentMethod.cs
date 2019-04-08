using BillsPaymentSystem.Models.Attributes;
using BillsPaymentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Models
{
   public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }

        public PaymentType PaymentType { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Xor(nameof(CreditCardId))]
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        public int? CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; }
    }
}
