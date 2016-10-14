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

namespace ActivityLibrary
{
    public class ItemInventory : IEquatable<ItemInventory>
    {
        public Int32 ItemId { get; set; }
        public Int32 QuantityOnHand { get; set; }

        public bool Equals(ItemInventory other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return (this.ItemId == other.ItemId);
            }
        }

        public void ReduceInventory(Int32 adjustment)
        {
            QuantityOnHand -= adjustment;
        }

        public static ItemInventory Create(Int32 itemId, Int32 quantity)
        {
            return new ItemInventory
            {
                ItemId = itemId,
                QuantityOnHand = quantity
            };
        }
    }
}
