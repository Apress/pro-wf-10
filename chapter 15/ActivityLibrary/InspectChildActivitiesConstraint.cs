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
    public static class InspectChildActivitiesConstraint
    {
        public static Constraint GetConstraint()
        {
            List<Type> allowedTypes = new List<Type>
            {
                typeof(Sequence),
                typeof(WriteLine),
                typeof(Assign)
            };

            Variable<String> buffer =
                new Variable<String>("buffer", String.Empty);

            Variable<Boolean> result =
                new Variable<Boolean>("result", false);
            DelegateInArgument<MySequenceWithConstraint> element =
                new DelegateInArgument<MySequenceWithConstraint>();
            DelegateInArgument<ValidationContext> vc = 
                new DelegateInArgument<ValidationContext>();
            DelegateInArgument<Activity> child = 
                new DelegateInArgument<Activity>();

            return new Constraint<MySequenceWithConstraint>
            {
                Body = new ActivityAction
                    <MySequenceWithConstraint, ValidationContext>
                {
                    Argument1 = element,
                    Argument2 = vc,
                    Handler = new Sequence
                    {
                        Variables = { result, buffer },
                        Activities = 
                        {
                            new ForEach<Activity>
                            {
                                Values = new GetChildSubtree
                                {
                                    ValidationContext = vc                                    
                                },
                                Body = new ActivityAction<Activity>
                                {                                    
                                    Argument = child, 
                                    Handler = new If()
                                    {                              
                                        Condition = new InArgument<Boolean>(ac =>
                                            allowedTypes.Contains(
                                                child.Get(ac).GetType())),
                                        Else = new Assign<String>
                                        {
                                            To = buffer,
                                            Value = new InArgument<String>(ac =>
                                                child.Get(ac).GetType().ToString())
                                        }
                                    }
                                }                                
                            },
                            new AssertValidation
                            {
                                Assertion = new InArgument<Boolean>(result),
                                Message = new InArgument<String>(buffer),
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
