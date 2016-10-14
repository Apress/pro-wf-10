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

namespace CalculatorEnum
{
    public enum CalculatorOperation
    {
        Unknown,
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public sealed class ParseCalculatorEnumArgs : CodeActivity
    {
        [RequiredArgument]
        public InArgument<String> Expression { get; set; }
        public OutArgument<Double> FirstNumber { get; set; }
        public OutArgument<Double> SecondNumber { get; set; }
        public OutArgument<CalculatorOperation> Operation { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            FirstNumber.Set(context, 0);
            SecondNumber.Set(context, 0);
            Operation.Set(context, CalculatorOperation.Unknown);

            String line = Expression.Get(context);
            if (!String.IsNullOrEmpty(line))
            {
                String[] arguments = line.Split(' ');
                if (arguments.Length == 3)
                {
                    Double number = 0;
                    if (Double.TryParse(arguments[0], out number))
                    {
                        FirstNumber.Set(context, number);
                    }

                    CalculatorOperation op = CalculatorOperation.Unknown;
                    Enum.TryParse<CalculatorOperation>(arguments[1], out op);
                    Operation.Set(context, op);

                    if (Double.TryParse(arguments[2], out number))
                    {
                        SecondNumber.Set(context, number);
                    }
                }
            }
        }
    }
}
