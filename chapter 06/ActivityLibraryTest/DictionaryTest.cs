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
using System.Activities;
using System.Collections.Generic;
using ActivityLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActivityLibraryTest
{
    [TestClass]
    public class DictionaryTest
    {
        [TestMethod]
        public void AddToDictionaryTest()
        {
            Dictionary<int, string> dictionary = GetTestDictionary();
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("Dictionary", dictionary);
            parameters.Add("Key", 400);
            parameters.Add("Item", "four");

            WorkflowInvoker.Invoke(
                new AddToDictionary<int, string>(), parameters);
            Assert.AreEqual(4, dictionary.Count,
                "Count is incorrect");
            Assert.IsTrue(dictionary.ContainsKey(400),
                "Test entry is missing");
        }

        [TestMethod]
        public void ExistsInDictionaryTest()
        {
            Dictionary<int, string> dictionary = GetTestDictionary();
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("Dictionary", dictionary);
            parameters.Add("Key", 300);

            IDictionary<string, object> outputs = WorkflowInvoker.Invoke(
                (Activity)new ExistsInDictionary<int, string>(), parameters);
            Assert.IsTrue((Boolean)outputs["Result"],
                "Result is incorrect");

            parameters.Clear();
            parameters.Add("Dictionary", dictionary);
            parameters.Add("Key", 99999);
            outputs = WorkflowInvoker.Invoke(
                (Activity)new ExistsInDictionary<int, string>(), parameters);
            Assert.IsFalse((Boolean)outputs["Result"],
                "Result is incorrect for bad key");
        }

        [TestMethod]
        public void RemoveFromDictionaryTest()
        {
            Dictionary<int, string> dictionary = GetTestDictionary();
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("Dictionary", dictionary);
            parameters.Add("Key", 300);

            IDictionary<string, object> outputs = WorkflowInvoker.Invoke(
                (Activity)new RemoveFromDictionary<int, string>(), parameters);
            Assert.IsTrue((Boolean)outputs["Result"],
                "Result is incorrect");
            Assert.AreEqual(2, dictionary.Count,
                "Count is incorrect");
            Assert.IsFalse(dictionary.ContainsKey(300),
                "Test entry should be missing");
        }

        [TestMethod]
        public void ClearDictionaryTest()
        {
            Dictionary<int, string> dictionary = GetTestDictionary();
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("Dictionary", dictionary);

            WorkflowInvoker.Invoke(
                new ClearDictionary<int, string>(), parameters);
            Assert.AreEqual(0, dictionary.Count,
                "Count is incorrect");
        }

        [TestMethod]
        public void FindInDictionaryTest()
        {
            Dictionary<int, string> dictionary = GetTestDictionary();
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("Dictionary", dictionary);
            parameters.Add("Key", 300);

            IDictionary<string, object> outputs = WorkflowInvoker.Invoke(
                (Activity)new FindInDictionary<int, string>(), parameters);
            Assert.IsTrue((Boolean)outputs["Result"],
                "Result is incorrect");
            Assert.AreEqual("three", (string)outputs["FoundItem"],
                "FoundItem is incorrect");

            parameters.Clear();
            parameters.Add("Dictionary", dictionary);
            parameters.Add("Key", 9999);
            outputs = WorkflowInvoker.Invoke(
                (Activity)new FindInDictionary<int, string>(), parameters);
            Assert.IsFalse((Boolean)outputs["Result"],
                "Result is incorrect for bad Key");
        }

        private static Dictionary<int, string> GetTestDictionary()
        {
            return new Dictionary<int, string>
            {
                {100, "one"},
                {200, "two"},
                {300, "three"},
            };
        }
    }
}
