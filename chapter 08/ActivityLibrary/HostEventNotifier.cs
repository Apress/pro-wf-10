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
    public class HostEventNotifier : IHostNotification
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
    }

    public class HostNotifyEventArgs : EventArgs
    {
        public String Message { get; private set; }
        public String BookmarkName { get; private set; }
        public HostNotifyEventArgs(String message, String bookmarkName)
            : base()
        {
            Message = message;
            BookmarkName = bookmarkName;
        }
    }
}
