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
using System.Workflow.Activities;
using System.Workflow.ComponentModel;

namespace ActivityLibrary35
{
    public partial class CalcItemTotal35 : SequentialWorkflowActivity
    {
        public static DependencyProperty ItemIdProperty =
            DependencyProperty.Register(
                "ItemId", typeof(Int32), typeof(CalcItemTotal35));

        [Description("ItemId")]
        [Category("CalcItemTotal35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Int32 ItemId
        {
            get
            {
                return ((Int32)(base.GetValue(CalcItemTotal35.ItemIdProperty)));
            }
            set
            {
                base.SetValue(CalcItemTotal35.ItemIdProperty, value);
            }
        }

        public static DependencyProperty ShipViaProperty =
            DependencyProperty.Register(
                "ShipVia", typeof(string), typeof(CalcItemTotal35));

        [Description("ShipVia")]
        [Category("CalcItemTotal35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ShipVia
        {
            get
            {
                return ((string)(base.GetValue(CalcItemTotal35.ShipViaProperty)));
            }
            set
            {
                base.SetValue(CalcItemTotal35.ShipViaProperty, value);
            }
        }

        public static DependencyProperty WeightProperty =
            DependencyProperty.Register(
                "Weight", typeof(Int32), typeof(CalcItemTotal35));

        [Description("Weight")]
        [Category("CalcItemTotal35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Int32 Weight
        {
            get
            {
                return ((Int32)(base.GetValue(CalcItemTotal35.WeightProperty)));
            }
            set
            {
                base.SetValue(CalcItemTotal35.WeightProperty, value);
            }
        }

        public static DependencyProperty PriceProperty =
            DependencyProperty.Register(
                "Price", typeof(Decimal), typeof(CalcItemTotal35));

        [Description("Price")]
        [Category("CalcItemTotal35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Decimal Price
        {
            get
            {
                return ((Decimal)(base.GetValue(CalcItemTotal35.PriceProperty)));
            }
            set
            {
                base.SetValue(CalcItemTotal35.PriceProperty, value);
            }
        }


        public static DependencyProperty ResultProperty =
            DependencyProperty.Register(
                "Result", typeof(Decimal), typeof(CalcItemTotal35));

        [Description("Result")]
        [Category("CalcItemTotal35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Decimal Result
        {
            get
            {
                return ((Decimal)(base.GetValue(CalcItemTotal35.ResultProperty)));
            }
            set
            {
                base.SetValue(CalcItemTotal35.ResultProperty, value);
            }
        }

        private void codeDisplayItem_ExecuteCode(object sender, EventArgs e)
        {
            Console.WriteLine(
                "Retrieved info for ItemId:{0} Weight:{1} Price:{2}",
                    ItemId, Weight, Price);
        }
    }
}
