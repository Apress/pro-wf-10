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
using System.Workflow.ComponentModel;

namespace ActivityLibrary35
{
    public partial class LookupItem35 : Activity
    {
        public static DependencyProperty ItemIdProperty =
            DependencyProperty.Register(
                "ItemId", typeof(Int32), typeof(LookupItem35));

        [Description("ItemId")]
        [Category("LookupItem35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Int32 ItemId
        {
            get
            {
                return ((Int32)(base.GetValue(LookupItem35.ItemIdProperty)));
            }
            set
            {
                base.SetValue(LookupItem35.ItemIdProperty, value);
            }
        }

        public static DependencyProperty WeightProperty =
            DependencyProperty.Register(
                "Weight", typeof(Int32), typeof(LookupItem35));

        [Description("Weight")]
        [Category("LookupItem35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Int32 Weight
        {
            get
            {
                return ((Int32)(base.GetValue(LookupItem35.WeightProperty)));
            }
            set
            {
                base.SetValue(LookupItem35.WeightProperty, value);
            }
        }

        public static DependencyProperty PriceProperty =
            DependencyProperty.Register(
                "Price", typeof(Decimal), typeof(LookupItem35));

        [Description("Price")]
        [Category("LookupItem35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Decimal Price
        {
            get
            {
                return ((Decimal)(base.GetValue(LookupItem35.PriceProperty)));
            }
            set
            {
                base.SetValue(LookupItem35.PriceProperty, value);
            }
        }

        public LookupItem35()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(
            ActivityExecutionContext executionContext)
        {
            Weight = ItemId * 5;
            Price = (Decimal)ItemId * 3.95M;
            return ActivityExecutionStatus.Closed;
        }
    }
}
