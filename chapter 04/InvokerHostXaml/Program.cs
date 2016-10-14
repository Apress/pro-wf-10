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
namespace InvokerHostXaml
{
    using System;
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n>>>> From Xaml <<<<");
            RunFromXaml();
        }

        private static void RunFromXaml()
        {
            Console.WriteLine("Host: About to run workflow - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                String fullFilePath =
                    @"..\..\..\ActivityLibrary\HostingDemoWorkflow.xaml";
                Activity activity = ActivityXamlServices.Load(fullFilePath);
                //activity is a DynamicActivity
                if (activity == null)
                {
                    throw new NullReferenceException(String.Format(
                        "Unable to deserialize {0}", fullFilePath));
                }

                IDictionary<String, Object> output = WorkflowInvoker.Invoke(
                    activity, new Dictionary<String, Object>
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
    }
}
