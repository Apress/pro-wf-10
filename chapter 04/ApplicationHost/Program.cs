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
namespace ApplicationHost
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Threading;
    using ActivityLibrary;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n>>>> Basic <<<<");
            BasicApplicationTest(TestScenario.Normal);
            Thread.Sleep(500);

            Console.WriteLine("\n>>>> Cancel <<<<");
            BasicApplicationTest(TestScenario.Cancel);
            Thread.Sleep(500);

            Console.WriteLine("\n>>>> Abort <<<<");
            BasicApplicationTest(TestScenario.Abort);
            Thread.Sleep(500);

            Console.WriteLine("\n>>>> Terminate <<<<");
            BasicApplicationTest(TestScenario.Terminate);
            Thread.Sleep(500);

            Console.WriteLine("\n>>>> BeginRun <<<<");
            BasicApplicationTest(TestScenario.BeginRun);
            Thread.Sleep(500);
        }

        /// <summary>
        /// Allows the demonstration of various scenarios using
        /// WorkflowApplication
        /// </summary>
        private static void BasicApplicationTest(TestScenario scenario)
        {
            AutoResetEvent waitEvent = new AutoResetEvent(false);

            WorkflowApplication wfApp = new WorkflowApplication(
                new HostingDemoWorkflow(),
                new Dictionary<String, Object>
                {
                    {"ArgNumberToEcho", 1001},
                });

            wfApp.Completed = delegate(WorkflowApplicationCompletedEventArgs e)
            {
                switch (e.CompletionState)
                {
                    case ActivityInstanceState.Closed:
                        Console.WriteLine("Host: {0} Closed - Thread:{1} - {2}",
                            wfApp.Id,
                            System.Threading.Thread.CurrentThread.ManagedThreadId,
                            e.Outputs["Result"]);
                        break;
                    case ActivityInstanceState.Canceled:
                        Console.WriteLine("Host: {0} Canceled - Thread:{1}",
                            wfApp.Id,
                            System.Threading.Thread.CurrentThread.ManagedThreadId);
                        break;
                    case ActivityInstanceState.Executing:
                        Console.WriteLine("Host: {0} Executing - Thread:{1}",
                            wfApp.Id,
                            System.Threading.Thread.CurrentThread.ManagedThreadId);
                        break;
                    case ActivityInstanceState.Faulted:
                        Console.WriteLine(
                            "Host: {0} Faulted - Thread:{1} - {2}:{3}",
                            wfApp.Id,
                            System.Threading.Thread.CurrentThread.ManagedThreadId,
                            e.TerminationException.GetType(),
                            e.TerminationException.Message);
                        break;
                    default:
                        break;
                }
                waitEvent.Set();
            };

            wfApp.OnUnhandledException =
                delegate(WorkflowApplicationUnhandledExceptionEventArgs e)
                {
                    Console.WriteLine(
                        "Host: {0} OnUnhandledException - Thread:{1} - {2}",
                        wfApp.Id,
                        System.Threading.Thread.CurrentThread.ManagedThreadId,
                        e.UnhandledException.Message);
                    waitEvent.Set();
                    return UnhandledExceptionAction.Cancel;
                };

            wfApp.Aborted = delegate(WorkflowApplicationAbortedEventArgs e)
            {
                Console.WriteLine("Host: {0} Aborted - Thread:{1} - {2}:{3}",
                    wfApp.Id,
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    e.Reason.GetType(), e.Reason.Message);
                waitEvent.Set();
            };

            wfApp.Idle = delegate(WorkflowApplicationIdleEventArgs e)
            {
                Console.WriteLine("Host: {0} Idle - Thread:{1}",
                    wfApp.Id,
                    System.Threading.Thread.CurrentThread.ManagedThreadId);
            };

            wfApp.PersistableIdle = delegate(WorkflowApplicationIdleEventArgs e)
            {
                Console.WriteLine("Host: {0} PersistableIdle - Thread:{1}",
                    wfApp.Id,
                    System.Threading.Thread.CurrentThread.ManagedThreadId);
                return PersistableIdleAction.Unload;
            };

            wfApp.Unloaded = delegate(WorkflowApplicationEventArgs e)
            {
                Console.WriteLine("Host: {0} Unloaded - Thread:{1}",
                    wfApp.Id,
                    System.Threading.Thread.CurrentThread.ManagedThreadId);
            };

            try
            {
                Console.WriteLine("Host: About to run {0} - Thread:{1}",
                    wfApp.Id,
                    System.Threading.Thread.CurrentThread.ManagedThreadId);

                //determine the demonstration scenario
                switch (scenario)
                {
                    case TestScenario.Normal:
                        wfApp.Run();
                        waitEvent.WaitOne();
                        break;

                    //case TestScenario.TimeSpan:
                    //    wfApp.Run(TimeSpan.FromSeconds(1));
                    //    waitEvent.WaitOne();
                    //    break;

                    case TestScenario.Cancel:
                        wfApp.Run();
                        //Wait just a bit then cancel the workflow
                        Thread.Sleep(1000);
                        wfApp.Cancel();
                        waitEvent.WaitOne();
                        break;

                    case TestScenario.Abort:
                        wfApp.Run();
                        //Wait just a bit then abort the workflow
                        Thread.Sleep(1000);
                        wfApp.Abort("My aborted reason");
                        waitEvent.WaitOne();
                        break;

                    case TestScenario.Terminate:
                        wfApp.Run();
                        //Wait just a bit then terminate the workflow
                        Thread.Sleep(1000);
                        wfApp.Terminate("My termination reason");
                        waitEvent.WaitOne();
                        break;

                    case TestScenario.BeginRun:
                        wfApp.BeginRun(delegate(IAsyncResult ar)
                        {
                            Console.WriteLine(
                                "Host: {0} BeginRunCallback - Thread:{1}",
                                wfApp.Id,
                                System.Threading.Thread.CurrentThread.ManagedThreadId);
                            ((WorkflowApplication)ar.AsyncState).EndRun(ar);
                        }, wfApp);
                        waitEvent.WaitOne();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Host: {0} exception:{1}:{2}",
                    wfApp.Id, exception.GetType(), exception.Message);
            }
        }

        private enum TestScenario
        {
            Normal,
            TimeSpan,
            Cancel,
            Abort,
            Terminate,
            BeginRun,
        }
    }
}
