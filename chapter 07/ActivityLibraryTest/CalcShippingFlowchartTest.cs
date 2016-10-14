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
    public class CalcShippingFlowchartTest
    {
        [TestMethod]
        public void NormalTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgShipVia", "normal");
            parameters.Add("ArgWeight", 20);
            parameters.Add("ArgOrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new CalcShippingFlowchart(), parameters);
            Assert.AreEqual(39.00M, outputs["Result"], "Result is incorrect");
        }

        [TestMethod]
        public void NormalMinimumTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgShipVia", "normal");
            parameters.Add("ArgWeight", 5);
            parameters.Add("ArgOrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new CalcShippingFlowchart(), parameters);
            Assert.AreEqual(12.95M, outputs["Result"], "Result is incorrect");
        }

        [TestMethod]
        public void NormalFreeTest()
        {
            Activity activity = new CalcShippingFlowchart();
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgShipVia", "normal");
            parameters.Add("ArgWeight", 5);
            parameters.Add("ArgOrderTotal", 75M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new CalcShippingFlowchart(), parameters);
            Assert.AreEqual(0.00M, outputs["Result"], "Result is incorrect");
        }

        [TestMethod]
        public void ExpressTest()
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ArgShipVia", "express");
            parameters.Add("ArgWeight", 5);
            parameters.Add("ArgOrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                new CalcShippingFlowchart(), parameters);
            Assert.AreEqual(17.50M, outputs["Result"], "Result is incorrect");
        }
    }
}
