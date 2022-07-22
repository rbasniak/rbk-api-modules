using System.IO;

namespace rbkApiModules.Utilities.Excel;
    public class ExcelsDetails
    {
        /// <summary>
        /// The Excel File as base64 string
        /// </summary>
        public string File { get; set; }
        /// <summary>
        /// The filename without the extension
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// The File extension. Ex: "xlsx"
        /// </summary>
        public string FileExtension { get; set; }
    }
