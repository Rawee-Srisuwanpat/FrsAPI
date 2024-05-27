using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace field_recording_api.Utilities
{
    public static class ObjectMapper
    {
        /////////////////////////////////////////////////////////
        //      Convert Data Table To List Object
        /////////////////////////////////////////////////////////

        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }


        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        string data_type = pro.PropertyType.FullName;

                        if (data_type == typeof(string).FullName)
                        {
                            pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                        }
                        else if (data_type == typeof(int).FullName)
                        {
                            pro.SetValue(obj, Convert.ToInt32(dr[column.ColumnName]), null);
                        }
                        else
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }

                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        public static List<T> ConvertDataTableToList2<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem2<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem2<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (ConvertFieldToDbFormatName(pro.Name).Equals(column.ColumnName))
                    {
                        pro.SetValue(obj, dr[column.ColumnName].Equals(System.DBNull.Value) ? null : dr[column.ColumnName], null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        private static string ConvertFieldToDbFormatName(string propertyName)
        {
            StringBuilder str = new StringBuilder("");
            int index = 0;
            foreach (char c in propertyName)
            {
                if (index == 0)
                {
                    str.Append(char.ToLower(c));
                }
                else
                {
                    if (char.IsUpper(c))
                    {
                        str.Append("_" + char.ToLower(c));
                    }
                    else
                    {
                        str.Append(c);
                    }
                }
                index++;
            }
            return str.ToString();
        }

        public static T ConvertToDataObject<T>(this DataRow row)
        {
            Type objectType = typeof(T);
            T newObect = (T)Activator.CreateInstance(objectType);
            if (newObect == null)
                return default(T);

            foreach (DataColumn column in row.Table.Columns)
            {
                if (row[column] == DBNull.Value)
                    continue;

                object Value = row[column];
                MemberInfo[] memberInfos = objectType.GetMember(column.ColumnName);
                if (memberInfos != null && memberInfos.Length > 0)
                {
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        switch (memberInfo.MemberType)
                        {
                            case MemberTypes.Property:
                                {
                                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                                    if (propertyInfo == null)
                                        continue;

                                    if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
                                    {
                                        Type nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                                        Value = Convert.ChangeType(Value, nullableType);
                                    }

                                    propertyInfo.SetValue(newObect, Value, null);
                                    break;
                                }
                            case MemberTypes.Field:
                                {
                                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                                    if (fieldInfo == null)
                                        continue;

                                    if (Nullable.GetUnderlyingType(fieldInfo.FieldType) != null)
                                    {
                                        Type nullableType = Nullable.GetUnderlyingType(fieldInfo.FieldType);
                                        Value = Convert.ChangeType(Value, nullableType);
                                    }

                                    fieldInfo.SetValue(newObect, Value);
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }

            return (T)newObect;
        }

        public static List<T> ConvertDataTableToListFromExcel<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = ConvertDataTableToObjectFromExcel<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T ConvertDataTableToObjectFromExcel<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        string data_type = pro.PropertyType.FullName;
                        if (data_type == typeof(string).FullName)
                        {
                            pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                        }
                        else if (data_type == typeof(int).FullName)
                        {
                            pro.SetValue(obj, Convert.ToInt32(dr[column.ColumnName]), null);
                        }
                        else if (data_type == typeof(DateTime).FullName)
                        {

                            pro.SetValue(obj, ConvertDateYYYYMMDD(dr[column.ColumnName]), null);
                        }
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        /////////////////////////////////////////////////////////
        //      Convert Data Table To JSON Data
        /////////////////////////////////////////////////////////

        //1. Convert DataTable to JSON using StringBuilder.  Format: {"firstName":"Satinder", "lastName":"Singh"}

        public static string ConvertDataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }




        //3. Convert DataTable to JSON using JSON.Net DLL (Newtonsoft).|using Newtonsoft.JSON; |
        public static string ConvertDataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }


        /////////////////////////////////////////////////////////
        //      Convert List Ojbect Or Oject To JSON Data
        /////////////////////////////////////////////////////////

        public static string ConvertListObjectToJSON<T>(List<T> ls)
        {
            return JsonConvert.SerializeObject(ls, Formatting.Indented);
        }

        public static string ConvertCollectionObjectToJSON<T>(ICollection<T> ls)
        {
            return JsonConvert.SerializeObject(ls, Formatting.Indented);
        }

        public static string ConvertObjectToJSON(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }


        /////////////////////////////////////////////////////////
        //      Convert JSON Data To Oject Or List Object
        /////////////////////////////////////////////////////////

        public static List<T> ConvertJSONToListObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public static object ConvertJSONToOject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }


        /////////////////////////////////////////////////////////
        //      Convert List Object To Data Table
        /////////////////////////////////////////////////////////
        public static DataTable ConvertListObjectToDataTable<T>(List<T> ls)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in ls)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        /////////////////////////////////////////////////////////
        //      Convert JSON Data  To Data Table
        /////////////////////////////////////////////////////////

        public static DataTable ConvertJSONToDataTable(string json)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(json.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }

        public static DataTable ConvertJSONToDataTableByDeserialize(string json)
        {
            return (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
        }


        public static DateTime ConvertDateYYYYMMDD(object value)
        {
            string[] arr = value.ToString().Split('/');
            string dt = string.Format("{0}-{1}-{2}", arr[2], arr[1], arr[0]);

            return Convert.ToDateTime(dt);
        }

        public static List<T> DataTableToJSONWithJSONNet<T>(DataTable table)
        {
            List<T> data = new List<T>();
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            data = JsonConvert.DeserializeObject<List<T>>(JSONString);
            return data;

        }
    }
}
