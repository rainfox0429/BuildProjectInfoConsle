namespace Utilities
{
    using System.Collections.Generic;
    using System.IO;

    using OfficeOpenXml;

    /// <summary>
    /// The ExcelHelper class
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// Saves the specified row.
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <param name="nugetInfos">The nuget infos.</param>
        public static void SaveProjectInfo(string pathFile, List<object[]> nugetInfos)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Worksheet1");

                var headerRow = new List<string[]>() { new string[] { "Project Name", "Old version", "New version" } };

                // Determine the header range (e.g. A1:D1)
                string headerRange = "A1:" + "C1";

                // Target a worksheet
                var worksheet = excel.Workbook.Worksheets["Worksheet1"];

                // Popular header row data
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                worksheet.Cells[2, 1].LoadFromArrays(nugetInfos);


                FileInfo excelFile = new FileInfo(pathFile);
                excel.SaveAs(excelFile);
            }
        }
    }
}
