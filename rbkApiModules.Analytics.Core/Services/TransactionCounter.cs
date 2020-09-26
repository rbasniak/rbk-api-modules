using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Analytics.Core
{
    public interface ITransactionCounter
    {
        int Transactions { get; set; }
        double TotalTime { get; set; }
    }

    public class TransactionCounter: ITransactionCounter
    {
        public int Transactions { get; set; }
        public double TotalTime { get; set; }
    }
}
