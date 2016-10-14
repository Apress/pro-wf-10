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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ActivityLibrary;

namespace WpfHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkflowManager _manager = new WorkflowManager();
        private Random _rnd = new Random(Environment.TickCount);
        private List<InstanceInfo> _instances = new List<InstanceInfo>();

        public MainWindow()
        {
            InitializeComponent();

            _manager.Started = Started;
            _manager.Completed = Completed;
            _manager.Idle = Idle;
            _manager.Incomplete = Incomplete;
            dataGrid1.ItemsSource = _instances;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Int32 testNumber = _rnd.Next(9999);

            //start a workflow instance
            Guid id = _manager.Run<HostingDemoWorkflow>(
                new Dictionary<String, Object>
                {
                    {"ArgNumberToEcho", testNumber},
                });
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _instances.Clear();
            dataGrid1.Items.Refresh();
        }

        private void Started(Guid id, IDictionary<string, object> parameters)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action<InstanceInfo>(UpdateDisplay), new InstanceInfo
            {
                Id = id,
                TestNumber = (Int32)parameters["ArgNumberToEcho"],
                Result = String.Empty,
                Status = "Started"
            });
        }

        private void Completed(Guid id, IDictionary<string, object> outputs)
        {
            //UpdateDisplay(new InstanceInfo
            //{
            //    Id = id,
            //    Result = outputs["Result"] as String,
            //    Status = "Completed"
            //});

            Application.Current.Dispatcher.BeginInvoke(
                new Action<InstanceInfo>(UpdateDisplay), new InstanceInfo
                {
                    Id = id,
                    Result = outputs["Result"] as String,
                    Status = "Completed"
                });
        }

        private void Idle(Guid id)
        {
            //UpdateDisplay(new InstanceInfo
            //{
            //    Id = id,
            //    Status = "Idle"
            //});

            Application.Current.Dispatcher.BeginInvoke(
                new Action<InstanceInfo>(UpdateDisplay), new InstanceInfo
                {
                    Id = id,
                    Status = "Idle"
                });
        }

        private void Incomplete(Guid id, String reason, String message)
        {
            //UpdateDisplay(new InstanceInfo
            //{
            //    Id = id,
            //    Result = message,
            //    Status = reason
            //});

            Application.Current.Dispatcher.BeginInvoke(
                new Action<InstanceInfo>(UpdateDisplay), new InstanceInfo
                {
                    Id = id,
                    Result = message,
                    Status = reason
                });
        }

        private void UpdateDisplay(InstanceInfo info)
        {
            InstanceInfo currInfo = null;
            currInfo =
               (from i in _instances
                where i.Id == info.Id
                select i).SingleOrDefault();
            if (currInfo != null)
            {
                currInfo.Status = info.Status;
                currInfo.Result = info.Result;
            }
            else
            {
                _instances.Add(info);
                currInfo = info;
            }

            dataGrid1.Items.Refresh();
            dataGrid1.ScrollIntoView(currInfo);
        }

        private void Window_Closing(
            object sender, System.ComponentModel.CancelEventArgs e)
        {
            Int32 count = _manager.GetActiveCount();
            if (count != 0)
            {
                if (MessageBox.Show(String.Format(
                    "{0} workflows are still executing.  Continue?", count),
                    "Workflow Executing",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                        != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
