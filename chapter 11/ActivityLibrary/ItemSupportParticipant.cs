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
using System.Activities.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ActivityLibrary
{
    public class ItemSupportParticipant : PersistenceParticipant, IItemSupport
    {
        private Dictionary<Int32, ItemDefinition> _items =
            new Dictionary<Int32, ItemDefinition>();
        private Int32 _orderId;

        private XName _itemsName = XName.Get(
            "ItemDefinitions", "ActivityLibrary.ItemSupportExtension");
        private XName _orderIdName = XName.Get(
            "OrderId", "ActivityLibrary.ItemSupportExtension");

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

        #region PersistenceParticipant members

        /// <summary>
        /// Executed when an instance is persisted
        /// </summary>
        /// <param name="readWriteValues"></param>
        /// <param name="writeOnlyValues"></param>
        protected override void CollectValues(
            out IDictionary<System.Xml.Linq.XName, object> readWriteValues,
            out IDictionary<System.Xml.Linq.XName, object> writeOnlyValues)
        {
            //Console.WriteLine("CollectValues");
            readWriteValues = new Dictionary<System.Xml.Linq.XName, object>();
            lock (_items)
            {
                readWriteValues.Add(_itemsName, _items);
            }

            if (_orderId > 0)
            {
                writeOnlyValues = new Dictionary<System.Xml.Linq.XName, object>();
                writeOnlyValues.Add(_orderIdName, _orderId);
                _orderId = 0;
            }
            else
            {
                writeOnlyValues = null;
            }
        }
        /// <summary>
        /// Executed when an instance is loaded
        /// </summary>
        /// <param name="readWriteValues"></param>
        protected override void PublishValues(
            IDictionary<System.Xml.Linq.XName, object> readWriteValues)
        {
            //Console.WriteLine("PublishValues");
            object value = null;
            if (readWriteValues.TryGetValue(_itemsName, out value))
            {
                if (value is Dictionary<Int32, ItemDefinition>)
                {
                    lock (_items)
                    {
                        _items = value as Dictionary<Int32, ItemDefinition>;
                    }
                    //Console.WriteLine("PublishValues restored value");
                }
            }
        }

        #endregion
    }
}
