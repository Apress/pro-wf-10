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
using System.Collections.Generic;
using System.Linq;

namespace ActivityLibrary
{
    public class ItemSupportExtension : IItemSupport
    {
        private Dictionary<Int32, ItemDefinition> _items =
            new Dictionary<Int32, ItemDefinition>();
        private Int32 _orderId;

        #region IItemSupport Members

        public Decimal GetItemPrice(int itemId)
        {
            Decimal price = 0;
            ItemDefinition def = null;
            if (_items.TryGetValue(itemId, out def))
            {
                price = def.Price;
            }
            return price;
        }

        public bool UpdatePendingInventory(
            Int32 orderId, int itemId, int quantity)
        {
            Boolean result = false;
            ItemDefinition def = null;
            lock (_items)
            {
                if (_items.TryGetValue(itemId, out def))
                {
                    if (quantity <= def.QtyAvailable)
                    {
                        Int32 origQuantity = def.QtyAvailable;
                        def.QtyAvailable -= quantity;
                        Console.WriteLine(
                            "Update: ItemId={0}, QtyBefore={1}, QtyAfter={2}",
                            def.ItemId, origQuantity, def.QtyAvailable);
                        _orderId = orderId;
                        result = true;
                    }
                }
            }
            return result;
        }

        public List<ItemDefinition> GetItemDefinitions()
        {
            return _items.Values.ToList();
        }

        #endregion

        #region Host Members

        public void AddItemDefinition(Int32 itemId, Decimal price,
            Int32 qtyAvailable)
        {
            if (!_items.ContainsKey(itemId))
            {
                ItemDefinition def = new ItemDefinition
                {
                    ItemId = itemId,
                    Price = price,
                    QtyAvailable = qtyAvailable
                };
                _items.Add(def.ItemId, def);
            }
        }

        #endregion
    }
}
