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
    public sealed class CalcShipping : CodeActivity<Decimal>
    {
        public InArgument<Int32> Weight { get; set; }
        public InArgument<Decimal> OrderTotal { get; set; }
        public InArgument<String> ShipVia { get; set; }

        private Decimal _normalRate = 1.95M;
        private Decimal _expressRate = 3.50M;
        private Decimal _minimum = 12.95M;
        private Decimal _freeThreshold = 75.00M;
        private Boolean _isFreeShipping = false;

        protected override Decimal Execute(CodeActivityContext context)
        {
            Decimal result = 0;
            switch (ShipVia.Get(context))
            {
                case "normal":
                    if (OrderTotal.Get(context) >= _freeThreshold)
                    {
                        _isFreeShipping = true;
                    }
                    else
                    {
                        result = (Weight.Get(context) * _normalRate);
                    }
                    break;

                case "express":
                    result = (Weight.Get(context) * _expressRate);
                    break;
            }

            if ((result < _minimum) && (!_isFreeShipping))
            {
                result = _minimum;
            }

            return result;
        }
    }
}
