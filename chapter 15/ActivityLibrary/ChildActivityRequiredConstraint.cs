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
using System.Activities.Validation;

namespace ActivityLibrary
{
    public static class ChildActivityRequiredConstraint
    {
        public static Constraint GetConstraint()
        {
            DelegateInArgument<MySequenceWithConstraint> element =
                new DelegateInArgument<MySequenceWithConstraint>();
            return new Constraint<MySequenceWithConstraint>
            {
                Body = new ActivityAction<MySequenceWithConstraint, ValidationContext>
                {
                    Argument1 = element,
                    Handler = new AssertValidation
                    {
                        IsWarning = false,
                        Assertion = new InArgument<bool>(
                            env => (element.Get(env).Activities.Count > 0)),
                        Message = new InArgument<string>(
                            "At least one child activity must be added"),
                    }
                }
            };
        }
    }
}