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

namespace ActivityLibrary
{
    public sealed class NotifyHost : CodeActivity
    {
        public InArgument<string> Message { get; set; }
        public InArgument<string> BookmarkName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IHostNotification extension =
                context.GetExtension<IHostNotification>();
            if (extension != null)
            {
                extension.Notify(Message.Get(context),
                    BookmarkName.Get(context));
            }
        }
    }
}
