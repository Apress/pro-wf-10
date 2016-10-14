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

namespace ActivityLibraryTest
{
    public static class SharedShippingTest
    {
        #region shared test method

        public static void NormalTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ShipVia", "normal");
            parameters.Add("Weight", 20);
            parameters.Add("OrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Assert.AreEqual(39.00M, outputs["Result"], "Result is incorrect");
        }

        public static void NormalMinimumTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ShipVia", "normal");
            parameters.Add("Weight", 5);
            parameters.Add("OrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Assert.AreEqual(12.95M, outputs["Result"], "Result is incorrect");
        }

        public static void NormalFreeTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ShipVia", "normal");
            parameters.Add("Weight", 5);
            parameters.Add("OrderTotal", 75M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Assert.AreEqual(0.00M, outputs["Result"], "Result is incorrect");
        }

        public static void ExpressTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            parameters.Add("ShipVia", "express");
            parameters.Add("Weight", 5);
            parameters.Add("OrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Assert.AreEqual(17.50M, outputs["Result"], "Result is incorrect");
        }

        #endregion
    }
}
