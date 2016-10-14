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
using System.Activities.Hosting;
using System.Linq;
using System.Threading;
using ActivityLibrary;

namespace ProblemReporting
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AutoResetEvent syncEvent = new AutoResetEvent(false);

                WorkflowApplication wfApp =
                    new WorkflowApplication(new ProblemReporting());

                wfApp.Completed = delegate(
                    WorkflowApplicationCompletedEventArgs e)
                {
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

                HostEventNotifier extension = new HostEventNotifier();
                extension.Notification += delegate(
                    Object sender, HostNotifyEventArgs e)
                {
                    Console.WriteLine(e.Message);

                    var bookmarks = wfApp.GetBookmarks();
                    if (bookmarks != null && bookmarks.Count > 0)
                    {
                        Console.WriteLine("Select one of these available actions:");
                        foreach (BookmarkInfo bookmark in bookmarks)
                        {
                            Console.WriteLine("->{0}", bookmark.BookmarkName);
                        }
                    }

                    Boolean isWaitingForChoice = true;
                    while (isWaitingForChoice)
                    {
                        String newAction = Console.ReadLine();
                        if (IsBookmarkValid(wfApp, newAction))
                        {
                            isWaitingForChoice = false;
                            wfApp.ResumeBookmark(newAction, null);
                        }
                        else
                        {
                            Console.WriteLine("Incorrect choice!");
                        }
                    }
                };

                wfApp.Extensions.Add(extension);
                wfApp.Run();
                syncEvent.WaitOne();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: {0}", exception.Message);
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
