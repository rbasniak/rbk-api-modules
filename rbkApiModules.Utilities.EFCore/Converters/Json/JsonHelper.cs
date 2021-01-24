using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Utilities.EFCore
{
    internal static class JsonHelper
    {
        public static T Deserialize<T>(string json) where T : class
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json);
        }

        public static string Serialize<T>(T obj) where T : class
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj);
        }
    }
}
