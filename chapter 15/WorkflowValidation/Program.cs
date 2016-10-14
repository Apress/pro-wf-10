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
using ActivityLibrary;

namespace WorkflowValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nMySequenceWithError");
            ShowValidationResults(ActivityValidationServices.Validate(
                MySequenceWithError()));

            Console.WriteLine("\nMySequenceNoError");
            ShowValidationResults(ActivityValidationServices.Validate(
                MySequenceNoError()));

            Console.WriteLine("\nWhileAndWriteLineError");
            ValidationSettings settings = new ValidationSettings();
            settings.AdditionalConstraints.Add(
                typeof(WriteLine), new List<Constraint>
                {
                    WhileParentConstraint.GetConstraint()
                });
            ShowValidationResults(ActivityValidationServices.Validate(
                WhileAndWriteLine(), settings));

            Console.WriteLine("\nWhileAndWriteLineNoError");
            ShowValidationResults(ActivityValidationServices.Validate(
                WhileAndWriteLine()));
        }

        private static Activity MySequenceWithError()
        {
            return new Sequence
            {
                Activities =
                {
                    new MySequenceWithConstraint
                    {
                        Activities = 
                        {
                            //no child activities is an error
                        }
                    }
                }
            };
        }

        private static Activity MySequenceNoError()
        {
            return new Sequence
            {
                Activities =
                {
                    new MySequenceWithConstraint
                    {
                        Activities = 
                        {
                            new WriteLine()
                        }
                    }
                }
            };
        }

        private static Activity WhileAndWriteLine()
        {
            return new Sequence
            {
                Activities =
                {
                    new While
                    {
                        Condition = true,
                        Body = new WriteLine()
                    }
                }
            };
        }

        private static void ShowValidationResults(ValidationResults vr)
        {
            Console.WriteLine("Total Errors: {0} - Warnings: {1}",
                vr.Errors.Count, vr.Warnings.Count);
            foreach (ValidationError error in vr.Errors)
            {
                Console.WriteLine("  Error: {0}", error.Message);
            }
            foreach (ValidationError warning in vr.Warnings)
            {
                Console.WriteLine("  Warning: {0}", warning.Message);
            }
        }
    }
}
