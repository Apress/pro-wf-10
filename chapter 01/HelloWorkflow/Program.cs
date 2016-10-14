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
using System.Collections.Generic;

namespace HelloWorkflow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nExecute: HelloWorkflow");
            WorkflowInvoker.Invoke(new Workflow1());

            Console.WriteLine("\nExecute: HelloWorkflow Dup");
            WorkflowInvoker.Invoke(new Workflow1Dup());

            Console.WriteLine("\nExecute: HelloWorkflowParameters - Dictionary");
            WorkflowInvoker.Invoke(new HelloWorkflowParameters(),
                new Dictionary<String, Object>
                {
                    {"ArgFirstName", "Bruce"}
                });

            Console.WriteLine("\nExecute: HelloWorkflowParameters - Properties");
            HelloWorkflowParameters wf = new HelloWorkflowParameters();
            wf.ArgFirstName = "Bruce";
            WorkflowInvoker.Invoke(wf);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
