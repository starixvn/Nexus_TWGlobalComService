using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace DataPostWS.DataType
{
    [DataContract]
    public class ClassBase
    {
        private static Dictionary<string, Dictionary<string, PropertyInfo>> class_properties = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        public static T CreateObjectFromReader1<T>(SqlDataReader reader) where T : new()
        {
            try
            {
                if (!reader.Read()) return default(T);

                System.Type type = typeof(T);
                ConstructorInfo ctor = type.GetConstructor(System.Type.EmptyTypes);
                Dictionary<string, PropertyInfo> dicProperties = GetTypeProperties<T>();

                T obj = new T();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo p;
                    if (!dicProperties.TryGetValue(reader.GetName(i).ToLower(), out p))
                        continue;
                    object val = reader[i];
                    if (val != null && val != DBNull.Value)
                    {
                        try
                        {
                            p.SetValue(obj, val, null);
                        }
                        catch (InvalidCastException ex)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("reader[\"{0}\"] type={1},{2},value={3}\r\n", reader.GetName(i), reader.GetFieldType(i).Name, reader.GetDataTypeName(i), reader[i]);
                            sb.AppendFormat("{0}.Property[\"{1}\"].type={2}", type.Name, p.Name, p.PropertyType.Name);
                            //Logger.Log(sb.ToString(), ex);
                        }
                        catch (Exception ex)
                        {
                            //   Logger.Log(string.Format("{0}.[\"{1}\"].SetValue(\"{2}\") fail !", type.Name, p.Name, val), ex);
                        }
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                //   Logger.Log("CreateObject fail!", ex);
            }
            return default(T);
        }
        public static T[] CreateObjectFromReader<T>(SqlDataReader reader) where T : new()
        {
            List<T> lst = new List<T>();
            try
            {
                T obj = default(T);
                while ((obj = CreateObjectFromReader1<T>(reader)) != null)
                {
                    lst.Add(obj);
                }
            }
            catch (Exception ex)
            {
                //Logger.Log("CreateObject fail!", ex);
            }

            return lst.ToArray();
        }
        private static Dictionary<string, PropertyInfo> GetTypeProperties<T>()
        {
            System.Type type = typeof(T);
            Dictionary<string, PropertyInfo> dicProperties;
            if (!class_properties.TryGetValue(type.Name, out dicProperties))
            {
                dicProperties = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo p in type.GetProperties())
                {
                    if (p.CanWrite == false) continue;
                    if (dicProperties.ContainsKey(p.Name.ToLower())) continue;
                    dicProperties.Add(p.Name.ToLower(), p);
                }
                class_properties.Add(type.Name, dicProperties);
            }
            return dicProperties;
        }
    }
}
