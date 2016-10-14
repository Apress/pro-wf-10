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
using System.Linq;
using AdventureWorksAccess;

namespace ActivityLibrary
{
    public sealed class DisplayProductInventory : AsyncCodeActivity
    {
        public InArgument<String> Description { get; set; }
        public InArgument<SalesOrderDetail> SalesDetail { get; set; }

        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context, AsyncCallback callback,
            object state)
        {
            Action<SalesOrderDetail, String> asyncWork =
                (sale, desc) => DisplayInventory(sale, desc);
            context.UserState = asyncWork;
            return asyncWork.BeginInvoke(
                SalesDetail.Get(context), Description.Get(context),
                callback, state);
        }

        protected override void EndExecute(
            AsyncCodeActivityContext context, IAsyncResult result)
        {
            ((Action<SalesOrderDetail, String>)
                context.UserState).EndInvoke(result);
        }

        private void DisplayInventory(SalesOrderDetail salesDetail, String desc)
        {
            using (AdventureWorksDataContext dc =
                new AdventureWorksDataContext())
            {
                var inventoryRow =
                    (from pi in dc.ProductInventories
                     where pi.ProductID == salesDetail.ProductID
                        && pi.LocationID == 7 //finished goods storage
                     select pi).SingleOrDefault();

                Boolean historyRowFound =
                    (from th in dc.TransactionHistories
                     where th.ProductID == salesDetail.ProductID
                      && (DateTime.Now - th.ModifiedDate < new TimeSpan(0, 0, 3))
                     select th).Any();

                if (inventoryRow != null)
                {
                    Console.WriteLine("Product {0}: {1} - {2} - {3}",
                        inventoryRow.ProductID, inventoryRow.Quantity, desc,
                        (historyRowFound ? "History Row Found" : "No History"));
                }
            }
        }
    }
}
