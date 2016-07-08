﻿using MultiTenancyFramework.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenancyFramework.IO
{
    public class CsvWriter
    {
        /// <summary>
        /// Creates a CSV file in the path given by <paramref name="fullFileName"/>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fullFileName"></param>
        /// <param name="replaceNullsWithDefaultValue">Replace Null Values With default values in the generated files</param>
        /// <param name="delimiter">The delimiter. It's comma by default</param>
        public static void CreateCSVfile(MyDataTable dataTable, string fullFileName, bool replaceNullsWithDefaultValue = false, string delimiter = ",")
        {
            using (StreamWriter sw = new StreamWriter(fullFileName, false))
            {
                WriteFile(sw, dataTable, null, replaceNullsWithDefaultValue, delimiter);
            }
        }

        /// <summary>
        /// Creates a CSV file and return it as byte array
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="headerItems"></param>
        /// <param name="replaceNullsWithDefaultValue"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static async Task<byte[]> CreateCSVfile(MyDataTable dataTable, IDictionary<string, object> headerItems = null, bool replaceNullsWithDefaultValue = false, string delimiter = ",")
        {
            return await Task.Run(() =>
            {
                byte[] theByte = null;
                using (var memory = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(memory))
                    {
                        WriteFile(sw, dataTable, headerItems, replaceNullsWithDefaultValue, delimiter, true);
                    }
                    theByte = memory.ToArray();
                }
                return theByte;
            });
        }

        private static void WriteFile(StreamWriter sw, MyDataTable dataTable, IDictionary<string, object> headerItems = null,
            bool replaceNullsWithDefaultValue = false, string delimiter = ",", bool doColumnNames = false)
        {
            // General Titles
            if (headerItems != null && headerItems.Count > 0)
            {
                foreach (var item in headerItems)
                {
                    sw.WriteLine(item.Key + delimiter + item.Value);
                    sw.Flush();
                }
            }

            if (doColumnNames)
            {
                // The Column Titles
                string[] columnNames = dataTable.Columns.Cast<MyDataColumn>().Select(column => column.ColumnName).ToArray();
                sw.WriteLine(string.Join(delimiter, columnNames));
                sw.Flush();
            }

            // The rows
            int icolcount = dataTable.Columns.Count;
            int i;
            foreach (MyDataRow drow in dataTable.Rows)
            {
                i = 0;
                foreach (var column in drow)
                {
                    #region Write cell data
                    object valueToWrite = null;
                    if (!Convert.IsDBNull(column.Value))
                    {
                        valueToWrite = column.Value;
                        MyDataColumn dataCol = dataTable.Columns[column.Key];
                        if (dataCol.DataType == typeof(bool))
                        {
                            valueToWrite = valueToWrite.ToString().ToLower() == "false" ? 0 : 1;
                        }
                        else if (dataCol.DataType == typeof(DateTime))
                        {
                            valueToWrite = Convert.ToDateTime(valueToWrite).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        sw.Write(valueToWrite);
                    }
                    else
                    {
                        if (replaceNullsWithDefaultValue)
                        {
                            MyDataColumn dataCol = dataTable.Columns[column.Key];
                            if (dataCol.DataType == typeof(DateTime))
                            {
                                sw.Write(new DateTime(1900, 1, 1).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else if (dataCol.DataType == typeof(bool))
                            {
                                sw.Write(0);
                            }
                            else
                            {
                                sw.Write(dataCol.DataType.GetDefaultValue());
                            }
                        }
                        else
                        {
                            sw.Write("NULL");
                        }
                    }
                    #endregion

                    if (i < icolcount - 1)
                    {
                        sw.Write(delimiter);
                    }
                    i++;
                }
                sw.Write(sw.NewLine);
            }
        }

    }
}