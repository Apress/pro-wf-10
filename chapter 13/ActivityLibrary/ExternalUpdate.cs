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
using AdventureWorksAccess;

namespace ActivityLibrary
{
    public sealed class ExternalUpdate : CodeActivity
    {
        public InArgument<Int32> SalesOrderId { get; set; }
        public InArgument<List<SalesOrderDetail>> OrderDetail { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            String operation = "record new sale";
            Console.WriteLine(
                "Order Id {0}: Notifying external system to {1}",
                SalesOrderId.Get(context), operation);
            foreach (SalesOrderDetail detail in OrderDetail.Get(context))
            {
                Console.WriteLine(
                    "Product {0}: {1}", detail.ProductID, operation);
            }
        }
    }
}
