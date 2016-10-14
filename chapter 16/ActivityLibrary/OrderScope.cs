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
using System.ComponentModel;

namespace ActivityLibrary
{
    [Designer(typeof(ActivityLibrary.Design.MySimpleParentDesigner))]
    public class OrderScope : NativeActivity
    {
        [Browsable(false)]
        public Activity Body { get; set; }
        [RequiredArgument]
        public InArgument<Int32> OrderId { get; set; }
        public OutArgument<Decimal> OrderTotal { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Int32 orderId = OrderId.Get(context);
            context.Properties.Add("OrderId", orderId);

            Bookmark bm = context.CreateBookmark(
                "UpdateOrderTotalBookmark", OnUpdateOrderTotal,
                BookmarkOptions.NonBlocking | BookmarkOptions.MultipleResume);
            context.Properties.Add(bm.Name, bm);

            context.ScheduleActivity(Body);
        }

        private void OnUpdateOrderTotal(NativeActivityContext context,
            Bookmark bookmark, object value)
        {
            if (value is Decimal)
            {
                OrderTotal.Set(context,
                    OrderTotal.Get(context) + (Decimal)value);
                Console.WriteLine(
                    "OrderScope.OnUpdateOrderTotal Value: {0}, Total: {1}",
                    (Decimal)value, OrderTotal.Get(context));
            }
        }

        protected override bool CanInduceIdle
        {
            get { return true; }
        }
    }
}
