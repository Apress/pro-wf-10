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
using Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorTest
{
    [TestClass]
    public class ParseCalculatorArgsTest
    {
        [TestMethod]
        public void ValidExpressionTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("Expression", "1 + 2");

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new ParseCalculatorArgs(), parameters);

            Assert.IsNotNull(outputs, "outputs should not be null");
            Assert.AreEqual(3, outputs.Count, "outputs count is incorrect");
            Assert.AreEqual((Double)1, outputs["FirstNumber"],
                "FirstNumber is incorrect");
            Assert.AreEqual((Double)2, outputs["SecondNumber"],
                "SecondNumber is incorrect");
            Assert.AreEqual("+", outputs["Operation"],
                "Operation is incorrect");
        }

        [TestMethod]
        public void InvalidExpressionTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("Expression", "badexpression");

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new ParseCalculatorArgs(), parameters);

            Assert.IsNotNull(outputs, "outputs should not be null");
            Assert.AreEqual(3, outputs.Count, "outputs count is incorrect");
            Assert.AreEqual("error", outputs["Operation"],
                "Operation is incorrect");
        }
    }
}
