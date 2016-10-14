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
    [Designer(typeof(ActivityLibrary.Design.MySimpleParentDesigner))]
    public class MySimpleParent : NativeActivity
    {
        [Browsable(false)]
        public Activity Body { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            Console.WriteLine("CacheMetadata");
            metadata.AddChild(Body);
        }

        protected override void Execute(NativeActivityContext context)
        {
            Console.WriteLine("Execute Scheduled Body");
            ActivityInstance instance =
                context.ScheduleActivity(Body, OnComplete);
            Console.WriteLine("Execute: ID: {0}, State: {1}",
                instance.Id, instance.State);
        }

        private void OnComplete(NativeActivityContext context,
            ActivityInstance completedInstance)
        {
            Console.WriteLine("OnComplete: State:{0}, IsCompleted:{1}",
                completedInstance.State, completedInstance.IsCompleted);
        }

        protected override void Cancel(NativeActivityContext context)
        {
            //not required since this is the default behavior. but should
            //get in the habit of adding this code anytime you 
            //schedule execution of an activity

            Console.WriteLine("Cancel");
            context.CancelChildren();
        }

        protected override void Abort(NativeActivityAbortContext context)
        {
            base.Abort(context);
            Console.WriteLine("Abort: Reason: {0}", context.Reason.Message);
        }
    }
}
