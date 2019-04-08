using BillsPaymentSystem.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BillsPaymentSystem.Models
{
  public class CreditCard
    {

        public int CreditCardId { get; set; }

        [Range(typeof(decimal),"0.01", "79228162514264337593543950335")]
        public decimal Limit { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal MoneyOwed { get; set; }

        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        [ExpirationDateAttribute]
        public DateTime ExpirationDate { get; set; }

        

        public PaymentMethod PaymentMethod { get; set; }

    }
}
