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
    public partial class CalcShipping35 : Activity
    {
        public static DependencyProperty WeightProperty =
            DependencyProperty.Register(
                "Weight", typeof(Int32), typeof(CalcShipping35));

        [Description("Weight")]
        [Category("CalcShipping35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Int32 Weight
        {
            get
            {
                return ((Int32)(base.GetValue(CalcShipping35.WeightProperty)));
            }
            set
            {
                base.SetValue(CalcShipping35.WeightProperty, value);
            }
        }

        public static DependencyProperty OrderTotalProperty =
            DependencyProperty.Register(
                "OrderTotal", typeof(Decimal), typeof(CalcShipping35));

        [Description("OrderTotal")]
        [Category("CalcShipping35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Decimal OrderTotal
        {
            get
            {
                return ((Decimal)(base.GetValue(
                    CalcShipping35.OrderTotalProperty)));
            }
            set
            {
                base.SetValue(CalcShipping35.OrderTotalProperty, value);
            }
        }

        public static DependencyProperty ShipViaProperty =
            DependencyProperty.Register(
                "ShipVia", typeof(string), typeof(CalcShipping35));

        [Description("ShipVia")]
        [Category("CalcShipping35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ShipVia
        {
            get
            {
                return ((string)(base.GetValue(CalcShipping35.ShipViaProperty)));
            }
            set
            {
                base.SetValue(CalcShipping35.ShipViaProperty, value);
            }
        }

        public static DependencyProperty ResultProperty =
            DependencyProperty.Register(
                "Result", typeof(Decimal), typeof(CalcShipping35));

        [Description("Result")]
        [Category("CalcShipping35 Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Decimal Result
        {
            get
            {
                return ((Decimal)(base.GetValue(CalcShipping35.ResultProperty)));
            }
            set
            {
                base.SetValue(CalcShipping35.ResultProperty, value);
            }
        }

        private Decimal _normalRate = 1.95M;
        private Decimal _expressRate = 3.50M;
        private Decimal _minimum = 12.95M;
        private Decimal _freeThreshold = 75.00M;
        private Boolean _isFreeShipping = false;

        public CalcShipping35()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(
            ActivityExecutionContext executionContext)
        {
            Result = 0;
            switch (ShipVia)
            {
                case "normal":
                    if (OrderTotal >= _freeThreshold)
                    {
                        _isFreeShipping = true;
                    }
                    else
                    {
                        Result = (Weight * _normalRate);
                    }
                    break;

                case "express":
                    Result = (Weight * _expressRate);
                    break;
            }

            if ((Result < _minimum) && (!_isFreeShipping))
            {
                Result = _minimum;
            }

            return ActivityExecutionStatus.Closed;
        }
    }
}
