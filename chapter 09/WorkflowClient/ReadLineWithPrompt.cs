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
using System.Reflection;

namespace WorkflowClient
{
    public class ReadLineWithPrompt<TResult> : AsyncCodeActivity<TResult>
    {
        public InArgument<string> Prompt { get; set; }

        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context,
            AsyncCallback callback, object state)
        {
            Console.WriteLine(Prompt.Get(context));
            Func<TResult> getInput = () => { return WaitForConsoleInput(); };
            context.UserState = getInput;
            return getInput.BeginInvoke(callback, state);
        }

        private TResult WaitForConsoleInput()
        {
            TResult value = default(TResult);
            String stringInput = Console.ReadLine();

            if (typeof(TResult) == typeof(String))
            {
                value = (TResult)(Object)(stringInput);
            }
            else
            {
                MethodInfo parse = typeof(TResult).GetMethod(
                    "Parse", BindingFlags.Static | BindingFlags.Public,
                    null, new Type[] { typeof(String) }, null);
                if (parse != null)
                {
                    try
                    {
                        value = (TResult)parse.Invoke(
                            null, new Object[] { stringInput });
                    }
                    catch
                    {
                        //ignore any parsing errors

                        //Console.WriteLine(
                        //    "Unable to parse value {0} into type {1}",
                        //    stringInput, typeof(TResult).Name);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "Parse method not supported");
                }
            }

            return value;
        }

        protected override TResult EndExecute(
            AsyncCodeActivityContext context, IAsyncResult ar)
        {
            return ((Func<TResult>)context.UserState).EndInvoke(ar);
        }
    }
}
