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
using System.IO;
using ActivityLibrary;

namespace WebInvokerHost
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Button1_Click(object sender, EventArgs e)
        {
            Int32 testNumber = 0;
            if (Int32.TryParse(TextBox1.Text, out testNumber))
            {
                StringWriter writer = new StringWriter();
                try
                {
                    writer.WriteLine("Host: About to run workflow - Thread:{0}",
                        System.Threading.Thread.CurrentThread.ManagedThreadId);

                    HostingDemoWorkflow wf = new HostingDemoWorkflow();
                    IDictionary<String, Object> output =
                        WorkflowInvoker.Invoke(wf, new Dictionary<String, Object>
                    {
                        {"ArgNumberToEcho", 1001},
                        {"ArgTextWriter", writer},
                    });

                    Label2.Text = (String)output["Result"];

                    writer.WriteLine(
                        "Host: Workflow completed - Thread:{0} - {1}",
                        System.Threading.Thread.CurrentThread.ManagedThreadId,
                        output["Result"]);
                }
                catch (Exception exception)
                {
                    writer.WriteLine("Host: Workflow exception:{0}:{1}",
                        exception.GetType(), exception.Message);
                }

                //dump the contents of the writer
                TextBox2.Text = writer.ToString();
            }
        }
    }
}