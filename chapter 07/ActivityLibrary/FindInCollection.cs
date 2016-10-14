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

namespace ActivityLibrary
{
    public class FindInCollection<T> : CodeActivity<Boolean>
    {
        [RequiredArgument]
        public InArgument<ICollection<T>> Collection { get; set; }
        [RequiredArgument]
        public InArgument<T> Item { get; set; }
        public OutArgument<T> FoundItem { get; set; }

        protected override Boolean Execute(CodeActivityContext context)
        {
            Boolean result = false;
            FoundItem.Set(context, default(T));
            ICollection<T> collection = Collection.Get(context);
            T item = Item.Get(context);

            if (collection != null)
            {
                foreach (T entry in collection)
                {
                    if (entry is IEquatable<T>)
                    {
                        if (((IEquatable<T>)entry).Equals(item))
                        {
                            FoundItem.Set(context, entry);
                            result = true;
                            break;
                        }
                    }
                    else if (entry.Equals(item))
                    {
                        FoundItem.Set(context, entry);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
