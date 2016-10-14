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
using ActivityLibrary;

namespace RunWorkflow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nTest CalcShipping35Interop");
            WorkflowInvoker.Invoke(new CalcShipping35Interop());

            Console.WriteLine("\nTest CalcItemTotal35Interop");
            WorkflowInvoker.Invoke(new CalcItemTotal35Interop());
        }
    }
}
