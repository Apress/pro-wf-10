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
    internal class CalcShippingAsyncArgs
    {
        public Int32 Weight { get; set; }
        public Decimal OrderTotal { get; set; }
        public String ShipVia { get; set; }
    }

    public sealed class CalcShippingAsync : AsyncCodeActivity<Decimal>
    {
        public InArgument<Int32> Weight { get; set; }
        public InArgument<Decimal> OrderTotal { get; set; }
        public InArgument<String> ShipVia { get; set; }

        private Decimal _normalRate = 1.95M;
        private Decimal _expressRate = 3.50M;
        private Decimal _minimum = 12.95M;
        private Decimal _freeThreshold = 75.00M;

        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context,
            AsyncCallback callback, object state)
        {
            CalcShippingAsyncArgs parameters = new CalcShippingAsyncArgs
            {
                Weight = Weight.Get(context),
                OrderTotal = OrderTotal.Get(context),
                ShipVia = ShipVia.Get(context),
            };

            Func<CalcShippingAsyncArgs, Decimal> asyncWork = a => Calculate(a);
            context.UserState = asyncWork;
            return asyncWork.BeginInvoke(parameters, callback, state);
        }

        private Decimal Calculate(CalcShippingAsyncArgs parameters)
        {
            Decimal result = 0;
            Boolean isFreeShipping = false;

            System.Threading.Thread.Sleep(500);  //simulate a short delay

            switch (parameters.ShipVia)
            {
                case "normal":
                    if (parameters.OrderTotal >= _freeThreshold)
                    {
                        isFreeShipping = true;
                    }
                    else
                    {
                        result = parameters.Weight * _normalRate;
                    }
                    break;

                case "express":
                    result = parameters.Weight * _expressRate;
                    break;
            }

            if ((result < _minimum) && (!isFreeShipping))
            {
                result = _minimum;
            }

            return result;
        }

        protected override Decimal EndExecute(
            AsyncCodeActivityContext context, IAsyncResult result)
        {
            return ((Func<CalcShippingAsyncArgs, Decimal>)
                context.UserState).EndInvoke(result);
        }
    }
}
