//--------------------------------------------------------------------------------
// This file is part of the downloadable code for the Apress book:
// Pro WF: Windows Workflow in .NET 4.0
// Copyright (c) Bruce Bukovics.  All rights reserved.
//
// This code is provided as is without warranty of any kind, either expressed
// or implied, including but not limited to fitness for any particular purpose. 
// You may use the code for any commercial or noncommercial purpose, and combine 
// it with your own code, but cannot reproduce it in whole or in part for 
// publication purposes without prior approval. 
//--------------------------------------------------------------------------------      
using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace ActivityLibrary
{
    public static class TrackingRecordSerializer
    {
        public static String Serialize(TrackingRecord tr)
        {
            if (tr == null)
            {
                return String.Empty;
            }

            XElement root = new XElement(tr.GetType().Name);
            XDocument xml = new XDocument(root);

            SerializeObject(root, tr);
            return xml.ToString();
        }

        private static void SerializeObject(
            XElement parent, Object o)
        {
            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                //Console.WriteLine("Property: {0} {1}", property.Name, property.PropertyType);
                if (IsPropertyWeWant(property))
                {
                    if (property.PropertyType.IsGenericType)
                    {
                        if (property.PropertyType.Name == "IDictionary`2")
                        {
                            SerializeDictionary(property, parent, o);
                        }
                    }
                    else
                    {
                        Object value = property.GetValue(o, null);
                        parent.Add(new XElement(property.Name, value));
                    }
                }
            }
        }

        private static bool IsPropertyWeWant(PropertyInfo property)
        {
            if (property.IsDefined(
                typeof(System.Data.Linq.Mapping.AssociationAttribute), true))
            {
                //Console.WriteLine("Ignore property {0}", property.Name);
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void SerializeDictionary(
            PropertyInfo property, XElement parent, Object o)
        {
            XElement element = new XElement(property.Name);
            parent.Add(element);

            Object value = property.GetValue(o, null);
            if (value is IDictionary<String, String>)
            {
                foreach (var kvPair in (IDictionary<String, String>)value)
                {
                    SerializeKeyValuePair(element, kvPair.Key, kvPair.Value);
                }
            }
            else if (value is IDictionary<String, Object>)
            {
                foreach (var kvPair in (IDictionary<String, Object>)value)
                {
                    SerializeKeyValuePair(element, kvPair.Key, kvPair.Value);
                }
            }
        }

        private static void SerializeKeyValuePair(
            XElement element, Object key, Object value)
        {
            if (value == null)
            {
                return;
            }

            Type type = value.GetType();
            if (type.IsPrimitive || type == typeof(String))
            {
                element.Add(new XElement("item",
                    new XAttribute("key", key),
                    new XAttribute("value", value)));
            }
            else
            {
                XElement valueElement = new XElement("value");
                //recursive call to serialize the value object
                SerializeObject(valueElement, value);
                element.Add(new XElement("item",
                    new XAttribute("key", key), valueElement));
            }
        }

    }
}
