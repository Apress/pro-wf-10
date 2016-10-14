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
using System.Activities;
using System.Linq;
using System.Threading;
using ActivityLibrary;

namespace BookmarkCalculatorEvent
{
    class Program
    {
        static void Main(string[] args)
        {
            Boolean isRunning = true;
            while (isRunning)
            {
                try
                {
                    AutoResetEvent syncEvent = new AutoResetEvent(false);

                    WorkflowApplication wfApp =
                        new WorkflowApplication(new BookmarkCalculatorExtension());

                    wfApp.Completed = delegate(
                        WorkflowApplicationCompletedEventArgs e)
                    {
                        if (e.CompletionState == ActivityInstanceState.Closed)
                        {
                            Console.WriteLine("Result = {0}", e.Outputs["Result"]);
                        }
                        syncEvent.Set();
                    };

                    wfApp.Idle = delegate(WorkflowApplicationIdleEventArgs e)
                    {
                        Console.WriteLine("Workflow is idle");
                    };

                    wfApp.OnUnhandledException = delegate(
                        WorkflowApplicationUnhandledExceptionEventArgs e)
                    {
                        Console.WriteLine(e.UnhandledException.Message.ToString());
                        return UnhandledExceptionAction.Terminate;
                    };

                    HostQueueNotifier extension = new HostQueueNotifier();
                    wfApp.Extensions.Add(extension);
                    wfApp.Run();

                    if (extension.MessageAvailableEvent.WaitOne())
                    {
                        HostNotifyMessage msg = null;
                        lock (extension.MessageQueue)
                        {
                            if (extension.MessageQueue.Count > 0)
                            {
                                msg = extension.MessageQueue.Dequeue();
                            }
                        }

                        if (msg != null)
                        {
                            Console.WriteLine(msg.Message);
                            String expression = Console.ReadLine();
                            if ((String.IsNullOrEmpty(expression)) ||
                                (!String.IsNullOrEmpty(expression) &&
                                 expression.Trim().ToLower() == "quit"))
                            {
                                wfApp.Cancel();
                                Console.WriteLine("Exiting program");
                                isRunning = false;
                            }
                            else if (IsBookmarkValid(wfApp, msg.BookmarkName))
                            {
                                wfApp.ResumeBookmark(
                                    msg.BookmarkName, expression);
                            }
                        }
                    }

                    syncEvent.WaitOne();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error: {0}", exception.Message);
                }
            }
        }

        private static Boolean IsBookmarkValid(
            WorkflowApplication wfApp, String bookmarkName)
        {
            Boolean result = false;
            var bookmarks = wfApp.GetBookmarks();
            if (bookmarks != null)
            {
                result =
                    (from b in bookmarks
                     where b.BookmarkName == bookmarkName
                     select b).Any();
            }
            return result;
        }
    }
}
