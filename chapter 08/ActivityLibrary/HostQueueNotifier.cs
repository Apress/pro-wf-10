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
using System.Threading;

namespace ActivityLibrary
{
    public class HostQueueNotifier : IHostNotification
    {
        public Queue<HostNotifyMessage> MessageQueue { get; private set; }
        public AutoResetEvent MessageAvailableEvent { get; private set; }

        public HostQueueNotifier()
        {
            MessageQueue = new Queue<HostNotifyMessage>();
            MessageAvailableEvent = new AutoResetEvent(false);
        }

        public void Notify(string message, String bookmarkName)
        {
            lock (MessageQueue)
            {
                MessageQueue.Enqueue(
                    new HostNotifyMessage(message, bookmarkName));
                MessageAvailableEvent.Set();
            }
        }
    }

    public class HostNotifyMessage
    {
        public String Message { get; private set; }
        public String BookmarkName { get; private set; }
        public HostNotifyMessage(String message, String bookmarkName)
        {
            Message = message;
            BookmarkName = bookmarkName;
        }
    }
}
