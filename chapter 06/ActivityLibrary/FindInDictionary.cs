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
namespace ActivityLibrary
{
    using System;
    using System.Activities;
    using System.Collections.Generic;

    public class FindInDictionary<TKey, TValue> : CodeActivity<Boolean>
    {
        [RequiredArgument]
        public InArgument<IDictionary<TKey, TValue>> Dictionary { get; set; }
        [RequiredArgument]
        public InArgument<TKey> Key { get; set; }
        public OutArgument<TValue> FoundItem { get; set; }

        protected override Boolean Execute(CodeActivityContext context)
        {
            Boolean result = false;
            FoundItem.Set(context, default(TValue));
            IDictionary<TKey, TValue> dictionary = Dictionary.Get(context);
            TKey key = Key.Get(context);
            if (dictionary != null)
            {
                if (dictionary.ContainsKey(key))
                {
                    FoundItem.Set(context, dictionary[key]);
                    result = true;
                }
            }
            return result;
        }
    }
}
