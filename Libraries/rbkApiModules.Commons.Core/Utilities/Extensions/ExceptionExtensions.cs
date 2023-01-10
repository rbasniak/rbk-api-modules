using System.Text;

namespace rbkApiModules.Commons.Core;

public static class ExceptionExtensions
{
    public static string ToBetterString(this Exception ex, string prepend = null)
    {
        if (ex == null) return "";

        var exceptionMessage = new StringBuilder();

        exceptionMessage.Append(Environment.NewLine + prepend + "Exception:" + ex.GetType());
        exceptionMessage.Append(Environment.NewLine + prepend + "Message:" + ex.Message);

        exceptionMessage.Append(GetOtherExceptionProperties(ex, Environment.NewLine + prepend));

        exceptionMessage.Append(Environment.NewLine + prepend + "Source:" + ex.Source);
        exceptionMessage.Append(Environment.NewLine + prepend + "StackTrace:" + ex.StackTrace);

        exceptionMessage.Append(GetExceptionData(Environment.NewLine + prepend, ex));

        if (ex.InnerException != null)
            exceptionMessage.Append(Environment.NewLine + prepend + "InnerException: "
                + ex.InnerException.ToBetterString(prepend + "    "));

        return exceptionMessage.ToString();
    }

    private static string GetExceptionData(string prependText, Exception exception)
    {
        var exData = new StringBuilder();
        foreach (var key in exception.Data.Keys.Cast<object>()
            .Where(key => exception.Data[key] != null))
        {
            exData.Append(prependText + string.Format("DATA-{0}:{1}", key,
                exception.Data[key]));
        }

        return exData.ToString();
    }

    private static string GetOtherExceptionProperties(Exception exception, string s)
    {
        var allOtherProps = new StringBuilder();
        var exPropList = exception.GetType().GetProperties();

        var propertiesAlreadyHandled = new List<string>
        { "StackTrace", "Message", "InnerException", "Data", "HelpLink",
            "Source", "TargetSite" };

        foreach (var prop in exPropList
            .Where(prop => !propertiesAlreadyHandled.Contains(prop.Name)))
        {
            var propObject = exception.GetType().GetProperty(prop.Name)
                .GetValue(exception, null);
            var propEnumerable = propObject as IEnumerable<object>;

            if (propEnumerable == null || propObject is string)
                allOtherProps.Append(s + string.Format("{0} : {1}",
                    prop.Name, propObject));
            else
            {
                var enumerableSb = new StringBuilder();
                foreach (var item in propEnumerable)
                {
                    enumerableSb.Append(item + "|");
                }
                allOtherProps.Append(s + string.Format("{0} : {1}",
                    prop.Name, enumerableSb));
            }
        }

        return allOtherProps.ToString();
    }
}
