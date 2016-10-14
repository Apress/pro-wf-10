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
using System.Activities.Validation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;

namespace ActivityLibrary
{
    [Designer(typeof(ActivityDesignerLibrary.MySequenceDesigner))]
    [ContentProperty("Activities")]
    public class MySequenceWithValidation : NativeActivity
    {
        [Browsable(false)]
        public Collection<Activity> Activities { get; set; }
        public Activity<Boolean> Condition { get; set; }
        [Browsable(false)]
        public Collection<Variable> Variables { get; set; }

        public MySequenceWithValidation()
        {
            Activities = new Collection<Activity>();
            Variables = new Collection<Variable>();
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (Activities.Count == 0)
            {
                metadata.AddValidationError(
                    "At least one child activity must be added");

                //metadata.AddValidationError(
                //    new ValidationError(
                //        "At least one child activity must be added",
                //        true, "Activities"));
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
