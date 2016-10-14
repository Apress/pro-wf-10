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
using System.Threading;

namespace WorkflowClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteClientWorkflow();

            //ReadLineWithPromptTest();
            //ReadInt32WithPromptTest();
            //ReadDateTimeWithPromptTest();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void ExecuteClientWorkflow()
        {
            WorkflowApplication wfApp = new WorkflowApplication(
                new InitiateOrderProcessing());

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            wfApp.Completed = (a) =>
            {
                waitEvent.Set();
            };

            wfApp.OnUnhandledException = (e) =>
            {
                Console.WriteLine("OnUnhandledException: {0}",
                    e.UnhandledException.Message);
                return UnhandledExceptionAction.Cancel;
            };

            wfApp.Run();
            waitEvent.WaitOne(90000);
        }


        private static void ReadLineWithPromptTest()
        {
            WorkflowApplication wfApp = new WorkflowApplication(
                new ReadLineWithPrompt<String>(),
                new Dictionary<String, Object>
                {
                    { "Prompt", "Please enter a string" }
                });

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            wfApp.Completed = (a) =>
            {
                Console.WriteLine("Result={0}", a.Outputs["Result"]);
                waitEvent.Set();
            };

            wfApp.Idle = (a) =>
            {
                Console.WriteLine("Is Idle");
            };

            wfApp.Run();
            waitEvent.WaitOne(10000);
        }

        private static void ReadInt32WithPromptTest()
        {
            WorkflowApplication wfApp = new WorkflowApplication(
                new ReadLineWithPrompt<Int32>(),
                new Dictionary<String, Object>
                {
                    { "Prompt", "Please enter an integer" }
                });

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            wfApp.Completed = (a) =>
            {
                Console.WriteLine("Result={0}", a.Outputs["Result"]);
                waitEvent.Set();
            };

            wfApp.Idle = (a) =>
            {
                Console.WriteLine("Is Idle");
            };

            wfApp.Run();
            waitEvent.WaitOne(10000);
        }

        private static void ReadDateTimeWithPromptTest()
        {
            WorkflowApplication wfApp = new WorkflowApplication(
                new ReadLineWithPrompt<DateTime>(),
                new Dictionary<String, Object>
                {
                    { "Prompt", "Please enter a date" }
                });

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            wfApp.Completed = (a) =>
            {
                Console.WriteLine("Result={0}", a.Outputs["Result"]);
                waitEvent.Set();
            };

            wfApp.OnUnhandledException = (a) =>
            {
                Console.WriteLine("OnUnhandledException: {0}", a.UnhandledException.Message);
                waitEvent.Set();
                return UnhandledExceptionAction.Cancel;
            };

            wfApp.Run();
            waitEvent.WaitOne(10000);
        }

    }
}
