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
using System.Activities.Presentation.Validation;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DesignerHost
{
    public class ValidationErrorService : IValidationErrorService
    {
        private ListBox lb;

        public ValidationErrorService(ListBox listBox)
        {
            lb = listBox;
        }

        public void ShowValidationErrors(IList<ValidationErrorInfo> errors)
        {
            lb.Items.Clear();
            foreach (ValidationErrorInfo error in errors)
            {
                if (String.IsNullOrEmpty(error.PropertyName))
                {
                    lb.Items.Add(error.Message);
                }
                else
                {
                    lb.Items.Add(String.Format("{0}: {1}",
                        error.PropertyName,
                        error.Message));
                }
            }
        }
    }
}
