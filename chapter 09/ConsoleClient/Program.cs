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
using System.ServiceModel;
using System.ServiceModel.Channels;
using ConsoleClient.OrderProcessing;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CallViaProxy();
            CallViaInterface();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void CallViaProxy()
        {
            OrderProcessingClient client = null;
            try
            {
                Console.WriteLine("About to invoke OrderProcessing service");
                client = new OrderProcessingClient();

                OrderProcessingRequest request = new OrderProcessingRequest();
                request.CreditCard = "4444111111111111";
                request.CreditCardExpiration = "0611";
                request.CustomerName = "Joe Consumer";
                request.CustomerAddress = "100 Main Street";
                request.CustomerEmail = "joe@foo.com";
                request.TotalAmount = 75.00M;
                request.Items = new List<Item>
                {
                    new Item { ItemId = 1234, Quantity = 1 },
                    new Item { ItemId = 2345, Quantity = 3 },
                };

                OrderProcessingResponse response = client.ProcessOrder(request);

                Console.WriteLine("Response IsSuccessful: {0}",
                    response.IsSuccessful);
                Console.WriteLine("Response OrderId: {0}",
                    response.OrderId);
                Console.WriteLine("Response ShipDate: {0:D}",
                    response.ShipDate);
                Console.WriteLine("Response CreditAuthCode: {0}",
                    response.CreditAuthCode);

                //Int32 orderId;
                //Boolean isSuccessful;
                //DateTime shipDate;
                //String creditAuthCode;

                //creditAuthCode = client.ProcessOrder(
                //    "44441111111111111111", "0611", "100 Main Street",
                //    "joe@foo.com", "Joe Consumer",
                //    new Item[2]
                //    {
                //        new Item { ItemId = 1234, Quantity = 1 },
                //        new Item { ItemId = 2345, Quantity = 3 },
                //    },
                //    75.00M, out orderId, out isSuccessful, out shipDate);

                //Console.WriteLine("Response IsSuccessful: {0}", isSuccessful);
                //Console.WriteLine("Response OrderId: {0}", orderId);
                //Console.WriteLine("Response ShipDate: {0:D}", shipDate);
                //Console.WriteLine("Response CreditAuthCode: {0}", creditAuthCode);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unhandled exception: {0}", exception.Message);
            }
            finally
            {
                client.Close();
            }
        }

        static void CallViaInterface()
        {
            IOrderProcessing client = null;
            try
            {
                Console.WriteLine("About to invoke OrderProcessing service");

                WSHttpBinding binding = new WSHttpBinding(
                    "WSHttpBinding_IOrderProcessing");
                EndpointAddress epAddr = new EndpointAddress(
                    "http://localhost:8080/OrderProcessing.xamlx");

                client = ChannelFactory<IOrderProcessing>.CreateChannel(
                    binding, epAddr);

                OrderProcessingRequest request = new OrderProcessingRequest();
                request.CreditCard = "4444111111111111";
                request.CreditCardExpiration = "0611";
                request.CustomerName = "Joe Consumer";
                request.CustomerAddress = "100 Main Street";
                request.CustomerEmail = "joe@foo.com";
                request.TotalAmount = 75.00M;
                request.Items = new List<Item>
                {
                    new Item { ItemId = 1234, Quantity = 1 },
                    new Item { ItemId = 2345, Quantity = 3 },
                };

                ProcessOrderResponse poResponse = client.ProcessOrder(
                    new ProcessOrderRequest(request));

                OrderProcessingResponse response =
                    poResponse.OrderProcessingResponse;

                Console.WriteLine("Response IsSuccessful: {0}",
                    response.IsSuccessful);
                Console.WriteLine("Response OrderId: {0}",
                    response.OrderId);
                Console.WriteLine("Response ShipDate: {0:D}",
                    response.ShipDate);
                Console.WriteLine("Response CreditAuthCode: {0}",
                    response.CreditAuthCode);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unhandled exception: {0}", exception.Message);
            }
            finally
            {
                ((IChannel)client).Close();
            }
        }
    }
}
