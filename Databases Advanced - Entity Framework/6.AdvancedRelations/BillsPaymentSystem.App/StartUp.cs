using BillsPaymentSystem.App.Core;
using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using System;

namespace BillsPaymentSystem.App
{
    class StartUp
    {
        static void Main(string[] args)
        {
            //Re-Creates DB and seeds tables.
            /*using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
            {
                DbInitializer dbInitializer = new DbInitializer(context);
                dbInitializer.InitializeDB(context);
            }*/

                ICommandInterpreter commandInterpreter = new CommandInterpreter();
            IEngine engine = new Engine(commandInterpreter);
            engine.Run();
        }
    }
}
