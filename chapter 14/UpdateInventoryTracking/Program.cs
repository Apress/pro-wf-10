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
namespace UpdateInventoryTracking
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using ActivityLibrary;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nExecuting DefaultETWTracking");
            DefaultETWTracking();
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting ETWTrackingWithProfile 1");
            ETWTrackingWithProfile(1);
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting ETWTrackingWithProfile 2");
            ETWTrackingWithProfile(2);
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting ETWTrackingWithProfile 3");
            ETWTrackingWithProfile(3);
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting ETWTrackingWithProfile 4");
            ETWTrackingWithProfile(4);
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting ETWTrackingWithProfile 5");
            ETWTrackingWithProfile(5);
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting ETWTrackingWithProfile 6");
            ETWTrackingWithProfile(6);
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting CustomTrackingWithProfile 6");
            CustomTrackingWithProfile(6);
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting CustomTrackingWithProfileAppConfig");
            CustomTrackingWithProfileAppConfig();
            System.Threading.Thread.Sleep(3000);

            Console.WriteLine("\nExecuting CustomEventTracking");
            CustomEventTracking();
            System.Threading.Thread.Sleep(3000);
        }

        private static void DefaultETWTracking()
        {
            UpdateInventory wf = new UpdateInventory();
            wf.ArgSalesOrderId = 43687;
            WorkflowInvoker instance = new WorkflowInvoker(wf);
            instance.Extensions.Add(new EtwTrackingParticipant());
            instance.Invoke();
        }

        private static void ETWTrackingWithProfile(Int32 profileNumber)
        {
            UpdateInventory wf = new UpdateInventory();
            wf.ArgSalesOrderId = 43687;
            WorkflowInvoker instance = new WorkflowInvoker(wf);

            EtwTrackingParticipant tp = new EtwTrackingParticipant();

            switch (profileNumber)
            {
                case 1:
                    //select workflow instance states
                    tp.TrackingProfile = new TrackingProfile
                    {
                        Name = "MyTrackingProfile",
                        Queries =
                        {
                            new WorkflowInstanceQuery
                            {
                                States = 
                                {
                                    WorkflowInstanceStates.Started,
                                    WorkflowInstanceStates.Completed,
                                }
                            }
                        }
                    };
                    break;
                case 2:
                    //all workflow instance states
                    tp.TrackingProfile = new TrackingProfile
                    {
                        Name = "MyTrackingProfile",
                        Queries =
                        {
                            new WorkflowInstanceQuery
                            {
                                States = {"*"}
                            }
                        }
                    };
                    break;
                case 3:
                    //selected activity states
                    tp.TrackingProfile = new TrackingProfile
                    {
                        Name = "MyTrackingProfile",
                        Queries =
                        {
                            new WorkflowInstanceQuery
                            {
                                States = {"*"}
                            },
                            new ActivityStateQuery
                            {
                                States = 
                                {
                                    ActivityStates.Executing,
                                    ActivityStates.Closed
                                }
                            }
                        }
                    };
                    break;
                case 4:
                    //Activity states for selected activities with
                    //argument extraction
                    tp.TrackingProfile = new TrackingProfile
                    {
                        Name = "MyTrackingProfile",
                        Queries =
                        {
                            new WorkflowInstanceQuery
                            {
                                States = 
                                {
                                    WorkflowInstanceStates.Started,
                                    WorkflowInstanceStates.Completed,
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "*" }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "InsertTranHistory",
                                States = {"*"}
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "SalesDetail" },
                                QueryAnnotations = 
                                {
                                    {"Threading Model", "Asynchronous update"}
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Closed }
                            }
                        }
                    };
                    break;
                case 5:
                    //Activity states for selected activities with
                    //argument extraction and selected activity scheduled query
                    tp.TrackingProfile = new TrackingProfile
                    {
                        Name = "MyTrackingProfile",
                        Queries =
                        {
                            new WorkflowInstanceQuery
                            {
                                States = 
                                {
                                    WorkflowInstanceStates.Started,
                                    WorkflowInstanceStates.Completed,
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "*" }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "InsertTranHistory",
                                States = {"*"}
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "SalesDetail" },
                                QueryAnnotations = 
                                {
                                    {"Threading Model", "Asynchronous update"}
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Closed }
                            },
                            new ActivityScheduledQuery
                            {
                                ChildActivityName = "UpdateProductInventory"
                            }
                        }
                    };
                    break;
                case 6:
                    //Activity states for selected activities with
                    //argument extraction and selected activity scheduled query
                    //also adds Custom Tracking Records
                    tp.TrackingProfile = new TrackingProfile
                    {
                        Name = "MyTrackingProfile",
                        Queries =
                        {
                            new WorkflowInstanceQuery
                            {
                                States = 
                                {
                                    WorkflowInstanceStates.Started,
                                    WorkflowInstanceStates.Completed,
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "*" }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "InsertTranHistory",
                                States = {"*"}
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "SalesDetail" },
                                QueryAnnotations = 
                                {
                                    {"Threading Model", "Asynchronous update"}
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Closed }
                            },
                            new ActivityScheduledQuery
                            {
                                ChildActivityName = "UpdateProductInventory"
                            },
                            new CustomTrackingQuery
                            {
                                ActivityName = "*",
                                Name = "*"
                            }
                        }
                    };
                    break;
                default:
                    break;
            }

            instance.Extensions.Add(tp);
            instance.Invoke();
        }

        private static void CustomTrackingWithProfile(Int32 profileNumber)
        {
            FileTrackingParticipant tp = new FileTrackingParticipant();

            UpdateInventory wf = new UpdateInventory();
            wf.ArgSalesOrderId = 43687;
            WorkflowInvoker instance = new WorkflowInvoker(wf);

            switch (profileNumber)
            {
                case 6:
                    //Activity states for selected activities with
                    //argument extraction and selected activity scheduled query
                    //also adds Custom Tracking Records
                    tp.TrackingProfile = new TrackingProfile
                    {
                        Name = "MyTrackingProfile",
                        Queries =
                        {
                            new WorkflowInstanceQuery
                            {
                                States = 
                                {
                                    WorkflowInstanceStates.Started,
                                    WorkflowInstanceStates.Completed,
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "*" }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "InsertTranHistory",
                                States = {"*"}
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Executing },
                                Arguments = { "SalesDetail" },
                                QueryAnnotations = 
                                {
                                    {"Threading Model", "Asynchronous update"}
                                }
                            },
                            new ActivityStateQuery
                            {
                                ActivityName = "UpdateProductInventory",
                                States = { ActivityStates.Closed }
                            },
                            new ActivityScheduledQuery
                            {
                                ChildActivityName = "UpdateProductInventory"
                            },
                            new CustomTrackingQuery
                            {
                                ActivityName = "*",
                                Name = "*"
                            }
                        }
                    };
                    break;
                default:
                    break;
            }

            instance.Extensions.Add(tp);
            instance.Invoke();

            tp.Stop();
        }

        private static void CustomTrackingWithProfileAppConfig()
        {
            TrackingProfileLoader config =
                new TrackingProfileLoader("MyTrackingProfile");

            FileTrackingParticipant tp = new FileTrackingParticipant();
            tp.TrackingProfile = config.Profile;

            UpdateInventory wf = new UpdateInventory();
            wf.ArgSalesOrderId = 43687;
            WorkflowInvoker instance = new WorkflowInvoker(wf);

            instance.Extensions.Add(tp);
            instance.Invoke();

            tp.Stop();
        }

        private static void CustomEventTracking()
        {
            EventTrackingParticipant tp =
                new EventTrackingParticipant();
            tp.Received = tr =>
                Console.WriteLine("{0:D2} {1:HH:mm:ss.ffffff} {2}",
                    tr.RecordNumber,
                    tr.EventTime,
                    tr.GetType().Name);

            UpdateInventory wf = new UpdateInventory();
            wf.ArgSalesOrderId = 43687;
            WorkflowInvoker instance = new WorkflowInvoker(wf);

            instance.Extensions.Add(tp);
            instance.Invoke();
        }
    }
}
