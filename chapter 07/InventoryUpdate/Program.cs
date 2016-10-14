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
namespace InventoryUpdate
{
    using System;
    using System.Linq;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.Generic;
    using ActivityLibrary;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test InventoryUpdateFlowchart...");
            RunWorkflow(new InventoryUpdateFlowchart());
        }

        private static void RunWorkflow(Activity workflow)
        {
            List<SalesHistory> salesHist = new List<SalesHistory>
            {
                new SalesHistory{ItemId = 100, Quantity = 5},
                new SalesHistory{ItemId = 200, Quantity = 25},
                new SalesHistory{ItemId = 100, Quantity = 7},
                new SalesHistory{ItemId = 300, Quantity = 75},
                new SalesHistory{ItemId = 100, Quantity = 30},
                new SalesHistory{ItemId = 300, Quantity = 26},
            };

            List<ItemInventory> inventory = new List<ItemInventory>
            {
                new ItemInventory{ItemId = 100, QuantityOnHand = 100},
                new ItemInventory{ItemId = 200, QuantityOnHand = 200},
            };

            WorkflowInvoker.Invoke(workflow,
                new Dictionary<string, object>
                {
                    {"ArgSales", salesHist},
                    {"ArgInventory", inventory}
                });

            foreach (ItemInventory item in inventory)
            {
                Console.WriteLine("Item {0} ending inventory: {1}",
                    item.ItemId, item.QuantityOnHand);
            }
        }
    }
}
