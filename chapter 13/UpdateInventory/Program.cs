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

namespace UpdateInventory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nUpdateInventory without exception");
            RunWorkflow(new UpdateInventory(), 43687, false);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventory with exception");
            RunWorkflow(new UpdateInventory(), 43687, true);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryTryCatch with exception");
            RunWorkflow(new UpdateInventoryTryCatch(), 43687, true);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryTryCatch2 with exception");
            RunWorkflow(new UpdateInventoryTryCatch2(), 43687, true);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryTran without exception");
            RunWorkflow(new UpdateInventoryTran(), 43687, false);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryTran with exception ");
            RunWorkflow(new UpdateInventoryTran(), 43687, true);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventory with Host Tran and exception");
            RunWorkflowWithTran(new UpdateInventory(), 43687, true);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryComp without exception ");
            RunWorkflow(new UpdateInventoryComp(), 43687, false);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryComp with exception ");
            RunWorkflow(new UpdateInventoryComp(), 43687, true);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryManualComp without exception ");
            RunWorkflow(new UpdateInventoryManualComp(), 43687, false);
            Thread.Sleep(4000);

            Console.WriteLine("\nUpdateInventoryManualComp with exception ");
            RunWorkflow(new UpdateInventoryManualComp(), 43687, true);
            Thread.Sleep(4000);

            //Console.WriteLine("\nUpdateInventoryCompCancel with exception ");
            //RunWorkflow(new UpdateInventoryCompCancel(), 43687, true);
            //Thread.Sleep(4000);
        }

        private static void RunWorkflow(Activity wf,
            Int32 orderId, Boolean isDemoException)
        {
            try
            {
                DisplayInventory(orderId, "Starting");
                AutoResetEvent syncEvent = new AutoResetEvent(false);
                WorkflowApplication wfApp =
                    new WorkflowApplication(wf, new Dictionary<String, Object>
                        {
                            {"ArgSalesOrderId", orderId},
                            {"ArgIsDemoException", isDemoException}
                        });

                wfApp.Completed = delegate(
                    WorkflowApplicationCompletedEventArgs e)
                {
                    syncEvent.Set();
                };

                wfApp.OnUnhandledException = delegate(
                    WorkflowApplicationUnhandledExceptionEventArgs e)
                {
                    Console.WriteLine("OnUnhandledException: {0}",
                        e.UnhandledException.Message);
                    return UnhandledExceptionAction.Cancel; //needed to compensate
                };

                //instance.Aborted = delegate(
                //    WorkflowApplicationAbortedEventArgs e)
                //{
                //    Console.WriteLine("Aborted: {0}", e.Reason.Message);
                //    syncEvent.Set();
                //};

                //instance.Idle = delegate(WorkflowApplicationIdleEventArgs e)
                //{
                //    Console.WriteLine("Workflow is idle");
                //};

                wfApp.Run();

                //TESTING FOR CANCELLATION LOGIC
                //Thread.Sleep(3000);
                //instance.Cancel();
                //TESTING FOR CANCELLATION LOGIC

                syncEvent.WaitOne();
                DisplayInventory(orderId, "Ending");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: {0}", exception.Message);
            }
        }

        private static void DisplayInventory(Int32 orderId, String desc)
        {
            WorkflowInvoker.Invoke(
                new DisplayInventory(), new Dictionary<String, Object>
                    {
                        {"ArgSalesOrderId", orderId},
                        {"ArgDescription", desc}
                    });
        }

        private static void RunWorkflowWithTran(Activity wf,
            Int32 orderId, Boolean isDemoException)
        {
            try
            {
                DisplayInventory(orderId, "Starting");

                using (System.Transactions.TransactionScope scope =
                    new System.Transactions.TransactionScope())
                {
                    WorkflowInvoker.Invoke(wf, new Dictionary<String, Object>
                        {
                            {"ArgSalesOrderId", orderId},
                            {"ArgIsDemoException", isDemoException}
                        });

                    scope.Complete();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: {0}", exception.Message);
            }
            finally
            {
                DisplayInventory(orderId, "Ending");
            }
        }
    }
}
