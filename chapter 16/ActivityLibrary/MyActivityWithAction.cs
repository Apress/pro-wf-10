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
using System.ComponentModel;

namespace ActivityLibrary
{
    [Designer(typeof(ActivityLibrary.Design.MyActivityWithActionDesigner))]
    public class MyActivityWithAction : NativeActivity
    {
        [Browsable(false)]
        public ActivityAction<String> Notify { get; set; }
        [RequiredArgument]
        public InArgument<List<String>> Strings { get; set; }

        private Variable<Int32> NextIndex = new Variable<Int32>();

        public MyActivityWithAction()
        {
            Notify = new ActivityAction<String>
                {
                    Argument = new DelegateInArgument<String>
                    {
                        Name = "message"
                    }
                };
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            Console.WriteLine("CacheMetadata");
            metadata.AddDelegate(Notify);
            metadata.AddArgument(new RuntimeArgument(
                "Strings", typeof(List<String>), ArgumentDirection.In));
            metadata.AddImplementationVariable(NextIndex);
        }

        protected override void Execute(NativeActivityContext context)
        {
            if (Notify != null)
            {
                ScheduleNextItem(context);
            }
        }

        private void OnComplete(NativeActivityContext context,
            ActivityInstance completedInstance)
        {
            Console.WriteLine("OnComplete:  State:{0}, IsCompleted:{1}",
                completedInstance.State, completedInstance.IsCompleted);

            if (!context.IsCancellationRequested)
            {
                ScheduleNextItem(context);
            }
        }

        private void ScheduleNextItem(NativeActivityContext context)
        {
            List<String> collection = Strings.Get(context);
            Int32 index = NextIndex.Get(context);
            if (index < collection.Count)
            {
                Console.WriteLine(
                    "ScheduleNextItem ScheduleAction: Handler: {0}, Value: {1}",
                    Notify.Handler.DisplayName, collection[index]);
                context.ScheduleAction<String>(
                    Notify, collection[index], OnComplete);
                NextIndex.Set(context, index + 1);
            }
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
