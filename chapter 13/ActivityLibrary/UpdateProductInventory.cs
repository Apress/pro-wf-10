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
using System.Transactions;
using AdventureWorksAccess;

namespace ActivityLibrary
{
    public sealed class UpdateProductInventory : AsyncCodeActivity
    {
        public InArgument<SalesOrderDetail> SalesDetail { get; set; }

        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context,
            AsyncCallback callback, object state)
        {
            DependentTransaction dependentTran = null;
            if (Transaction.Current != null)
            {
                dependentTran = Transaction.Current.DependentClone(
                    DependentCloneOption.BlockCommitUntilComplete);
            }

            Action<DependentTransaction, SalesOrderDetail> asyncWork =
                (dt, sale) => UpdateInventory(dt, sale);
            context.UserState = asyncWork;
            return asyncWork.BeginInvoke(
                 dependentTran, SalesDetail.Get(context), callback, state);
        }

        protected override void EndExecute(
            AsyncCodeActivityContext context, IAsyncResult result)
        {
            ((Action<DependentTransaction, SalesOrderDetail>)
                context.UserState).EndInvoke(result);
        }

        private void UpdateInventory(DependentTransaction dt,
            SalesOrderDetail salesDetail)
        {
            try
            {
                using (AdventureWorksDataContext dc =
                    new AdventureWorksDataContext())
                {
                    //use the dependent transaction if there is one, 
                    //or suppress the creation of a new transaction
                    using (TransactionScope scope = (dt != null ?
                        new TransactionScope(dt) :
                        new TransactionScope(TransactionScopeOption.Suppress)))
                    {
                        var inventoryRow =
                            (from pi in dc.ProductInventories
                             where pi.ProductID == salesDetail.ProductID
                                && pi.LocationID == 7 //finished goods storage
                             select pi).SingleOrDefault();
                        if (inventoryRow != null)
                        {
                            inventoryRow.Quantity -= salesDetail.OrderQty;
                            inventoryRow.ModifiedDate = DateTime.Now;
                            Console.WriteLine(
                                "Product {0}: Reduced by {1}",
                                inventoryRow.ProductID, salesDetail.OrderQty);
                            dc.SubmitChanges();
                        }

                        scope.Complete();
                    }
                }
            }
            finally
            {
                //the DependentTransaction must be completed otherwise
                //the ambient transaction will block on complete
                if (dt != null)
                {
                    dt.Complete();
                    dt.Dispose();
                }
            }
        }
    }
}
