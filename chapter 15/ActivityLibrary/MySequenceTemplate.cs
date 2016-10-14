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
using System.Activities.Presentation;
using System.Activities.Statements;

namespace ActivityLibrary
{
    public class MySequenceTemplate : IActivityTemplateFactory
    {
        public Activity Create(System.Windows.DependencyObject target)
        {
            return new MySequenceWithConstraint
            {
                DisplayName = "MySequenceTemplate",
                Activities =
                {
                    new Sequence
                    {
                        Activities = 
                        {
                            new WriteLine
                            {
                                Text = "Template generated text"
                            }
                        }
                    }
                }
            };
        }
    }
}
