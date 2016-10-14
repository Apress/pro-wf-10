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
    [Designer(typeof(ActivityDesignerLibrary.MyActivityWithActionDesigner))]
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
            //base.CacheMetadata(metadata);
            metadata.AddDelegate(Notify);
            metadata.AddArgument(new RuntimeArgument(
                "Strings", typeof(List<String>), ArgumentDirection.In));
            metadata.AddImplementationVariable(NextIndex);
        }

        protected override void Execute(NativeActivityContext context)
        {
            if (Notify != null)
            {
                List<String> collection = Strings.Get(context);
                if (collection != null && collection.Count > 0)
                {
                    NextIndex.Set(context, NextIndex.Get(context) + 1);
                    context.ScheduleAction<String>(Notify, collection[0],
                        ActionCompletion);
                }
            }
        }

        private void ActionCompletion(NativeActivityContext context,
            ActivityInstance completedInstance)
        {
            List<String> collection = Strings.Get(context);
            Int32 index = NextIndex.Get(context);
            if (index < collection.Count)
            {
                NextIndex.Set(context, index + 1);
                context.ScheduleAction<String>(Notify, collection[index],
                    ActionCompletion);
            }
        }
    }
}
