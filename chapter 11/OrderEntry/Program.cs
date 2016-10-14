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
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.DurableInstancing;
using System.Threading;
using ActivityLibrary;

namespace OrderEntry
{
    class Program
    {
        static private AutoResetEvent _unloadedEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            InstanceStore store = CreateInstanceStore();
            //InstanceStore store = CreateCustomInstanceStore();

            ItemSupportExtension extension = new ItemSupportExtension();
            extension.AddItemDefinition(101, 1.23M, 10);
            extension.AddItemDefinition(202, 2.34M, 20);
            extension.AddItemDefinition(303, 3.45M, 30);
            DisplayInventory("Before Execution", extension);

            Guid instanceId = Guid.Empty;
            StartNewInstance(ref instanceId, store, extension);
            Boolean isRunning = true;
            while (isRunning)
            {
                Console.WriteLine(
                    "Enter ItemId and Quantity (Ex: 101 1) or [Enter] to quit");
                String input = Console.ReadLine();
                if (!String.IsNullOrEmpty(input))
                {
                    AddItem(instanceId, store, extension, input);
                }
                else
                {
                    OrderComplete(instanceId, store, extension);
                    isRunning = false;
                }
            }

            DisplayInventory("After Execution", extension);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            store.Execute(store.CreateInstanceHandle(),
                new DeleteWorkflowOwnerCommand(), TimeSpan.FromSeconds(10));
        }

        private static void StartNewInstance(
            ref Guid instanceId, InstanceStore store, IItemSupport extension)
        {
            WorkflowApplication wfApp = SetupInstance(
                ref instanceId, store, extension);
            wfApp.Run();
            _unloadedEvent.WaitOne(5000);
        }

        private static void AddItem(Guid instanceId, InstanceStore store,
            IItemSupport extension, String input)
        {
            Int32 itemId = 0;
            Int32 quantity = 0;
            String[] parts = input.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Incorrect number of arguments entered!");
                return;
            }
            Int32.TryParse(parts[0], out itemId);
            Int32.TryParse(parts[1], out quantity);
            if (itemId == 0 || quantity == 0)
            {
                Console.WriteLine("Arguments in incorrect format!");
                return;
            }

            WorkflowApplication wfApp = SetupInstance(
                ref instanceId, store, extension);
            Item item = new Item
            {
                ItemId = itemId,
                Quantity = quantity
            };

            wfApp.ResumeBookmark("AddItem", item);
            _unloadedEvent.WaitOne(5000);
        }

        private static void OrderComplete(
            Guid instanceId, InstanceStore store, IItemSupport extension)
        {
            WorkflowApplication wfApp = SetupInstance(
                ref instanceId, store, extension);
            wfApp.Completed = (e) =>
            {
                Console.WriteLine("{0} Is Completed", e.InstanceId);
                List<Item> items = e.Outputs["Items"] as List<Item>;
                Console.WriteLine("\nOrdered Items:");
                foreach (Item i in items)
                {
                    Console.WriteLine(
                        "ItemId={0}, Quantity={1}, UnitPrice={2}, Total={3}",
                        i.ItemId, i.Quantity, i.UnitPrice, i.TotalPrice);
                }
            };

            wfApp.ResumeBookmark("OrderComplete", null);
            _unloadedEvent.WaitOne(5000);
        }

        private static WorkflowApplication SetupInstance(
            ref Guid instanceId, InstanceStore store, IItemSupport extension)
        {
            WorkflowApplication wfApp =
                new WorkflowApplication(new OrderEntry());

            wfApp.Idle = (e) =>
            {
                Console.WriteLine("{0} Is Idle", e.InstanceId);
            };
            wfApp.PersistableIdle = (e) =>
            {
                Console.WriteLine("{0} Is PersistableIdle", e.InstanceId);
                return PersistableIdleAction.Unload;
            };
            wfApp.Unloaded = (e) =>
            {
                Console.WriteLine("{0} Is Unloaded", e.InstanceId);
                _unloadedEvent.Set();
            };
            wfApp.OnUnhandledException = (e) =>
            {
                Console.WriteLine("{0} OnUnhandledException: {1}",
                    e.InstanceId, e.UnhandledException.Message);
                return UnhandledExceptionAction.Cancel;
            };

            wfApp.InstanceStore = store;
            wfApp.Extensions.Add(extension);

            if (instanceId == Guid.Empty)
            {
                instanceId = wfApp.Id;
            }
            else
            {
                wfApp.Load(instanceId);
            }
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

        private static void DisplayInventory(String desc, IItemSupport extension)
        {
            Console.WriteLine("\nItem inventory {0}:", desc);
            foreach (ItemDefinition item in extension.GetItemDefinitions())
            {
                Console.WriteLine("ItemId={0}, QtyAvailable={1}",
                    item.ItemId, item.QtyAvailable);
            }
            Console.WriteLine("");
        }

        private static InstanceStore CreateCustomInstanceStore()
        {
            InstanceStore store = new FileSystemInstanceStore();
            InstanceView view = store.Execute(
                store.CreateInstanceHandle(),
                new CreateWorkflowOwnerCommand(),
                TimeSpan.FromSeconds(30));
            store.DefaultInstanceOwner = view.InstanceOwner;
            return store;
        }
    }
}
