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
using System.Collections.Generic;

namespace ExpressionActivities
{
    class Program
    {
        static void Main(string[] args)
        {
            Direct();
            Composition();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static Int32 Composition()
        {
            Int32 result = WorkflowInvoker.Invoke<Int32>(
                new AddNumbers(),
                new Dictionary<String, Object>
                {
                    {"NumberOne", 1},
                    {"NumberTwo", 2}
                });

            Console.WriteLine("Result: {0}", result);
            return result;
        }

        private static Int32 Direct()
        {
            Int32 result = WorkflowInvoker.Invoke<Int32>(
                new Add<Int32, Int32, Int32>(),
                new Dictionary<String, Object>
                {
                    {"Left", 1},
                    {"Right", 2}
                });
            Console.WriteLine("Result: {0}", result);
            return result;
        }
    }
}
