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
using System.ComponentModel;

namespace ActivityLibrary
{
    [Designer(typeof(ActivityLibrary.Design.MyWhileDesigner))]
    public class MyWhile : NativeActivity
    {
        [Browsable(false)]
        public Activity Body { get; set; }
        [RequiredArgument]
        public Activity<Boolean> Condition { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            Console.WriteLine("CacheMetadata");
            metadata.AddChild(Body);
            metadata.AddChild(Condition);
        }

        protected override void Execute(NativeActivityContext context)
        {
            if (Condition != null)
            {
                Console.WriteLine("Execute Scheduled Condition");
                context.ScheduleActivity<Boolean>(Condition, OnConditionComplete);
            }
        }

        private void OnConditionComplete(NativeActivityContext context,
            ActivityInstance completedInstance, Boolean result)
        {
            Console.WriteLine(
                "OnConditionComplete: State:{0}, IsCompleted:{1}: Result:{2}",
                completedInstance.State, completedInstance.IsCompleted, result);
            if (!context.IsCancellationRequested)
            {
                if (result && (Body != null))
                {
                    Console.WriteLine("OnConditionComplete Scheduled Body");
                    context.ScheduleActivity(Body, OnComplete, OnFaulted);
                }
            }
        }

        private void OnComplete(NativeActivityContext context,
            ActivityInstance completedInstance)
        {
            Console.WriteLine("OnComplete: State:{0}, IsCompleted:{1}",
                completedInstance.State, completedInstance.IsCompleted);
            if (!context.IsCancellationRequested)
            {
                if (Condition != null)
                {
                    Console.WriteLine("OnComplete Scheduled Condition");
                    context.ScheduleActivity<Boolean>(
                        Condition, OnConditionComplete, OnFaulted);
                }
            }
        }

        protected virtual void OnFaulted(NativeActivityFaultContext faultContext,
            Exception propagatedException, ActivityInstance propagatedFrom)
        {
            Console.WriteLine("OnFaulted: {0}", propagatedException.Message);
        }

        protected override void Cancel(NativeActivityContext context)
        {
            Console.WriteLine("Cancel");
            if (context.IsCancellationRequested)
            {
                Console.WriteLine("IsCancellationRequested");
                context.CancelChildren();
            }
        }

        protected override void Abort(NativeActivityAbortContext context)
        {
            base.Abort(context);
            Console.WriteLine("Abort Reason: {0}", context.Reason.Message);
        }

    }
}
