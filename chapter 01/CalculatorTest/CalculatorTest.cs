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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorTest
{
    [TestClass]
    public class CalculatorTest
    {
        [TestMethod]
        public void AddTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgExpression", "111 + 222");

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new Calculator.Calculator(), parameters);

            Assert.IsNotNull(outputs, "outputs should not be null");
            Assert.AreEqual(1, outputs.Count, "outputs count is incorrect");
            Assert.AreEqual((Double)333, outputs["Result"],
                "Result is incorrect");
        }

        [TestMethod]
        public void SubtractTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgExpression", "333 - 222");

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new Calculator.Calculator(), parameters);

            Assert.IsNotNull(outputs, "outputs should not be null");
            Assert.AreEqual(1, outputs.Count, "outputs count is incorrect");
            Assert.AreEqual((Double)111, outputs["Result"],
                "Result is incorrect");
        }

        [TestMethod]
        public void MultiplyTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgExpression", "111 * 5");

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new Calculator.Calculator(), parameters);

            Assert.IsNotNull(outputs, "outputs should not be null");
            Assert.AreEqual(1, outputs.Count, "outputs count is incorrect");
            Assert.AreEqual((Double)555, outputs["Result"],
                "Result is incorrect");
        }

        [TestMethod]
        public void DivideTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgExpression", "555 / 5");

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new Calculator.Calculator(), parameters);

            Assert.IsNotNull(outputs, "outputs should not be null");
            Assert.AreEqual(1, outputs.Count, "outputs count is incorrect");
            Assert.AreEqual((Double)111, outputs["Result"],
                "Result is incorrect");
        }
    }
}
