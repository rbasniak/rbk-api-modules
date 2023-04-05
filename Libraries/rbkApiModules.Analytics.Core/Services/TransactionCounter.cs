using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

/// <summary>
/// Had to creathe this interface to work as a cache for the transactions count
/// because the DbInterceptor runs in a separated scope and we cannot use the 
/// HttpContext to store data.
/// </summary>
public interface ITransactionCounter
{
    int Transactions { get; set; }
    double TotalTime { get; set; }
}

public class TransactionCounter : ITransactionCounter
{
    public int Transactions { get; set; }
    public double TotalTime { get; set; }
}