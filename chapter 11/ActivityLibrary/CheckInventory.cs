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

namespace ActivityLibrary
{
    public sealed class CheckInventory : CodeActivity<Boolean>
    {
        public InArgument<Int32> OrderId { get; set; }
        [RequiredArgument]
        public InArgument<Int32> ItemId { get; set; }
        [RequiredArgument]
        public InArgument<Int32> Quantity { get; set; }

        protected override Boolean Execute(CodeActivityContext context)
        {
            Boolean result = false;
            IItemSupport extension = context.GetExtension<IItemSupport>();
            if (extension != null)
            {
                result = extension.UpdatePendingInventory(OrderId.Get(context),
                    ItemId.Get(context), Quantity.Get(context));
            }
            return result;
        }
    }
}
