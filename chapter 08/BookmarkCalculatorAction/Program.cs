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
using System.Activities.Statements;
using System.Linq;
using System.Threading;

namespace BookmarkCalculatorAction
{
    class Program
    {
        private static Boolean isRunning = true;
        private static WorkflowApplication wfApp;

        static void Main(string[] args)
        {
            while (isRunning)
            {
                try
                {
                    AutoResetEvent syncEvent = new AutoResetEvent(false);

#if INVOKEACTION
                    BookmarkCalculatorInvokeAction wf = 
                        new BookmarkCalculatorInvokeAction();
#else
                    BookmarkCalculatorAction wf = new BookmarkCalculatorAction();
#endif
                    wf.Action = new ActivityAction<String, String>
                    {
                        Argument1 = new DelegateInArgument<String>(),
                        Argument2 = new DelegateInArgument<String>(),
                        Handler = new InvokeMethod
                        {
                            TargetType = typeof(Program),
                            MethodName = "ReceivedNotification",
                            Parameters =
                            {
                                new InArgument<String>(
                                    ac => wf.Action.Argument1.Get(ac)),
                                new InArgument<String>(
                                    ac => wf.Action.Argument2.Get(ac))
                            }
                        }
                    };

                    wfApp = new WorkflowApplication(wf);
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

                    wfApp.Run();
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

        public static void ReceivedNotification(
            String prompt, String bookmarkName)
        {
            Action<String, String> asyncWork = (msg, bm) =>
            {
                Console.WriteLine(msg);
                String expression = Console.ReadLine();

                if ((String.IsNullOrEmpty(expression)) ||
                    (!String.IsNullOrEmpty(expression) &&
                     expression.Trim().ToLower() == "quit"))
                {
                    wfApp.Cancel();
                    Console.WriteLine("Exiting program");
                    isRunning = false;
                }
                else if (IsBookmarkValid(wfApp, bm))
                {
                    wfApp.ResumeBookmark(bm, expression);
                }
            };

            //ar = IAsyncResult
            asyncWork.BeginInvoke(prompt, bookmarkName,
                ar => { ((Action<String, String>)ar.AsyncState).EndInvoke(ar); },
                asyncWork);
        }
    }
}
