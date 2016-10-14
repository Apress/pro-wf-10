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
using System.Threading;

namespace ActivityLibrary
{
    public class HostEventNotifierWithInterface : IHostNotification, 
        System.Activities.Hosting.IWorkflowInstanceExtension
    {
        public event EventHandler<HostNotifyEventArgs> Notification;

        public void Notify(string message, String bookmarkName)
        {
            if (Notification != null)
            {
                ThreadPool.QueueUserWorkItem(delegate(Object state)
                {
                    Notification(this,
                        new HostNotifyEventArgs(state as String, bookmarkName));
                }, message);
            }
        }

        public System.Collections.Generic.IEnumerable<object> GetAdditionalExtensions()
        {
            Console.WriteLine("GetAdditionalExtensions");
            return null;
            //throw new NotImplementedException();
        }

        public void SetInstance(System.Activities.Hosting.WorkflowInstanceProxy instance)
        {
            Console.WriteLine("SetInstance: {0}", instance.Id);
        }
    }
}
