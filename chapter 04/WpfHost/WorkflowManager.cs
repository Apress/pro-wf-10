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
using System.Collections.Generic;

namespace WpfHost
{
    public class WorkflowManager
    {
        private Dictionary<Guid, WorkflowApplication> _wfApps
            = new Dictionary<Guid, WorkflowApplication>();

        public Guid Run<T>(IDictionary<String, Object> parameters)
            where T : Activity, new()
        {
            Console.WriteLine("Host: About to run workflow - Thread:{0}",
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            Guid id = Guid.Empty;

            T activity = new T();
            WorkflowApplication wfApp = null;
            if (parameters != null)
            {
                wfApp = new WorkflowApplication(activity, parameters);
            }
            else
            {
                wfApp = new WorkflowApplication(activity);
            }
            id = wfApp.Id;

            _wfApps.Add(wfApp.Id, wfApp);

            wfApp.Completed = AppCompleted;
            wfApp.Idle = AppIdle;
            wfApp.Aborted = AppAborted;
            wfApp.OnUnhandledException = AppException;

            if (Started != null)
            {
                Started(id, parameters);
            }

            //Add this line to schedule and execute all workflows on
            //the WPF UI thread.
            //wfApp.SynchronizationContext = 
            //    System.Threading.SynchronizationContext.Current;

            wfApp.Run();

            //System.Threading.Thread.Sleep(500);
            //wfApp.Terminate("Terminated");
            //wfApp.Cancel();
            //wfApp.Abort("Aborted reason");

            return id;
        }

        public Action<Guid, IDictionary<string, object>> Started { get; set; }
        public Action<Guid, IDictionary<string, object>> Completed { get; set; }
        public Action<Guid> Idle { get; set; }
        public Action<Guid, String, String> Incomplete { get; set; }

        public Int32 GetActiveCount()
        {
            lock (_wfApps)
            {
                return _wfApps.Count;
            }
        }

        private void AppCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            switch (e.CompletionState)
            {
                case ActivityInstanceState.Closed:
                    if (Completed != null)
                    {
                        Completed(e.InstanceId, e.Outputs);
                    }
                    RemoveInstance(e.InstanceId);
                    break;
                case ActivityInstanceState.Canceled:
                    if (Incomplete != null)
                    {
                        Incomplete(e.InstanceId, "Canceled",
                            String.Empty);
                    }
                    RemoveInstance(e.InstanceId);
                    break;
                case ActivityInstanceState.Faulted:
                    if (Incomplete != null)
                    {
                        Incomplete(e.InstanceId, "Faulted",
                            e.TerminationException.Message);
                    }
                    RemoveInstance(e.InstanceId);
                    break;
                default:
                    break;
            }
        }

        private void AppIdle(WorkflowApplicationIdleEventArgs e)
        {
            if (Idle != null)
            {
                Idle(e.InstanceId);
            }
        }

        private void AppAborted(WorkflowApplicationAbortedEventArgs e)
        {
            if (Incomplete != null)
            {
                Incomplete(e.InstanceId, "Aborted", e.Reason.Message);
            }
            RemoveInstance(e.InstanceId);
        }

        private UnhandledExceptionAction AppException(
            WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            if (Incomplete != null)
            {
                Incomplete(e.InstanceId, "Exception",
                    e.UnhandledException.Message);
            }
            RemoveInstance(e.InstanceId);
            return UnhandledExceptionAction.Cancel;
        }

        private void RemoveInstance(Guid id)
        {
            lock (_wfApps)
            {
                if (_wfApps.ContainsKey(id))
                {
                    _wfApps[id].Completed = null;
                    _wfApps[id].Idle = null;
                    _wfApps[id].Aborted = null;
                    _wfApps[id].OnUnhandledException = null;
                    _wfApps.Remove(id);
                }
            }
        }
    }
}
