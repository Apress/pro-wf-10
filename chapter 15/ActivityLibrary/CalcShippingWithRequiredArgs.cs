﻿//--------------------------------------------------------------------------------
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
    [Designer(typeof(ActivityDesignerLibrary.CalcShippingCollapsibleDesigner))]
    public sealed class CalcShippingWithRequiredArgs : CodeActivity<Decimal>
    {
        [RequiredArgument]
        public InArgument<Int32> Weight { get; set; }

        [RequiredArgument]
        public InArgument<Decimal> OrderTotal { get; set; }

        [RequiredArgument]
        public InArgument<String> ShipVia { get; set; }

        protected override Decimal Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
