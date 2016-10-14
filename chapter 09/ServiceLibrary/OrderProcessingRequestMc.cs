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
using System.ServiceModel;

namespace ServiceLibrary
{
    [MessageContract]
    public class OrderProcessingRequestMc
    {
        public OrderProcessingRequestMc()
        {
            Items = new List<Item>();
        }

        [MessageBodyMember]
        public String CustomerName { get; set; }
        [MessageBodyMember]
        public String CustomerAddress { get; set; }
        [MessageBodyMember]
        public String CustomerEmail { get; set; }
        [MessageBodyMember]
        public Decimal TotalAmount { get; set; }
        [MessageHeader]
        public String CreditCard { get; set; }
        [MessageHeader]
        public String CreditCardExpiration { get; set; }
        [MessageBodyMember]
        public List<Item> Items { get; set; }
    }
}
