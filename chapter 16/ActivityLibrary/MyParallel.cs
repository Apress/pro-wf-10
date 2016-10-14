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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ActivityLibrary
{
    [Designer(typeof(ActivityLibrary.Design.MySequenceDesigner))]
    public class MyParallel : NativeActivity
    {
        [Browsable(false)]
        public Collection<Activity> Activities { get; set; }
        [RequiredArgument]
        public Activity<Boolean> Condition { get; set; }

        private Variable<Dictionary<String, ActivityInstance>> scheduledChildren =
            new Variable<Dictionary<String, ActivityInstance>>();

        public MyParallel()
        {
            Activities = new Collection<Activity>();
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            Console.WriteLine("CacheMetadata");
            if (Activities != null && Activities.Count > 0)
            {
                foreach (Activity activity in Activities)
                {
                    metadata.AddChild(activity);
                }
            }

            metadata.AddChild(Condition);
            metadata.AddImplementationVariable(scheduledChildren);
        }

        protected override void Execute(NativeActivityContext context)
        {
            if (Condition != null)
            {
                Console.WriteLine("Execute Scheduled Condition");

                scheduledChildren.Set(
                    context, new Dictionary<String, ActivityInstance>());

                context.ScheduleActivity<Boolean>(Condition, OnConditionComplete);
            }
        }

        private void OnConditionComplete(NativeActivityContext context,
            ActivityInstance completedInstance, Boolean result)
        {
            Console.WriteLine(
                "OnConditionComplete:  State:{0}, IsCompleted:{1}: Result:{2}",
                completedInstance.State, completedInstance.IsCompleted, result);
            if (!context.IsCancellationRequested)
            {
                if (result)
                {
                    if (Activities != null && Activities.Count > 0)
                    {
                        for (Int32 i = Activities.Count - 1; i >= 0; i--)
                        {
                            Console.WriteLine(
                                "OnConditionComplete Scheduled activity: {0}",
                                    Activities[i].DisplayName);
                            ActivityInstance instance = context.ScheduleActivity(
                                Activities[i], OnComplete, OnFaulted);
                            scheduledChildren.Get(context).Add(
                                instance.Id, instance);
                        }
                    }
                }
            }
        }

        private void OnComplete(NativeActivityContext context,
            ActivityInstance completedInstance)
        {
            Console.WriteLine(
                "OnComplete: Activity: {0}, State:{0}, IsCompleted:{1}",
                completedInstance.Activity.DisplayName,
                completedInstance.State, completedInstance.IsCompleted);

            scheduledChildren.Get(context).Remove(completedInstance.Id);
        }

        private void OnFaulted(NativeActivityFaultContext faultContext,
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

                foreach (ActivityInstance instance in
                    scheduledChildren.Get(context).Values)
                {
                    Console.WriteLine(
                        "Cancel scheduled child: {0}",
                            instance.Activity.DisplayName);
                    context.CancelChild(instance);
                }
            }
        }

        protected override void Abort(NativeActivityAbortContext context)
        {
            base.Abort(context);
            Console.WriteLine("Abort Reason: {0}", context.Reason.Message);
        }
    }
}
