namespace rbkApiModules.Commons.Core;

public class QueryResponse : BaseResponse
{
    public QueryResponse() : base()
    {
    }

    internal QueryResponse(object result) : base(result)
    {
    }

    public static QueryResponse Success(object result)
    {
        return new QueryResponse(result);
    }
}