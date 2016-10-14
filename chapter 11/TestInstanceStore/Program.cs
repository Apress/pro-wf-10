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
using System.Linq;
using System.Activities;
using System.Activities.Statements;
using System.Configuration;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Runtime.DurableInstancing;

using System.Threading;

using ActivityLibrary;

namespace TestInstanceStore
{
    using System.Activities.DurableInstancing;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Runtime.DurableInstancing;

    class Program
    {
        static void Main(string[] args)
        {
            FileSystemTest();
            DurableDelayTest();
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void FileSystemTest()
        {
            Boolean isRunning = true;
            Guid instanceId = Guid.Empty;
            ManualResetEvent completedWaitEvent = new ManualResetEvent(false);

            while (isRunning)
            {
                AutoResetEvent persistedWaitEvent = new AutoResetEvent(false);
                WorkflowApplication wfApp =
                    new WorkflowApplication(new Workflow1());
                wfApp.Completed = (e) =>
                {
                    Console.WriteLine("Completed");
                    isRunning = false;
                    completedWaitEvent.Set();
                };
                wfApp.Idle = (e) =>
                {
                    Console.WriteLine("Idle");
                };

                wfApp.PersistableIdle = (e) =>
                {
                    Console.WriteLine("PersistableIdle");
                    persistedWaitEvent.Set();
                    return PersistableIdleAction.Unload;
                };

                wfApp.InstanceStore = new FileSystemInstanceStore();
                if (instanceId == Guid.Empty)
                {
                    instanceId = wfApp.Id;
                }
                else
                {
                    wfApp.Load(instanceId);
                }

                wfApp.Run();

                persistedWaitEvent.WaitOne(3000);
                Thread.Sleep(3000);
            }

            completedWaitEvent.WaitOne(10000);
        }

        private static void DurableDelayTest()
        {
            string connectionString = ConfigurationManager.ConnectionStrings
                ["InstanceStore"].ConnectionString;
            InstanceStore store =
                new SqlWorkflowInstanceStore(connectionString);
            InstanceView view = store.Execute(
                store.CreateInstanceHandle(),
                new CreateWorkflowOwnerCommand(),
                TimeSpan.FromSeconds(30));
            store.DefaultInstanceOwner = view.InstanceOwner;

            ManualResetEvent completedWaitEvent = new ManualResetEvent(false);

            WorkflowApplication wfApp = new WorkflowApplication(new Workflow1());
            wfApp.InstanceStore = store;

            wfApp.Completed = (e) =>
            {
                Console.WriteLine("Completed");
                completedWaitEvent.Set();
            };
            wfApp.Idle = (e) =>
            {
                Console.WriteLine("Idle");
            };

            wfApp.PersistableIdle = (e) =>
            {
                Console.WriteLine("PersistableIdle");
                return PersistableIdleAction.Unload;
            };

            wfApp.Run();
            completedWaitEvent.WaitOne(5000);

        }
    }
}
