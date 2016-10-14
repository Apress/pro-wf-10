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
using System.Activities.DurableInstancing;
using System.Configuration;
using System.Runtime.DurableInstancing;
using System.Threading;

namespace TestOrderScope
{
    class Program
    {
        static private AutoResetEvent waitHandle = new AutoResetEvent(false);
        static private AutoResetEvent unloadedHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            InstanceStore store = CreateInstanceStore();

            WorkflowApplication wfApp = SetupInstance(store);
            Guid instanceId = wfApp.Id;
            wfApp.Run();

            //wait for the wf to unload itself
            unloadedHandle.WaitOne(5000);
            Thread.Sleep(1000);
            wfApp = SetupInstance(store);
            wfApp.Load(instanceId);
            wfApp.Run();

            //wait for the wf to unload itself
            unloadedHandle.WaitOne(5000);
            Thread.Sleep(1000);
            wfApp = SetupInstance(store);
            wfApp.Load(instanceId);
            wfApp.Run();

            //wait for the wf to unload itself
            unloadedHandle.WaitOne(5000);
            Thread.Sleep(5000);
            wfApp = SetupInstance(store);
            wfApp.Load(instanceId);
            wfApp.Run();

            waitHandle.WaitOne(10000);
        }

        private static WorkflowApplication SetupInstance(
            InstanceStore store)
        {
            WorkflowApplication wfApp =
                new WorkflowApplication(new Workflow1());

            wfApp.Completed = (e) =>
            {
                Console.WriteLine("{0} Is Completed", e.InstanceId);
                waitHandle.Set();
            };

            wfApp.Idle = (e) =>
            {
                Console.WriteLine("{0} Is Idle", e.InstanceId);
            };
            wfApp.PersistableIdle = (e) =>
            {
                Console.WriteLine("{0} Is PersistableIdle", e.InstanceId);
                unloadedHandle.Set();
                return PersistableIdleAction.Unload;
            };

            wfApp.Unloaded = (e) =>
            {
                Console.WriteLine("{0} Is Unloaded", e.InstanceId);
            };
            wfApp.OnUnhandledException = (e) =>
            {
                Console.WriteLine("{0} OnUnhandledException: {1}",
                    e.InstanceId, e.UnhandledException.Message);
                return UnhandledExceptionAction.Cancel;
            };

            wfApp.InstanceStore = store;

            return wfApp;
        }

        private static InstanceStore CreateInstanceStore()
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
            return store;
        }
    }
}
