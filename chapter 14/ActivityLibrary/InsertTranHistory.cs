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
using System.Transactions;

namespace ActivityLibrary
{
    public sealed class InsertTranHistory : CodeActivity
    {
        public InArgument<SalesOrderDetail> SalesDetail { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            SalesOrderDetail salesDetail = SalesDetail.Get(context);

            using (AdventureWorksDataContext dc =
                new AdventureWorksDataContext())
            {
                var historyRow = new TransactionHistory();
                historyRow.ProductID = salesDetail.ProductID;
                historyRow.ModifiedDate = DateTime.Now;
                historyRow.Quantity = salesDetail.OrderQty;
                historyRow.TransactionDate = salesDetail.ModifiedDate;
                historyRow.TransactionType = 'S';
                historyRow.ReferenceOrderID = salesDetail.SalesOrderID;
                historyRow.ReferenceOrderLineID
                    = salesDetail.SalesOrderDetailID;

                dc.TransactionHistories.InsertOnSubmit(historyRow);
                dc.SubmitChanges();
                Console.WriteLine(
                    "Product {0}: Added history for Qty of {1} ",
                    salesDetail.ProductID, salesDetail.OrderQty);
            }
        }
    }
}
