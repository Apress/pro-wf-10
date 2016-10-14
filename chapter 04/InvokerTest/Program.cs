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
namespace InvokerHost
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Threading;

    using ActivityLibrary;

    class Program
    {
        private static AutoResetEvent waitEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("\n>>>> BasicWorkflowInvoker <<<<");
            BasicWorkflowInvoker();

            Console.WriteLine("\n>>>> BasicWorkflowInvokerProperties <<<<");
            BasicWorkflowInvokerProperties();

            Console.WriteLine("\n>>>> BasicWorkflowInvokerTimeSpan <<<<");
            BasicWorkflowInvokerTimeSpan();

            Console.WriteLine("\n>>>> WorkflowInvokerGeneric <<<<");
            WorkflowInvokerGeneric();

            Console.WriteLine("\n>>>> WorkflowInvokerAsync <<<<");
            WorkflowInvokerAsync();

            Console.WriteLine("\n>>>> WorkflowInvokerBeginInvoke <<<<");
            WorkflowInvokerBeginInvoke();
        }

        /// <summary>
        /// Basic use of WorkflowInvoker to execute a workflow
        /// </summary>
        private static void BasicWorkflowInvoker()
        {
            Console.WriteLine("Host: About to run workflow - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                IDictionary<String, Object> output = WorkflowInvoker.Invoke(
                    new HostingDemoWorkflow(),
                    new Dictionary<String, Object>
                    {
                        {"ArgNumberToEcho", 1001},
                    });

                Console.WriteLine("Host: Workflow completed - Thread:{0} - {1}",
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    output["Result"]);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Host: Workflow exception:{0}:{1}",
                    exception.GetType(), exception.Message);
            }
        }

        /// <summary>
        /// Basic use of WorkflowInvoker to execute a workflow
        /// using properties to set input arguments
        /// </summary>
        private static void BasicWorkflowInvokerProperties()
        {
            Console.WriteLine("Host: About to run workflow - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                HostingDemoWorkflow wf = new HostingDemoWorkflow();
                wf.ArgNumberToEcho = 1001;
                IDictionary<String, Object> output = WorkflowInvoker.Invoke(wf);

                Console.WriteLine("Host: Workflow completed - Thread:{0} - {1}",
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    output["Result"]);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Host: Workflow exception:{0}:{1}",
                    exception.GetType(), exception.Message);
            }
        }

        /// <summary>
        /// WorkflowInvoker using option TimeSpan to limit
        /// the amount of time allocated to execute the workflow
        /// </summary>
        private static void BasicWorkflowInvokerTimeSpan()
        {
            Console.WriteLine("Host: About to run workflow - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                HostingDemoWorkflow wf = new HostingDemoWorkflow();
                wf.ArgNumberToEcho = 1001;
                IDictionary<String, Object> output = WorkflowInvoker.Invoke(
                    wf, TimeSpan.FromSeconds(1));

                Console.WriteLine(
                    "Host: Workflow completed - Thread:{0} - {1}",
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    output["Result"]);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Host: Workflow exception:{0}:{1}",
                    exception.GetType(), exception.Message);
            }
        }

        /// <summary>
        /// Generic version of WorkflowInvoker.Invoke to
        /// execute an activity that returns a result directly
        /// </summary>
        private static void WorkflowInvokerGeneric()
        {
            Console.WriteLine("Host: About to run Activity - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                HostingDemoActivity activity = new HostingDemoActivity();
                activity.NumberToEcho = 1001;
                String result = WorkflowInvoker.Invoke<String>(activity);

                Console.WriteLine(
                    "Host: Activity completed - Thread:{0} - {1}",
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    result);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Host: Activity exception:{0}:{1}",
                    exception.GetType(), exception.Message);
            }
        }

        /// <summary>
        /// Use InvokeAsync
        /// </summary>
        private static void WorkflowInvokerAsync()
        {
            Console.WriteLine("Host: About to run workflow - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                WorkflowInvoker instance = new WorkflowInvoker(
                    new HostingDemoWorkflow());
                instance.InvokeCompleted +=
                    delegate(Object sender, InvokeCompletedEventArgs e)
                    {
                        Console.WriteLine(
                            "Host: Workflow completed - Thread:{0} - {1}",
                            System.Threading.Thread.CurrentThread.ManagedThreadId,
                            e.Outputs["Result"]);
                        waitEvent.Set();
                    };

                instance.InvokeAsync(new Dictionary<String, Object>
                    {
                        {"ArgNumberToEcho", 1001},
                    });

                Console.WriteLine("Host: Workflow started - Thread:{0}",
                    System.Threading.Thread.CurrentThread.ManagedThreadId);
                waitEvent.WaitOne();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Host: Workflow exception:{0}:{1}",
                    exception.GetType(), exception.Message);
            }
        }

        /// <summary>
        /// Use BeginInvoke/EndInvoke
        /// </summary>
        private static void WorkflowInvokerBeginInvoke()
        {
            Console.WriteLine("Host: About to run workflow - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                WorkflowInvoker instance = new WorkflowInvoker(
                    new HostingDemoWorkflow { ArgNumberToEcho = 1001 });
                IAsyncResult ar = instance.BeginInvoke(
                    BeginInvokeCallback, instance);

                Console.WriteLine(
                    "Host: Workflow started - Thread:{0}",
                    System.Threading.Thread.CurrentThread.ManagedThreadId);
                waitEvent.WaitOne();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Host: Workflow exception:{0}:{1}",
                    exception.GetType(), exception.Message);
            }
        }

        /// <summary>
        /// Callback when BeginInvoke is used
        /// </summary>
        /// <param name="ar"></param>
        private static void BeginInvokeCallback(IAsyncResult ar)
        {
            IDictionary<String, Object> output =
                ((WorkflowInvoker)ar.AsyncState).EndInvoke(ar);

            Console.WriteLine(
                "Host: BeginInvokeCallback Invoked - Thread:{0} - {1}",
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                output["Result"]);

            waitEvent.Set();
        }
    }
}
