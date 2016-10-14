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
using System.Activities.Statements;
using System.Collections.Generic;

namespace TestDynamicActivity
{
    class Program
    {
        static void Main(string[] args)
        {
            ////39
            //NormalTest(CreateDynamicActivity());
            ////12.95
            //NormalMinimumTest(CreateDynamicActivity());
            ////0
            //NormalFreeTest(CreateDynamicActivity());
            ////17.50
            //ExpressTest(CreateDynamicActivity());

            //streamlined version follows

            //39
            NormalTest(CreateDynamicActivity("normal"));
            //12.95
            NormalMinimumTest(CreateDynamicActivity("normal"));
            //0
            NormalFreeTest(CreateDynamicActivity("normal"));
            //17.50
            ExpressTest(CreateDynamicActivity("express"));


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        #region Full version of CalcShippingInCode

        private static Activity CreateDynamicActivity()
        {
            DynamicActivity<Decimal> a = new DynamicActivity<Decimal>();
            a.DisplayName = "DynamicCalcShipping";

            InArgument<Int32> Weight = new InArgument<int>();
            InArgument<Decimal> OrderTotal = new InArgument<decimal>();
            InArgument<String> ShipVia = new InArgument<string>();

            Variable<Boolean> isFreeShipping =
                new Variable<Boolean> { Name = "IsFreeShipping" };
            Variable<Decimal> normalRate =
                new Variable<Decimal> { Name = "NormalRate", Default = 1.95M };
            Variable<Decimal> expressRate =
                new Variable<Decimal> { Name = "ExpressRate", Default = 3.50M };
            Variable<Decimal> minimum =
                new Variable<Decimal> { Name = "Minimum", Default = 12.95M };
            Variable<Decimal> freeThreshold =
                new Variable<Decimal> { Name = "FreeThreshold", Default = 75.00M };

            a.Properties.Add(new DynamicActivityProperty
            {
                Name = "Weight",
                Type = typeof(InArgument<Int32>),
                Value = Weight
            });
            a.Properties.Add(new DynamicActivityProperty
            {
                Name = "OrderTotal",
                Type = typeof(InArgument<Decimal>),
                Value = OrderTotal
            });
            a.Properties.Add(new DynamicActivityProperty
            {
                Name = "ShipVia",
                Type = typeof(InArgument<String>),
                Value = ShipVia
            });

            a.Implementation = () =>
            {
                Sequence root = new Sequence();
                root.Variables.Add(isFreeShipping);
                root.Variables.Add(normalRate);
                root.Variables.Add(expressRate);
                root.Variables.Add(minimum);
                root.Variables.Add(freeThreshold);

                //normal if statement to test free threshold
                If normalIf = new If();
                normalIf.Condition = new InArgument<Boolean>(ac =>
                    OrderTotal.Get(ac) >= freeThreshold.Get(ac));

                //meets free threshold
                Assign<Boolean> isFreeAssign = new Assign<Boolean>();
                isFreeAssign.To = new OutArgument<Boolean>(ac =>
                    isFreeShipping.Get(ac));
                isFreeAssign.Value = true;
                normalIf.Then = isFreeAssign;

                //not free, so calc using normal rate
                Assign<Decimal> calcNormalAssign = new Assign<Decimal>();
                calcNormalAssign.To = new OutArgument<Decimal>(ac =>
                    a.Result.Get(ac));
                calcNormalAssign.Value = new InArgument<Decimal>(ac =>
                    Weight.Get(ac) * normalRate.Get(ac));
                normalIf.Else = calcNormalAssign;

                //calc using express rate
                Assign<Decimal> expressAssign = new Assign<Decimal>();
                expressAssign.To = new OutArgument<Decimal>(ac =>
                    a.Result.Get(ac));
                expressAssign.Value = new InArgument<Decimal>(ac =>
                    Weight.Get(ac) * expressRate.Get(ac));

                //determine normal or express shipping
                Switch<String> sw = new Switch<string>(
                    new InArgument<string>(ac => ShipVia.Get(ac)));
                sw.Cases.Add("normal", normalIf);
                sw.Cases.Add("express", expressAssign);

                root.Activities.Add(sw);

                //test for minimum charge
                If testMinIf = new If();
                testMinIf.Condition = new InArgument<bool>(ac =>
                    a.Result.Get(ac) < minimum.Get(ac) &&
                        (!isFreeShipping.Get(ac)));
                Assign minAssign = new Assign();
                minAssign.To = new OutArgument<Decimal>(ac => a.Result.Get(ac));
                minAssign.Value = new InArgument<Decimal>(ac => minimum.Get(ac));
                testMinIf.Then = minAssign;

                root.Activities.Add(testMinIf);

                return root;
            };

            return a;
        }

        #endregion

        #region Streamlined procedural version of CalcShippingInCode

        private static Activity CreateDynamicActivity(String shipVia)
        {
            Boolean isNormal = (shipVia == "normal");

            DynamicActivity<Decimal> a = new DynamicActivity<Decimal>();
            a.DisplayName = "DynamicCalcShipping";

            InArgument<Int32> Weight = new InArgument<int>();
            InArgument<Decimal> OrderTotal = new InArgument<decimal>();

            //InArgument<String> ShipVia = new InArgument<string>();

            Variable<Boolean> isFreeShipping =
                new Variable<Boolean> { Name = "IsFreeShipping" };

            Variable<Decimal> rate = null;
            if (isNormal)
            {
                rate = new Variable<Decimal> { Name = "Rate", Default = 1.95M };
            }
            else
            {
                rate = new Variable<Decimal> { Name = "Rate", Default = 3.50M };
            }

            //Variable<Decimal> expressRate =
            //    new Variable<Decimal> { Name = "ExpressRate", Default = 3.50M };
            Variable<Decimal> minimum =
                new Variable<Decimal> { Name = "Minimum", Default = 12.95M };
            Variable<Decimal> freeThreshold =
                new Variable<Decimal> { Name = "FreeThreshold", Default = 75.00M };

            a.Properties.Add(new DynamicActivityProperty
            {
                Name = "Weight",
                Type = typeof(InArgument<Int32>),
                Value = Weight
            });
            a.Properties.Add(new DynamicActivityProperty
            {
                Name = "OrderTotal",
                Type = typeof(InArgument<Decimal>),
                Value = OrderTotal
            });
            //a.Properties.Add(new DynamicActivityProperty
            //{
            //    Name = "ShipVia",
            //    Type = typeof(InArgument<String>),
            //    Value = ShipVia
            //});

            a.Implementation = () =>
            {
                Sequence root = new Sequence();
                root.Variables.Add(isFreeShipping);
                root.Variables.Add(rate);
                //root.Variables.Add(expressRate);
                root.Variables.Add(minimum);
                root.Variables.Add(freeThreshold);

                if (isNormal)
                {
                    //normal if statement to test free threshold
                    If normalIf = new If();
                    normalIf.Condition = new InArgument<Boolean>(ac =>
                        OrderTotal.Get(ac) >= freeThreshold.Get(ac));

                    //meets free threshold
                    Assign<Boolean> isFreeAssign = new Assign<Boolean>();
                    isFreeAssign.To = new OutArgument<Boolean>(ac =>
                        isFreeShipping.Get(ac));
                    isFreeAssign.Value = true;
                    normalIf.Then = isFreeAssign;

                    //not free, so calc using normal rate
                    Assign<Decimal> calcNormalAssign = new Assign<Decimal>();
                    calcNormalAssign.To = new OutArgument<Decimal>(ac =>
                        a.Result.Get(ac));
                    calcNormalAssign.Value = new InArgument<Decimal>(ac =>
                        Weight.Get(ac) * rate.Get(ac));
                    normalIf.Else = calcNormalAssign;
                    root.Activities.Add(normalIf);
                }
                else
                {
                    //calc using express rate
                    Assign<Decimal> expressAssign = new Assign<Decimal>();
                    expressAssign.To = new OutArgument<Decimal>(ac =>
                        a.Result.Get(ac));
                    expressAssign.Value = new InArgument<Decimal>(ac =>
                        Weight.Get(ac) * rate.Get(ac));
                    root.Activities.Add(expressAssign);
                }

                ////determine normal or express shipping
                //Switch<String> sw = new Switch<string>(
                //    new InArgument<string>(ac => ShipVia.Get(ac)));
                //sw.Cases.Add("normal", normalIf);
                //sw.Cases.Add("express", expressAssign);

                //root.Activities.Add(sw);

                //test for minimum charge
                If testMinIf = new If();
                testMinIf.Condition = new InArgument<bool>(ac =>
                    a.Result.Get(ac) < minimum.Get(ac) &&
                        (!isFreeShipping.Get(ac)));
                Assign minAssign = new Assign();
                minAssign.To = new OutArgument<Decimal>(ac => a.Result.Get(ac));
                minAssign.Value = new InArgument<Decimal>(ac => minimum.Get(ac));
                testMinIf.Then = minAssign;

                root.Activities.Add(testMinIf);

                return root;
            };

            return a;
        }

        #endregion

        private static void NormalTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            //            parameters.Add("ShipVia", "normal");
            parameters.Add("Weight", 20);
            parameters.Add("OrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Console.WriteLine("Normal Result: {0}", outputs["Result"]);
        }

        private static void NormalMinimumTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            //            parameters.Add("ShipVia", "normal");
            parameters.Add("Weight", 5);
            parameters.Add("OrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Console.WriteLine("NormalMinimum Result: {0}", outputs["Result"]);
        }

        private static void NormalFreeTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            //            parameters.Add("ShipVia", "normal");
            parameters.Add("Weight", 5);
            parameters.Add("OrderTotal", 75M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Console.WriteLine("NormalFree Result: {0}", outputs["Result"]);
        }

        private static void ExpressTest(Activity activity)
        {
            Dictionary<String, Object> parameters
                = new Dictionary<string, object>();
            //            parameters.Add("ShipVia", "express");
            parameters.Add("Weight", 5);
            parameters.Add("OrderTotal", 50M);

            IDictionary<String, Object> outputs = WorkflowInvoker.Invoke(
                activity, parameters);
            Console.WriteLine("Express Result: {0}", outputs["Result"]);
        }
    }
}
