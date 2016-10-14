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
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.DurableInstancing;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Description;
using System.Xaml;
using System.Xml.Linq;
using ActivityLibrary;

namespace ServiceHost
{
    class Program
    {
        private static List<WorkflowServiceHost> _hosts =
            new List<WorkflowServiceHost>();

        static void Main(string[] args)
        {
            try
            {
                ItemSupportParticipant extension =
                    new ActivityLibrary.ItemSupportParticipant();
                //ItemSupportExtension extension = 
                //    new ActivityLibrary.ItemSupportExtension();

                extension.AddItemDefinition(101, 1.23M, 10);
                extension.AddItemDefinition(202, 2.34M, 20);
                extension.AddItemDefinition(303, 3.45M, 30);
                DisplayInventory("Before Execution", extension);

                CreateServiceHost("OrderEntryService.xamlx", extension);
                //CreateServiceHostCustomPersistence(
                //    "OrderEntryService.xamlx", extension);

                //Extra tests
                //CreateServiceHost("OrderEntryServiceDelay.xamlx", extension);
                //CreateServiceHostCustomPersistence("TestPersistence1.xamlx", extension);

                foreach (WorkflowServiceHost host in _hosts)
                {
                    host.Open();
                    foreach (var ep in host.Description.Endpoints)
                    {
                        Console.WriteLine("Contract: {0}",
                            ep.Contract.Name);
                        Console.WriteLine("    at {0}",
                            ep.Address);
                    }
                }

                Console.WriteLine("Press any key to stop hosting and exit");
                Console.ReadKey();

                DisplayInventory("After Execution", extension);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Service Exception: {0}", exception.Message);
            }
            finally
            {
                Console.WriteLine("Closing services...");
                foreach (WorkflowServiceHost host in _hosts)
                {
                    host.Close();
                }
                Console.WriteLine("Services closed");
                _hosts.Clear();
            }
        }

        private static WorkflowServiceHost CreateServiceHost(
            String xamlxName, IItemSupport extension)
        {
            WorkflowService wfService = LoadService(xamlxName);
            WorkflowServiceHost host = new WorkflowServiceHost(wfService);

            string connectionString = ConfigurationManager.ConnectionStrings
                ["InstanceStore"].ConnectionString;
            SqlWorkflowInstanceStoreBehavior storeBehavior =
                new SqlWorkflowInstanceStoreBehavior(connectionString);
            storeBehavior.InstanceCompletionAction =
                InstanceCompletionAction.DeleteAll;
            storeBehavior.InstanceLockedExceptionAction =
                InstanceLockedExceptionAction.BasicRetry;
            storeBehavior.InstanceEncodingOption =
                InstanceEncodingOption.GZip;
            storeBehavior.HostLockRenewalPeriod =
                TimeSpan.FromMinutes(1);

            //promption of persisted variables
            List<XName> variables = new List<XName>()
            {   
                XName.Get("OrderId", "ActivityLibrary.ItemSupportExtension")
            };
            storeBehavior.Promote("OrderEntry", variables, null);

            host.Description.Behaviors.Add(storeBehavior);

            //WorkflowUnhandledExceptionBehavior exceptionBehavior =
            //    new WorkflowUnhandledExceptionBehavior
            //{
            //    Action = WorkflowUnhandledExceptionAction.Cancel
            //};
            //host.Description.Behaviors.Add(exceptionBehavior);

            WorkflowIdleBehavior idleBehavior = new WorkflowIdleBehavior()
            {
                TimeToUnload = TimeSpan.FromSeconds(0)
            };
            host.Description.Behaviors.Add(idleBehavior);

            //add control endpoint in code
            //WorkflowControlEndpoint wce = new WorkflowControlEndpoint(
            //    new System.ServiceModel.WSHttpBinding(),
            //    new System.ServiceModel.EndpointAddress(
            //        "http://localhost:9000/OrderEntryControl"));
            //host.AddServiceEndpoint(wce);

            //add an extension instance for each workflow instance
            //host.WorkflowExtensions.Add<ItemSupportParticipant>(() =>
            //{
            //    ItemSupportParticipant ext = new ItemSupportParticipant();
            //    ext.AddItemDefinition(101, 1.23M, 10);
            //    ext.AddItemDefinition(202, 2.34M, 20);
            //    ext.AddItemDefinition(303, 3.45M, 30);
            //    return ext;
            //});

            if (extension != null)
            {
                host.WorkflowExtensions.Add(extension);
            }

            _hosts.Add(host);

            return host;
        }

        private static WorkflowServiceHost CreateServiceHostCustomPersistence(
            String xamlxName, IItemSupport extension)
        {
            WorkflowService wfService = LoadService(xamlxName);
            WorkflowServiceHost host = new WorkflowServiceHost(wfService);

            InstanceStore store = new FileSystemInstanceStore();
            host.DurableInstancingOptions.InstanceStore = store;

            WorkflowIdleBehavior idleBehavior = new WorkflowIdleBehavior()
            {
                TimeToUnload = TimeSpan.FromSeconds(0)
            };
            host.Description.Behaviors.Add(idleBehavior);

            //host.Faulted += new EventHandler(host_Faulted);

            if (extension != null)
            {
                host.WorkflowExtensions.Add(extension);
            }

            _hosts.Add(host);

            return host;
        }

        //static void host_Faulted(object sender, EventArgs e)
        //{
        //    Console.WriteLine("host_Faulted {0}:{1}", sender, e);
        //}

        private static WorkflowService LoadService(String xamlxName)
        {
            String fullFilePath = Path.Combine(
                @"..\..\..\ServiceLibrary", xamlxName);
            WorkflowService service =
                XamlServices.Load(fullFilePath) as WorkflowService;
            if (service != null)
            {
                return service;
            }
            else
            {
                throw new NullReferenceException(String.Format(
                    "Unable to load service definition from {0}", fullFilePath));
            }
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
    }
}
