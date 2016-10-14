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
using System.Runtime.Serialization;

namespace ServiceLibrary
{
    [DataContract]
    public class OrderProcessingRequestDc
    {
        public OrderProcessingRequestDc()
        {
            Items = new List<Item>();
        }

        [DataMember]
        public String CustomerName { get; set; }
        [DataMember]
        public String CustomerAddress { get; set; }
        [DataMember]
        public String CustomerEmail { get; set; }
        [DataMember]
        public Decimal TotalAmount { get; set; }
        [DataMember]
        public String CreditCard { get; set; }
        [DataMember]
        public String CreditCardExpiration { get; set; }
        [DataMember]
        public List<Item> Items { get; set; }
    }
}