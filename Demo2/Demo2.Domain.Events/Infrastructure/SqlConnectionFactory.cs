using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Infrastructure;

[IgnoreAutomaticIocContainerRegistration]
public interface ISqlConnectionFactory
{
    SqlConnection SqlConnection();
}

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }
    public SqlConnection SqlConnection()
    {
        return new SqlConnection(this.connectionString);
    }
}