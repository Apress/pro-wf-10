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
using System.Linq;
using AdventureWorksAccess;

using System.Activities.Tracking;

namespace ActivityLibrary
{
    public sealed class GetOrderDetail : AsyncCodeActivity
    {
        public InArgument<Int32> SalesOrderId { get; set; }
        public OutArgument<List<SalesOrderDetail>> OrderDetail { get; set; }

        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context, AsyncCallback callback,
            object state)
        {
            Func<Int32, List<SalesOrderDetail>> asyncWork =
                orderId => RetrieveOrderDetail(orderId);
            context.UserState = asyncWork;
            return asyncWork.BeginInvoke(
                SalesOrderId.Get(context), callback, state);
        }

        protected override void EndExecute(
            AsyncCodeActivityContext context, IAsyncResult result)
        {
            List<SalesOrderDetail> orderDetail =
                ((Func<Int32, List<SalesOrderDetail>>)
                    context.UserState).EndInvoke(result);
            if (orderDetail != null)
            {
                OrderDetail.Set(context, orderDetail);

                //add custom tracking
                CustomTrackingRecord trackRec =
                    new CustomTrackingRecord("QueryResults");
                trackRec.Data.Add("Count", orderDetail.Count);
                context.Track(trackRec);
            }
        }

        private List<SalesOrderDetail> RetrieveOrderDetail(Int32 salesOrderId)
        {
            List<SalesOrderDetail> result = new List<SalesOrderDetail>();
            using (AdventureWorksDataContext dc =
                new AdventureWorksDataContext())
            {
                var salesDetail =
                    (from sd in dc.SalesOrderDetails
                     where sd.SalesOrderID == salesOrderId
                     select sd).ToList();

                if (salesDetail != null && salesDetail.Count > 0)
                {
                    result = salesDetail;
                }
            }
            return result;
        }
    }
}
