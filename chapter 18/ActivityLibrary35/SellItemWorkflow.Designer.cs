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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace ActivityLibrary35
{
	partial class SellItemWorkflow
	{
		#region Designer generated code
		
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
		private void InitializeComponent()
		{
            this.CanModifyActivities = true;
            System.Workflow.Activities.Rules.RuleSetReference rulesetreference1 = new System.Workflow.Activities.Rules.RuleSetReference();
            this.policyActivity1 = new System.Workflow.Activities.PolicyActivity();
            // 
            // policyActivity1
            // 
            this.policyActivity1.Name = "policyActivity1";
            rulesetreference1.RuleSetName = "CalculateItemTotals";
            this.policyActivity1.RuleSetReference = rulesetreference1;
            // 
            // SellItemWorkflow
            // 
            this.Activities.Add(this.policyActivity1);
            this.Name = "SellItemWorkflow";
            this.CanModifyActivities = false;

		}

		#endregion

        private PolicyActivity policyActivity1;











    }
}
