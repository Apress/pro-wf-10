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
using System.Activities.Expressions;
using System.Activities.Statements;

namespace ExpressionActivities
{

    public sealed class AddNumbers : Activity<Int32>
    {
        public InArgument<Int32> NumberOne { get; set; }
        public InArgument<Int32> NumberTwo { get; set; }

        protected override Func<Activity> Implementation
        {
            get { return () => BuildImplementation(); }
        }

        private Activity BuildImplementation()
        {
            return new Sequence
            {
                Activities =
                {
                    new Assign
                    {
                        To = new OutArgument<Int32>(ac => 
                            this.Result.Get(ac)),
                        Value = new InArgument<Int32>
                        {
                            Expression = new Add<Int32,Int32,Int32>
                            {
                                Left = new InArgument<Int32>(ac =>
                                    NumberOne.Get(ac)),
                                Right = new InArgument<Int32>(ac =>
                                    NumberTwo.Get(ac)),
                            }
                        }
                    }
                }
            };
        }
    }
}
