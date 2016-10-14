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

namespace GetItemLocation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GetItemLocation tests...");
            RunWorkflow(new GetItemLocation(), 100, 2);
            RunWorkflow(new GetItemLocation(), 100, 8);
            RunWorkflow(new GetItemLocation(), 100, 20);
            RunWorkflow(new GetItemLocation(), 100, 100);
            RunWorkflow(new GetItemLocation(), 200, 10);
            RunWorkflow(new GetItemLocation(), 300, 15);

            Console.WriteLine("\nGetItemLocationAsync tests...");
            RunWorkflow(new GetItemLocationAsync(), 100, 2);
            RunWorkflow(new GetItemLocationAsync(), 100, 8);
            RunWorkflow(new GetItemLocationAsync(), 100, 20);
            RunWorkflow(new GetItemLocationAsync(), 100, 100);
            RunWorkflow(new GetItemLocationAsync(), 200, 10);
            RunWorkflow(new GetItemLocationAsync(), 300, 15);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void RunWorkflow(Activity workflow,
            Int32 itemId, Int32 quantity)
        {
            IDictionary<String, Object> output = WorkflowInvoker.Invoke(
                workflow, new Dictionary<string, object>
                {
                    {"ArgItemId", itemId},
                    {"ArgQuantity", quantity}
                });

            Console.WriteLine(
                "Item: {0} Requested: {1} Found: {2} Warehouse: {3}",
                    itemId, quantity, output["ArgQuantityFound"],
                    output["ArgWarehouseIdFound"]);
        }
    }
}
