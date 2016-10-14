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
using System.Activities.Statements;

namespace ActivityLibrary
{
    public sealed class CalcShippingInCode : Activity<Decimal>
    {
        public InArgument<Int32> Weight { get; set; }
        public InArgument<Decimal> OrderTotal { get; set; }
        public InArgument<String> ShipVia { get; set; }

        public CalcShippingInCode()
        {
            //public delegate TResult Func<out TResult>();
            this.Implementation = BuildImplementation;
        }

        private Activity BuildImplementation()
        {
            Variable<Boolean> isFreeShipping =
                new Variable<Boolean> { Name = "IsFreeShipping" };
            Variable<Decimal> normalRate =
                new Variable<Decimal> { Name = "NormalRate", Default = 1.95M };
            Variable<Decimal> expressRate =
                new Variable<Decimal> { Name = "ExpressRate", Default = 3.50M };
            Variable<Decimal> minimum =
                new Variable<Decimal> { Name = "Minimum", Default = 12.95M };
            Variable<Decimal> freeThreshold =
                new Variable<Decimal> { Name = "FreeThreshold", Default = 75.00M };

            return new Sequence
            {
                Variables = 
                {
                    isFreeShipping, normalRate, expressRate, minimum, freeThreshold
                },

                Activities =
                {
                    new Switch<String>
                    {
                        //public InArgument(Expression<Func<ActivityContext, T>> expression);
                        Expression = new InArgument<String>(
                            ac => ShipVia.Get(ac)),
                        Cases = 
                        {
                            {"normal", new If
                                {
                                    Condition = new InArgument<Boolean>(ac => 
                                      OrderTotal.Get(ac) >= freeThreshold.Get(ac)),
                                    Then = new Assign<Boolean>
                                    {
                                        //public OutArgument(Expression<Func<ActivityContext, T>> expression);
                                        To = new OutArgument<Boolean>(ac =>
                                          isFreeShipping.Get(ac)),
                                        Value = true
                                    },
                                    Else = new Assign<Decimal>
                                    {
                                        To = new OutArgument<Decimal>(ac => 
                                          this.Result.Get(ac)),
                                        Value = new InArgument<Decimal>(ac => 
                                          Weight.Get(ac) * normalRate.Get(ac))
                                    }
                                }
                            },
                            {"express", new Assign<Decimal>
                                {
                                    To = new OutArgument<Decimal>(ac => 
                                      this.Result.Get(ac)),
                                    Value = new InArgument<Decimal>(ac => 
                                      Weight.Get(ac) * expressRate.Get(ac))
                                }
                            }
                        }
                    },
                    new If
                    {
                        Condition = new InArgument<bool>(ac => 
                          Result.Get(ac) < minimum.Get(ac) && 
                            (!isFreeShipping.Get(ac))),
                        Then = new Assign
                        {
                            To = new OutArgument<Decimal>(ac => Result.Get(ac)),
                            Value = new InArgument<Decimal>(ac => minimum.Get(ac))
                        }
                    }
                }
            }; //new Sequence
        }
    }
}
