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
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Activities;
using System.Xaml;

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
                //Test V1
                //CreateServiceHost("OrderProcessing.xamlx");

                //Test V2
                //CreateServiceHost("OrderProcessing2.xamlx");
                //CreateServiceHost("ShipOrder.xamlx");

                //Test V3
                //CreateServiceHost("OrderProcessing2.xamlx");
                //CreateServiceHost("ShipOrderContent.xamlx");

                //Test V4
                //CreateServiceHost("OrderProcessing3.xamlx");
                //CreateServiceHost("ShipOrderContent.xamlx");
                //CreateServiceHost("CreditApproval.xamlx");

                //Test V5
                CreateServiceHost("OrderProcessing4.xamlx");
                CreateServiceHost("ShipOrderContent.xamlx");
                CreateServiceHost("CreditApproval.xamlx");

                //Test V6 - only for use with WorkflowClientWithFault
                //CreateServiceHost("OrderProcessing5.xamlx");
                //CreateServiceHost("ShipOrderContent.xamlx");
                //CreateServiceHost("CreditApproval.xamlx");

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

        private static WorkflowServiceHost CreateServiceHost(String xamlxName)
        {
            WorkflowService wfService = LoadService(xamlxName);
            WorkflowServiceHost host = new WorkflowServiceHost(wfService);

            //host.UnknownMessageReceived += (sender, e) =>
            //{
            //    Console.WriteLine("Service1 UnknownMessageReceived");
            //    Console.WriteLine(((UnknownMessageReceivedEventArgs)e).Message);
            //};
            //host.Faulted += (sender, e) =>
            //{
            //    Console.WriteLine("Service1 Faulted");
            //};

            //
            //add the extension
            //

            //host.WorkflowExtensions.Add<ServiceLibrary.OrderUtilityExtension>(
            //    () => new ServiceLibrary.OrderUtilityExtension());
            host.WorkflowExtensions.Add(new ServiceLibrary.OrderUtilityExtension());

            _hosts.Add(host);

            return host;
        }

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

    }
}
