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
using System.Configuration;
using System.Data.SqlClient;
using System.ServiceModel.Activities;
using OrderEntryConsoleClient.OrderEntryReference;

namespace OrderEntryConsoleClient
{
    class Program
    {
        static private Int32 lastOrderId = 0;
        static private Dictionary<Int32, Guid> queriedInstances =
            new Dictionary<int, Guid>();

        static void Main(string[] args)
        {
            Boolean isDone = false;
            while (!isDone)
            {
                Console.WriteLine(
                  "\nCommands: start | add | complete | query | cancel | exit");
                String command = Console.ReadLine();
                if (String.IsNullOrEmpty(command))
                {
                    command = "exit";
                }
                switch (command.ToLower())
                {
                    case "start":
                        Start();
                        break;
                    case "add":
                        Add();
                        break;
                    case "complete":
                        Complete();
                        break;
                    case "query":
                        Query();
                        break;
                    case "cancel":
                        Cancel();
                        break;
                    case "exit":
                        Console.WriteLine("Exiting application...");
                        isDone = true;
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }

        static Int32 GetOrderId(Boolean isPromptForEntry)
        {
            if (lastOrderId == 0 || isPromptForEntry)
            {
                Console.WriteLine("Enter an OrderId (int) for the order");
                String input = Console.ReadLine();
                Int32 value = 0;
                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("A value must be entered");
                    return value;
                }
                Int32.TryParse(input, out value);

                if (value == 0)
                {
                    Console.WriteLine("OrderId must not be zero");
                    return value;
                }

                lastOrderId = value;
            }

            return lastOrderId;
        }

        static void Start()
        {
            try
            {
                Int32 orderId = GetOrderId(true);
                if (orderId == 0)
                {
                    return;
                }

                OrderEntryReference.OrderEntryClient client =
                    new OrderEntryReference.OrderEntryClient();
                client.StartOrder(orderId);
                lastOrderId = orderId;
                Console.WriteLine("New order {0} started", orderId);
            }
            catch (Exception exception)
            {
                lastOrderId = 0;
                Console.WriteLine("Start Unhandled exception: {0}",
                    exception.Message);
            }
        }

        static void Add()
        {
            try
            {
                Int32 orderId = GetOrderId(false);
                if (orderId == 0)
                {
                    return;
                }

                Console.WriteLine(
                    "Enter ItemId and Quantity (Ex: 101 1)");
                String input = Console.ReadLine();

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

                Item item = new Item
                {
                    ItemId = itemId,
                    Quantity = quantity
                };

                OrderEntryReference.OrderEntryClient client =
                    new OrderEntryReference.OrderEntryClient();
                client.AddItem(orderId, item);
                Console.WriteLine("Ordered {0} of ItemId {1} for OrderId {2}",
                    item.Quantity, item.ItemId, orderId);
            }
            catch (Exception exception)
            {
                lastOrderId = 0;
                Console.WriteLine("Add Unhandled exception: {0}",
                    exception.Message);
            }
        }

        static void Complete()
        {
            try
            {
                Int32 orderId = GetOrderId(false);
                if (orderId == 0)
                {
                    return;
                }

                OrderEntryReference.OrderEntryClient client =
                    new OrderEntryReference.OrderEntryClient();
                List<Item> items = client.OrderComplete(orderId);
                lastOrderId = 0;
                Console.WriteLine("Order {0} Is Completed", orderId);
                if (items != null && items.Count > 0)
                {
                    Console.WriteLine("\nOrdered Items:");
                    foreach (Item i in items)
                    {
                        Console.WriteLine(
                            "ItemId={0}, Quantity={1}, UnitPrice={2}, Total={3}",
                            i.ItemId, i.Quantity, i.UnitPrice, i.TotalPrice);
                    }
                }
            }
            catch (Exception exception)
            {
                lastOrderId = 0;
                Console.WriteLine("Complete Unhandled exception: {0}",
                    exception.Message);
            }
        }

        static void Query()
        {
            String sql =
             @"Select Value1, InstanceId from 
               [System.Activities.DurableInstancing].[InstancePromotedProperties]
               where PromotionName = 'OrderEntry'               
               order by Value1";
            try
            {
                queriedInstances.Clear();
                string connectionString = ConfigurationManager.ConnectionStrings
                    ["InstanceStore"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Promoted OrderId values:");
                        while (reader.Read())
                        {
                            Int32 orderId = (Int32)reader["Value1"];
                            Guid instanceId = (Guid)reader["InstanceId"];

                            Console.WriteLine("OrderId={0}, InstanceId={1}",
                                orderId, instanceId);
                            if (!queriedInstances.ContainsKey(orderId))
                            {
                                queriedInstances.Add(orderId, instanceId);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                lastOrderId = 0;
                Console.WriteLine("Query Unhandled exception: {0}",
                    exception.Message);
            }
        }

        static void Cancel()
        {
            try
            {
                Console.WriteLine("Enter an OrderId to cancel");
                String input = Console.ReadLine();
                Int32 orderIdToCancel = 0;
                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("A value must be entered");
                    return;
                }
                Int32.TryParse(input, out orderIdToCancel);
                if (orderIdToCancel == 0)
                {
                    Console.WriteLine("OrderId must not be zero");
                    return;
                }

                Guid instanceId = Guid.Empty;
                if (!queriedInstances.TryGetValue(orderIdToCancel, 
                    out instanceId))
                {
                    Console.WriteLine("Instance not found");
                    return;
                }

                using (WorkflowControlClient client =
                    new WorkflowControlClient("ClientControlEndpoint"))
                {
                    client.Cancel(instanceId);
                }
            }
            catch (Exception exception)
            {
                lastOrderId = 0;
                Console.WriteLine("Cancel Unhandled exception: {0}",
                    exception.Message);
            }
        }
    }
}
