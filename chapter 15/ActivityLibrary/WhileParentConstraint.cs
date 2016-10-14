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
using System.Activities.Validation;
using System.Collections.Generic;

namespace ActivityLibrary
{
    public static class WhileParentConstraint
    {
        public static Constraint GetConstraint()
        {
            List<Type> prohibitedParentTypes = new List<Type>
            {
                typeof(While),
            };

            Variable<Boolean> result =
                new Variable<Boolean>("result", true);
            DelegateInArgument<Activity> element =
                new DelegateInArgument<Activity>();
            DelegateInArgument<ValidationContext> vc =
                new DelegateInArgument<ValidationContext>();
            DelegateInArgument<Activity> child =
                new DelegateInArgument<Activity>();

            return new Constraint<Activity>
            {
                Body = new ActivityAction
                    <Activity, ValidationContext>
                {
                    Argument1 = element,
                    Argument2 = vc,
                    Handler = new Sequence
                    {
                        Variables = { result },
                        Activities = 
                        {
                            new ForEach<Activity>
                            {
                                //Values = new GetWorkflowTree

                                Values = new GetParentChain
                                {
                                    ValidationContext = vc                                    
                                },
                                Body = new ActivityAction<Activity>
                                {                                    
                                    Argument = child, 
                                    Handler = new If()
                                    {                              
                                        Condition = new InArgument<Boolean>(ac =>
                                            prohibitedParentTypes.Contains(
                                                child.Get(ac).GetType())),
                                        Then = new Assign<Boolean>
                                        {
                                            To = result,
                                            Value = false
                                        }
                                    }
                                }                                
                            },
                            new AssertValidation
                            {
                                Assertion = new InArgument<Boolean>(result),
                                Message = new InArgument<String>(
                                 "Parent While activity not allowed"),
                                PropertyName = new InArgument<String>(
                                    (env) => element.Get(env).DisplayName)
                            }
                        }
                    }
                }
            };
        }
    }
}
