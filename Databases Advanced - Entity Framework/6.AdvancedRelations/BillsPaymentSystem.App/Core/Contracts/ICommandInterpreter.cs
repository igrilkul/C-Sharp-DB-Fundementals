using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.App.Core.Contracts
{
    public interface ICommandInterpreter
    {
        string Read(string[] inputParams,BillsPaymentSystemContext context);
    }
}
