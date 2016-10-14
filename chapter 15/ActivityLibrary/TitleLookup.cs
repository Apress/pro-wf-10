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
    public sealed class TitleLookup : CodeActivity
    {
        [RequiredArgument]
        [OverloadGroup("ByKeyword")]
        [OverloadGroup("ByTitle")]
        public InArgument<String> Category { get; set; }

        [RequiredArgument]
        [OverloadGroup("ByKeyword")]
        public InArgument<String> Keyword { get; set; }

        [RequiredArgument]
        [OverloadGroup("ByTitle")]
        public InArgument<String> Author { get; set; }

        [RequiredArgument]
        [OverloadGroup("ByTitle")]
        public InArgument<String> Title { get; set; }

        [RequiredArgument]
        [OverloadGroup("ByISBN")]
        public InArgument<String> ISBN { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
