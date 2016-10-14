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
using System.ServiceModel;
using System.ServiceModel.Activities.Description;
using System.Threading;
using ActivityLibrary;
using ActivityLibrary35;

namespace GuessingGame
{
    class Program
    {
        private static GuessingGameService ggService;
        private static AutoResetEvent waitEvent;

        [ServiceContract]
        public interface IServiceStarter
        {
            [OperationContract(IsOneWay = true)]
            void Start();
        }

        static void Main(string[] args)
        {
            System.ServiceModel.Activities.WorkflowServiceHost host = null;
            try
            {
                waitEvent = new AutoResetEvent(false);
                string baseAddr = "net.pipe://localhost/GuessingGame";

                host = new System.ServiceModel.Activities.WorkflowServiceHost(
                        new GuessingGame35Interop(), new Uri(baseAddr));

                System.Workflow.Activities.ExternalDataExchangeService des =
                    new System.Workflow.Activities.ExternalDataExchangeService();
                ggService = new GuessingGameService();
                ggService.MessageReceived +=
                    new EventHandler<MessageReceivedEventArgs>(
                        Service_MessageReceived);
                des.AddService(ggService);

                WorkflowRuntimeEndpoint endpoint = new WorkflowRuntimeEndpoint();
                endpoint.AddService(des);
                host.AddServiceEndpoint(endpoint);
                host.AddDefaultEndpoints();

                ////configure persistence
                //string connectionString = ConfigurationManager.ConnectionStrings
                //    ["InstanceStore"].ConnectionString;
                //SqlWorkflowInstanceStoreBehavior storeBehavior =
                //    new SqlWorkflowInstanceStoreBehavior(connectionString);
                //host.Description.Behaviors.Add(storeBehavior);

                //WorkflowIdleBehavior idleBehavior = new WorkflowIdleBehavior();
                //idleBehavior.TimeToUnload = TimeSpan.FromSeconds(0);
                //host.Description.Behaviors.Add(idleBehavior);

                host.Open();

                IServiceStarter client =
                    ChannelFactory<IServiceStarter>.CreateChannel(
                        new NetNamedPipeBinding(), new EndpointAddress(baseAddr));
                client.Start();
                waitEvent.WaitOne(TimeSpan.FromMinutes(2));

                Console.WriteLine("Program exiting...");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unhandled exception: {0}",
                    exception.Message);
            }
            finally
            {
                if (host != null)
                {
                    host.Close();
                }
            }
        }

        private static void Service_MessageReceived(
            object sender, MessageReceivedEventArgs e)
        {
            Action<Guid, String> handler = HandleNewInput;
            handler.BeginInvoke(e.InstanceId, e.Message,
                ar => handler.EndInvoke(ar), handler);
        }

        private static void HandleNewInput(Guid instanceId, String message)
        {
            Console.WriteLine(message);
            if (message.StartsWith("Congratulations"))
            {
                waitEvent.Set();
                return;
            }

            String input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input))
            {
                Int32 guess = 0;
                if (Int32.TryParse(input, out guess))
                {
                    ggService.OnGuessReceived(
                        new GuessReceivedEventArgs(instanceId, guess));
                }
            }
            else
            {
                waitEvent.Set();
            }
        }
    }
}
