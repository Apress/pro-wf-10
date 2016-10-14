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
using System.Threading;
using ActivityLibrary;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RunActivity(new MySimpleParentTest());
                RunActivity(new MyWhileTest());
                RunActivity(new MyWhileExceptionTest());
                RunActivity(new MyWhileExceptionTest2());
                RunActivity(new MySequenceTest());
                RunActivity(new MyParallelTest());
                RunActivity(new MyActivityWithActionTest());

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                    "caught unhandled exception: {0}", exception.Message);
            }
        }

        private static void RunActivity(Activity activity)
        {
            RunActivity(activity, TestType.Normal);
            RunActivity(activity, TestType.Cancel);
            RunActivity(activity, TestType.Abort);
            RunActivity(activity, TestType.Terminate);
        }

        private static void RunActivity(Activity activity, TestType testType)
        {
            Console.WriteLine("\n{0} {1}", activity.DisplayName, testType);

            AutoResetEvent waitEvent = new AutoResetEvent(false);
            WorkflowApplication wfApp = new WorkflowApplication(activity);
            wfApp.Completed = (e) =>
            {
                Console.WriteLine("WorkflowApplication.Completed");
                waitEvent.Set();
            };

            wfApp.Aborted = (e) =>
            {
                Console.WriteLine("WorkflowApplication.Aborted");
                waitEvent.Set();
            };

            wfApp.OnUnhandledException = (e) =>
            {
                Console.WriteLine("WorkflowApplication.OnUnhandledException: {0}",
                    e.UnhandledException.Message);
                return UnhandledExceptionAction.Cancel;
            };

            wfApp.Run();

            switch (testType)
            {
                case TestType.Cancel:
                    Thread.Sleep(100);
                    wfApp.Cancel();
                    break;

                case TestType.Abort:
                    Thread.Sleep(100);
                    wfApp.Abort("Abort was called");
                    break;

                case TestType.Terminate:
                    Thread.Sleep(100);
                    wfApp.Terminate("Terminate was called");
                    break;
                default:
                    break;
            }

            waitEvent.WaitOne(TimeSpan.FromSeconds(60));
        }

        private enum TestType
        {
            Normal,
            Cancel,
            Abort,
            Terminate
        }
    }
}
