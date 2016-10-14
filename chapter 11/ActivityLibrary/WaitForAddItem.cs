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
using System.Activities;

namespace ActivityLibrary
{
    public sealed class WaitForAddItem : NativeActivity<Item>
    {
        protected override void Execute(NativeActivityContext context)
        {
            context.CreateBookmark("AddItem", Resumed);
        }

        private void Resumed(NativeActivityContext context,
            Bookmark bookmark, object value)
        {
            if (value is Item)
            {
                Result.Set(context, value as Item);
            }
        }

        protected override bool CanInduceIdle
        {
            get { return true; }
        }
    }
}
