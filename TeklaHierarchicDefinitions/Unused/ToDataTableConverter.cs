﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace TeklaHierarchicDefinitions.TeklaAPIUtils
{
    /// <summary>
    /// Старая реализация
    /// </summary>
    static class ToDataTableConverter
    {
        //https://stackoverflow.com/questions/18746064/using-reflection-to-create-a-datatable-from-a-class
        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(T).FullName;
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
