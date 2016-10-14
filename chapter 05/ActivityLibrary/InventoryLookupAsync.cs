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
    internal class InventoryLookupAsyncArgs
    {
        public Int32 ItemId { get; set; }
        public Int32 WarehouseId { get; set; }
        public Int32 RequestedQty { get; set; }
    }

    public class InventoryLookupAsync : AsyncCodeActivity<Int32>
    {
        public InArgument<Int32> ItemId { get; set; }
        public InArgument<Int32> WarehouseId { get; set; }
        public InArgument<Int32> RequestedQty { get; set; }

        private Dictionary<Int32, Dictionary<Int32, Int32>> _warehouses
            = new Dictionary<Int32, Dictionary<Int32, Int32>>();

        public InventoryLookupAsync()
        {
            //load some test data
            _warehouses.Add(1, new Dictionary<int, int>
            {
                {100, 5},
                {200, 0},
                {300, 0},
            });

            _warehouses.Add(2, new Dictionary<int, int>
            {
                {100, 10},
                {200, 0},
                {300, 0},
            });

            _warehouses.Add(3, new Dictionary<int, int>
            {
                {100, 50},
                {200, 75},
                {300, 0},
            });
        }

        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context,
            AsyncCallback callback, object state)
        {
            InventoryLookupAsyncArgs parameters = new InventoryLookupAsyncArgs
            {
                ItemId = ItemId.Get(context),
                WarehouseId = WarehouseId.Get(context),
                RequestedQty = RequestedQty.Get(context),
            };

            Func<InventoryLookupAsyncArgs, Int32> asyncWork =
                args => Lookup(args);
            context.UserState = asyncWork;
            return asyncWork.BeginInvoke(parameters, callback, state);
        }

        private Int32 Lookup(InventoryLookupAsyncArgs args)
        {
            Int32 availableInventory = 0;
            Dictionary<Int32, Int32> warehouse = null;
            if (_warehouses.TryGetValue(args.WarehouseId, out warehouse))
            {
                if (warehouse.TryGetValue(args.ItemId, out availableInventory))
                {
                    if (availableInventory > args.RequestedQty)
                    {
                        availableInventory = args.RequestedQty;
                    }
                }
            }
            return availableInventory;
        }

        protected override int EndExecute(
            AsyncCodeActivityContext context, IAsyncResult result)
        {
            return ((Func<InventoryLookupAsyncArgs, Int32>)
                context.UserState).EndInvoke(result);
        }
    }
}
