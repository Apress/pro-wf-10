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
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ActivityLibrary
{
    internal class FileSystemInstanceStoreIO
    {
        private String _dataDirectory = String.Empty;

        public FileSystemInstanceStoreIO()
        {
            CreateDataDirectory();
        }

        #region Save Methods

        public Boolean SaveAllInstanceData(Guid instanceId,
            SaveWorkflowCommand command)
        {
            Boolean isExistingInstance = false;
            try
            {
                String fileName = String.Format("{0}.xml", instanceId);
                String fullPath = Path.Combine(_dataDirectory, fileName);
                isExistingInstance = File.Exists(fullPath);

                XElement root = new XElement("Instance");
                root.Add(new XAttribute("InstanceId", instanceId));
                XDocument xml = new XDocument(root);

                NetDataContractSerializer serializer = 
                    new NetDataContractSerializer();

                XElement section = new XElement("InstanceData");
                root.Add(section);
                foreach (var entry in command.InstanceData)
                {
                    SaveSingleEntry(serializer, section, entry);
                }
                SaveInstanceDocument(fullPath, xml);
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                    "SaveAllInstanceData Exception: {0}", exception.Message);
                throw exception;
            }
            return isExistingInstance;
        }

        public void SaveAllInstanceMetaData(Guid instanceId,
            SaveWorkflowCommand command)
        {
            try
            {
                String fileName = String.Format("{0}.meta.xml", instanceId);
                String fullPath = Path.Combine(_dataDirectory, fileName);

                XElement root = new XElement("Instance");
                root.Add(new XAttribute("InstanceId", instanceId));
                XDocument xml = new XDocument(root);

                NetDataContractSerializer serializer = 
                    new NetDataContractSerializer();

                XElement section = new XElement("InstanceMetadata");
                root.Add(section);
                foreach (var entry in command.InstanceMetadataChanges)
                {
                    SaveSingleEntry(serializer, section, entry);
                }
                SaveInstanceDocument(fullPath, xml);
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                    "SaveAllMetaData Exception: {0}", exception.Message);
                throw exception;
            }
        }

        private void SaveSingleEntry(NetDataContractSerializer serializer, 
            XElement section, KeyValuePair<XName, InstanceValue> entry)
        {
            if (entry.Value.IsDeletedValue)
            {
                return;
            }

            XElement entryElement = new XElement("Entry");
            section.Add(entryElement);
            Serialize(serializer, entryElement, "Key", entry.Key);
            Serialize(serializer, entryElement, "Value", entry.Value.Value);
            Serialize(serializer, entryElement, "Options", entry.Value.Options);
        }

        private static void SaveInstanceDocument(String fullPath, XDocument xml)
        {
            using (FileStream stream =
                new FileStream(fullPath, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteRaw(xml.ToString());
                }
            }
        }

        #endregion

        #region Load Methods

        public Boolean LoadInstance(Guid instanceId,
            out IDictionary<XName, InstanceValue> instanceData,
            out IDictionary<XName, InstanceValue> instanceMetadata)
        {
            Boolean result = false;
            try
            {
                instanceData = new Dictionary<XName, InstanceValue>();
                instanceMetadata = new Dictionary<XName, InstanceValue>();

                String fileName = String.Format("{0}.xml", instanceId);
                String fullPath = Path.Combine(_dataDirectory, fileName);
                if (!File.Exists(fullPath))
                {
                    return result;
                }

                NetDataContractSerializer serializer =
                    new NetDataContractSerializer();

                //load instance data
                XElement xml = XElement.Load(fullPath);
                var entries =
                    (from e in xml.Element("InstanceData").Elements("Entry")
                     select e).ToList();
                foreach (XElement entry in entries)
                {
                    LoadSingleEntry(serializer, instanceData, entry);
                }

                //load instance metadata
                fileName = String.Format("{0}.meta.xml", instanceId);
                fullPath = Path.Combine(_dataDirectory, fileName);
                xml = XElement.Load(fullPath);
                entries =
                    (from e in xml.Element(
                         "InstanceMetadata").Elements("Entry")
                     select e).ToList();
                foreach (XElement entry in entries)
                {
                    LoadSingleEntry(serializer, instanceMetadata, entry);
                }

                result = true;
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                    "LoadInstance Exception: {0}", exception.Message);
                throw exception;
            }
            return result;
        }

        private void LoadSingleEntry(NetDataContractSerializer serializer,
            IDictionary<XName, InstanceValue> instanceData, XElement entry)
        {
            XName key =
                (XName)Deserialize(serializer, entry.Element("Key"));
            Object value =
                Deserialize(serializer, entry.Element("Value"));
            InstanceValue iv = new InstanceValue(value);
            InstanceValueOptions options =
                (InstanceValueOptions)Deserialize(
                    serializer, entry.Element("Options"));
            if (!options.HasFlag(InstanceValueOptions.WriteOnly))
            {
                instanceData.Add(key, iv);
            }
        }

        #endregion

        #region Delete Methods

        public void DeleteInstance(Guid instanceId)
        {
            String fileName = String.Format("{0}.xml", instanceId);
            String fullPath = Path.Combine(_dataDirectory, fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            fileName = String.Format("{0}.meta.xml", instanceId);
            fullPath = Path.Combine(_dataDirectory, fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        #endregion

        #region Association Methods

        public void SaveInstanceAssociation(Guid instanceId,
            Guid instanceKeyToAssociate, Boolean isDelete)
        {
            try
            {
                String fileName = String.Format("Key.{0}.{1}.xml",
                    instanceKeyToAssociate, instanceId);
                String fullPath = Path.Combine(_dataDirectory, fileName);
                if (!isDelete)
                {
                    if (!File.Exists(fullPath))
                    {
                        File.Create(fullPath);
                    }
                }
                else
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                    "PersistInstanceAssociation Exception: {0}", 
                    exception.Message);
                throw exception;
            }
        }

        public Guid GetInstanceAssociation(Guid instanceKey)
        {
            Guid instanceId = Guid.Empty;
            try
            {
                String[] files = Directory.GetFiles(_dataDirectory,
                    String.Format("Key.{0}.*.xml", instanceKey));
                if (files != null && files.Length > 0)
                {
                    String[] nodes = files[0].Split('.');
                    if (nodes.Length == 4)
                    {
                        instanceId = Guid.Parse(nodes[2]);
                    }
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                    "GetInstanceAssociation Exception: {0}", 
                    exception.Message);
                throw exception;
            }
            return instanceId;
        }

        public void DeleteInstanceAssociation(Guid instanceKey)
        {
            try
            {
                String[] files = Directory.GetFiles(_dataDirectory,
                    String.Format("Key.*.{0}.xml", instanceKey));
                if (files != null && files.Length > 0)
                {
                    foreach (String file in files)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                    "DeleteInstanceAssociation Exception: {0}", 
                    exception.Message);
                throw exception;
            }
        }

        #endregion

        #region Private methods

        private void CreateDataDirectory()
        {
            _dataDirectory = Path.Combine(
                Environment.CurrentDirectory, "InstanceStore");
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        private XElement Serialize(NetDataContractSerializer serializer,
            XElement parent, String name, Object value)
        {
            XElement element = new XElement(name);
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, value);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    element.Add(XElement.Load(stream));
                }
            }
            parent.Add(element);
            return element;
        }

        private Object Deserialize(NetDataContractSerializer serializer,
            XElement element)
        {
            Object result = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlDictionaryWriter writer =
                    XmlDictionaryWriter.CreateTextWriter(stream))
                {
                    foreach (XNode node in element.Nodes())
                    {
                        node.WriteTo(writer);
                    }

                    writer.Flush();
                    stream.Position = 0;
                    result = serializer.Deserialize(stream);
                }
            }
            return result;
        }

        #endregion
    }
}

