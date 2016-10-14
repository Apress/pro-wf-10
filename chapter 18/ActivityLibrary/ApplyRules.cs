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
using System.IO;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;

namespace ActivityLibrary
{
    public class ApplyRules<T> : AsyncCodeActivity
    {
        public InOutArgument<T> Target { get; set; }
        public InArgument<String> RulesFilePath { get; set; }
        public InArgument<String> RuleSetName { get; set; }
        public OutArgument<ValidationErrorCollection> Errors { get; set; }

        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            T target = Target.Get(context);
            String path = RulesFilePath.Get(context);
            String setName = RuleSetName.Get(context);

            Func<T, String, String, ValidationErrorCollection> asyncWork =
                ApplyRuleSet;
            context.UserState = asyncWork;
            return asyncWork.BeginInvoke(target, path, setName, callback, state);
        }

        protected override void EndExecute(
            AsyncCodeActivityContext context, IAsyncResult result)
        {
            ValidationErrorCollection errors =
                ((Func<T, String, String, ValidationErrorCollection>)
                    context.UserState).EndInvoke(result);
            if (errors != null)
            {
                Errors.Set(context, errors);
            }
        }

        private ValidationErrorCollection ApplyRuleSet(T target,
            String path, String setName)
        {
            ValidationErrorCollection errors = null;

            WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();

            RuleSet rs = null;
            if (File.Exists(path))
            {
                using (XmlTextReader reader = new XmlTextReader(path))
                {
                    RuleDefinitions rules =
                        serializer.Deserialize(reader) as RuleDefinitions;
                    if (rules != null && rules.RuleSets.Contains(setName))
                    {
                        rs = rules.RuleSets[setName];
                    }
                }
            }

            if (rs == null)
            {
                throw new ArgumentException(String.Format(
                    "Unable to retrieve RuleSet {0} from {1}", setName, path));
            }

            RuleValidation val = new RuleValidation(target.GetType(), null);
            if (!rs.Validate(val))
            {
                errors = val.Errors;
                return errors;
            }

            RuleEngine rulesEngine = new RuleEngine(rs, val);
            rulesEngine.Execute(target);
            return errors;
        }
    }
}
