using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rbkApiModules.Logging.Core
{
    public class CustomException
    {
        public string ExceptionName { get; set; }
        public string ModuleName { get; set; }
        public string DeclaringTypeName { get; set; }
        public string TargetSiteName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public List<DictEntry> Data { get; set; }
        public CustomException InnerException { get; set; }

        public CustomException GetBaseError()
        {
            return InnerException != null ? InnerException.GetBaseError() : this;
        }
        public override string ToString()
        {
            return ToBetterString();
        }

        public string ToBetterString(string prepend = null)
        {
            var exceptionMessage = new StringBuilder();

            exceptionMessage.Append("\n" + prepend + "Exception:" + ExceptionName);
            exceptionMessage.Append("\n" + prepend + "Message:" + Message);

            exceptionMessage.Append("\n" + prepend + "ModuleName:" + ModuleName);
            exceptionMessage.Append("\n" + prepend + "DeclaringType:" + DeclaringTypeName);
            exceptionMessage.Append("\n" + prepend + "TargetSite:" + TargetSiteName);
            exceptionMessage.Append("\n" + prepend + "StackTrace:" + StackTrace);

            exceptionMessage.Append(GetExceptionData("\n" + prepend));

            if (InnerException != null)
                exceptionMessage.Append("\n" + prepend + "InnerException: "
                    + InnerException.ToBetterString(prepend + "\t"));

            return exceptionMessage.ToString();
        }
        private string GetExceptionData(string prependText)
        {
            var exData = new StringBuilder();
            foreach (var item in Data.Where(a => a.Value != null))
            {
                exData.Append(prependText + String.Format("DATA-{0}:{1}", item.Key,
                    item.Value));
            }

            return exData.ToString();
        }
    }

    public class DictEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
