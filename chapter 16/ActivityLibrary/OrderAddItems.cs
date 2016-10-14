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

namespace ActivityLibrary
{
    public class OrderAddItems : NativeActivity
    {
        [RequiredArgument]
        public InArgument<List<Int32>> Items { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Int32 orderId = (Int32)context.Properties.Find("OrderId");
            Console.WriteLine("OrderAddItems process OrderId: {0}", orderId);
            Bookmark bm = (Bookmark)context.Properties.Find(
                "UpdateOrderTotalBookmark");

            if (bm == null)
            {
                throw new NullReferenceException(
                    "UpdateOrderTotalBookmark was not provided by parent scope");
            }

            List<Int32> items = Items.Get(context);
            if (items != null && items.Count > 0)
            {
                foreach (Int32 itemId in items)
                {
                    Decimal price = ((Decimal)itemId / 100);
                    context.ResumeBookmark(bm, price);
                }
            }
        }
    }
}
