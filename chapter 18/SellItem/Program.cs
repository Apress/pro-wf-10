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

namespace SellItem
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWorkflow(new SellItem35Interop());
            TestWorkflow(new SellItemApplyRules());
        }

        private static void TestWorkflow(Activity activity)
        {
            ActivityLibrary35.SalesItem item = new ActivityLibrary35.SalesItem();

            //execute the workflow with parameters that will 
            //result in a normal priced item and shipping
            Console.WriteLine("Executing SellItemWorkflow");
            item = new ActivityLibrary35.SalesItem();
            item.ItemPrice = 10.00;
            item.Quantity = 4;
            item.IsNewCustomer = false;
            ExecuteInteropWorkflow(activity, item);
            Console.WriteLine("Completed SellItemWorkflow\n\r");

            //execute the workflow again with parameters that
            //will cause a discounted price and shipping
            Console.WriteLine("Executing SellItemWorkflow (Discounts)");
            item = new ActivityLibrary35.SalesItem();
            item.ItemPrice = 10.00;
            item.Quantity = 11;
            item.IsNewCustomer = false;
            ExecuteInteropWorkflow(activity, item);
            Console.WriteLine("Completed SellItemWorkflow (Discounts)\n\r");

            //execute the workflow once more, this time with the 
            //IsNewCustomer property set to true
            Console.WriteLine("Executing SellItemWorkflow (New Customer)");
            item = new ActivityLibrary35.SalesItem();
            item.ItemPrice = 10.00;
            item.Quantity = 11;
            item.IsNewCustomer = true;
            ExecuteInteropWorkflow(activity, item);
            Console.WriteLine("Completed SellItemWorkflow (New Customer)\n\r");
        }

        private static void ExecuteInteropWorkflow(Activity activity,
            ActivityLibrary35.SalesItem item)
        {
            DisplaySalesItem(item, "Before");
            WorkflowInvoker.Invoke(activity,
                new Dictionary<String, Object>
                {
                    {"ArgItem", item}
                });
            DisplaySalesItem(item, "After");
        }

        private static void DisplaySalesItem(
            ActivityLibrary35.SalesItem item, String message)
        {
            Console.WriteLine("{0}:", message);
            Console.WriteLine("  ItemPrice     = {0:C}", item.ItemPrice);
            Console.WriteLine("  Quantity      = {0}", item.Quantity);
            Console.WriteLine("  OrderTotal    = {0:C}", item.OrderTotal);
            Console.WriteLine("  Shipping      = {0:C}", item.Shipping);
            Console.WriteLine("  IsNewCustomer = {0}", item.IsNewCustomer);
        }
    }
}
