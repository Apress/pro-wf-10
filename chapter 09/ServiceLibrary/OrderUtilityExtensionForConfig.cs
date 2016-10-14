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
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;

namespace ServiceLibrary
{
    /// <summary>
    /// This version implements IServiceBehavior in order to 
    /// be added via a web.config
    /// </summary>
    public class OrderUtilityExtensionForConfig : IServiceBehavior
    {
        private Random random = new Random(Environment.TickCount);

        public OrderUtilityExtensionForConfig()
        {
            Console.WriteLine("New OrderUtilityExtension instance created");
        }

        public Int32 GetOrderId()
        {
            return random.Next(Int32.MaxValue);
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
        }

        public void Validate(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
        }
    }
}