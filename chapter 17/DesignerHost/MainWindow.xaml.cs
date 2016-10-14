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
using System.Activities.Core.Presentation;
using System.Activities.Core.Presentation.Factories;
using System.Activities.Presentation;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.Toolbox;
using System.Activities.Presentation.Validation;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;

namespace DesignerHost
{
    public partial class MainWindow : Window
    {
        private WorkflowDesigner wd;
        private ToolboxControl toolboxControl;
        private String currentXamlPath;
        private String originalTitle;
        private Boolean isModified = false;

#if VALIDATION
        private ValidationErrorService errorService;
#endif

        private HashSet<Type> loadedToolboxActivities =
            new HashSet<Type>();

#if EXTRATOOLBOX
        private ToolboxCategory autoAddedCategory;
        private Dictionary<String, ToolboxCategory> referencedCategories =
            new Dictionary<string, ToolboxCategory>();
#endif

#if FLOWCHART
        private ToolboxCategory flowchartCategory;
        private List<ToolboxItemWrapper> flowchartActivities =
            new List<ToolboxItemWrapper>();
#endif

#if CONTEXTMENU
        private MenuItem miAddFlowDecision;
#endif

        #region Construction and Initialization

        public MainWindow()
        {
            InitializeComponent();

            originalTitle = this.Title;

            //register designers for the standard activities
            DesignerMetadata dm = new DesignerMetadata();
            dm.Register();

            //toolbox
            toolboxControl = CreateToolbox();
            toolboxArea.Child = toolboxControl;

#if CONTEXTMENU
            //create a context menu item
            CreateContextMenu();
#endif

#if VALIDATION
            errorService = new ValidationErrorService(this.messageListBox);
#endif

            InitializeDesigner();
            StartNewWorkflow();


#if CUSTOMDESIGNER
            //override designer for the standard While activity
            AttributeTableBuilder atb = new AttributeTableBuilder();
            atb.AddCustomAttributes(typeof(While), new DesignerAttribute(
                typeof(ActivityLibrary.Design.MyWhileDesigner)));
            MetadataStore.AddAttributeTable(atb.CreateTable());
#endif
        }

        private void InitializeDesigner()
        {
            //cleanup the previous designer
            if (wd != null)
            {
                wd.ModelChanged -= new EventHandler(Designer_ModelChanged);
            }

            //designer
            wd = new WorkflowDesigner();
            designerArea.Child = wd.View;

            //property grid
            propertiesArea.Child = wd.PropertyInspectorView;

            //event handler
            wd.ModelChanged += new EventHandler(Designer_ModelChanged);

#if VALIDATION
            //error service
            wd.Context.Services.Publish<IValidationErrorService>(errorService);
#endif

#if FLOWCHART
            //ModelService ms = wd.Context.Services.GetService<ModelService>();
            wd.Context.Items.Subscribe<Selection>(OnItemSelected);
#endif
        }

        private void StartNewWorkflow()
        {
            wd.Load(new ActivityBuilder
            {
                Name = "Activity1"
            });
            currentXamlPath = null;
            isModified = false;

#if EXTRATOOLBOX
            RemoveAutoAddedToolboxCategory();
#endif
        }

        #endregion

        #region Event handling

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
            if (IsCloseAllowed())
            {
                InitializeDesigner();
                StartNewWorkflow();
                UpdateTitle();
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
#if LOADSAVE
            if (String.IsNullOrEmpty(currentXamlPath))
            {
                menuSaveAs_Click(sender, e);
            }
            else
            {
                wd.Flush();
                SaveXamlFile(currentXamlPath, wd.Text);
                isModified = false;
                UpdateTitle();
            }
#endif
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
#if LOADSAVE
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".xaml";
            dialog.Filter = "Xaml files (.xaml)|*xaml|All files|*.*";
            dialog.FilterIndex = 0;
            Boolean? result = dialog.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                wd.Flush();
                currentXamlPath = dialog.FileName;
                SaveXamlFile(currentXamlPath, wd.Text);
                isModified = false;
                UpdateTitle();
            }
#endif
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
#if LOADSAVE
            if (!IsCloseAllowed())
            {
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".xaml";
            dialog.Filter = "Xaml files (.xaml)|*xaml|All files|*.*";
            dialog.FilterIndex = 0;
            Boolean? result = dialog.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                String markup = LoadXamlFile(dialog.FileName);
                if (!String.IsNullOrEmpty(markup))
                {
                    InitializeDesigner();
                    wd.Text = markup;
                    wd.Load();
                    isModified = false;
                    currentXamlPath = dialog.FileName;
                    UpdateTitle();
#if EXTRATOOLBOX
                    AutoAddActivitiesToToolbox();
#endif
                }
                else
                {
                    MessageBox.Show(this,
                        String.Format(
                            "Unable to load xaml file {0}", dialog.FileName),
                        "Open File Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
#endif
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            if (IsCloseAllowed())
            {
                isModified = false;
                this.Close();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!IsCloseAllowed())
            {
                e.Cancel = true;
            }
        }

        private void menuRun_Click(object sender, RoutedEventArgs e)
        {
#if RUNWF
            try
            {
                wd.Flush();
                Activity activity = null;
                using (StringReader reader = new StringReader(wd.Text))
                {
                    activity = ActivityXamlServices.Load(reader);
                }

                if (activity != null)
                {
#if LISTARGS
                    //list any defined arguments
                    messageListBox.Items.Clear();
                    ModelService ms =
                        wd.Context.Services.GetService<ModelService>();
                    ModelItemCollection items =
                        ms.Root.Properties["Properties"].Collection;
                    foreach (var item in items)
                    {
                        if (item.ItemType == typeof(DynamicActivityProperty))
                        {
                            DynamicActivityProperty prop =
                                item.GetCurrentValue() as DynamicActivityProperty;
                            if (prop != null)
                            {
                                Argument arg = prop.Value as Argument;
                                if (arg != null)
                                {
                                    messageListBox.Items.Add(String.Format(
                                        "Name={0} Type={1} Direction={2} Exp={3}",
                                            prop.Name, arg.ArgumentType,
                                            arg.Direction, arg.Expression));
                                }
                            }
                        }
                    }
#endif

                    StringBuilder sb = new StringBuilder();
                    using (StringWriter writer = new StringWriter(sb))
                    {
                        Console.SetOut(writer);
                        try
                        {
                            WorkflowInvoker.Invoke(activity);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(this,
                                exception.Message, "Exception",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            MessageBox.Show(this,
                                sb.ToString(), "Results",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    StreamWriter standardOutput =
                        new StreamWriter(Console.OpenStandardOutput());
                    Console.SetOut(standardOutput);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this,
                    exception.Message, "Outer Exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
#endif
        }

        private void menuAddReference_Click(object sender, RoutedEventArgs e)
        {
#if EXTRATOOLBOX
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".dll";
            dialog.Filter = "Assemblies (.dll)|*dll|All files|*.*";
            dialog.FilterIndex = 0;
            Boolean? result = dialog.ShowDialog(this);

            if (result.HasValue && result.Value)
            {
                AddReferencedActivitiesToToolbox(dialog.FileName);
            }
#endif
        }

        private void Designer_ModelChanged(object sender, EventArgs e)
        {
            isModified = true;
            UpdateTitle();
        }

#if FLOWCHART
        private void OnItemSelected(Selection item)
        {
            ModelItem mi = item.PrimarySelection;
            if (mi != null)
            {
                if (flowchartCategory != null && wd.ContextMenu != null)
                {
                    if (mi.ItemType == typeof(Flowchart))
                    {
                        //add the flowchart-only activities
                        foreach (var tool in flowchartActivities)
                        {
                            if (!flowchartCategory.Tools.Contains(tool))
                            {
                                flowchartCategory.Tools.Add(tool);
                            }
                        }

#if CONTEXTMENU
                        if (!wd.ContextMenu.Items.Contains(miAddFlowDecision))
                        {
                            wd.ContextMenu.Items.Add(miAddFlowDecision);
                        }
#endif
                    }
                    else
                    {
                        //remove the flowchart-only activities
                        foreach (var tool in flowchartActivities)
                        {
                            flowchartCategory.Tools.Remove(tool);
                        }
#if CONTEXTMENU
                        wd.ContextMenu.Items.Remove(miAddFlowDecision);
#endif
                    }
                }
            }
        }
#endif

        #endregion

        #region File IO

#if LOADSAVE
        private String LoadXamlFile(String path)
        {
            String markup = null;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        markup = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("LoadXamlFile exception: {0}:{1}",
                    exception.GetType(), exception.Message);
            }
            return markup;
        }

        private void SaveXamlFile(String path, String markup)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(markup);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("SaveXamlFile exception: {0}:{1}",
                    exception.GetType(), exception.Message);
            }
        }
#endif

        #endregion

        #region Toolbox methods

        private ToolboxControl CreateToolbox()
        {
            Dictionary<String, List<Type>> activitiesToInclude =
                new Dictionary<String, List<Type>>();

            activitiesToInclude.Add("Basic", new List<Type>
            {
                typeof(Sequence),
                typeof(If),
                typeof(While),
                typeof(Assign),
                typeof(WriteLine)
            });

            activitiesToInclude.Add("Flowchart", new List<Type>
            {
                typeof(Flowchart),
                typeof(FlowDecision),
                typeof(FlowSwitch<>)
            });

            activitiesToInclude.Add("Collections", new List<Type>
            {
                typeof(ForEachWithBodyFactory<>),
                typeof(ParallelForEachWithBodyFactory<>),
                typeof(ExistsInCollection<>),
                typeof(AddToCollection<>),
                typeof(RemoveFromCollection<>),
                typeof(ClearCollection<>),
            });

            activitiesToInclude.Add("Error Handling", new List<Type>
            {
                typeof(TryCatch),
                typeof(Throw),
                typeof(TransactionScope)
            });

            ToolboxControl tb = new ToolboxControl();
            foreach (var category in activitiesToInclude)
            {
                ToolboxCategory cat = CreateToolboxCategory(
                    category.Key, category.Value, true);
                tb.Categories.Add(cat);

#if FLOWCHART
                if (cat.CategoryName == "Flowchart")
                {
                    flowchartCategory = cat;
                    foreach (var tool in cat.Tools)
                    {
                        if (tool.Type == typeof(FlowDecision) ||
                            tool.Type == typeof(FlowSwitch<>))
                        {
                            flowchartActivities.Add(tool);
                        }
                    }
                }
#endif
            }

            return tb;
        }

        private ToolboxCategory CreateToolboxCategory(
            String categoryName, List<Type> activities, Boolean isStandard)
        {
            ToolboxCategory tc = new ToolboxCategory(categoryName);

            foreach (Type activity in activities)
            {
                if (!loadedToolboxActivities.Contains(activity))
                {
                    //cleanup the name of generic activities
                    String name;
                    String[] nameChunks = activity.Name.Split('`');
                    if (nameChunks.Length == 1)
                    {
                        name = activity.Name;
                    }
                    else
                    {
                        name = String.Format("{0}<>", nameChunks[0]);
                    }

                    ToolboxItemWrapper tiw = new ToolboxItemWrapper(
                        activity.FullName, activity.Assembly.FullName,
                            null, name);
                    //                        @"..\..\WindowsHS.png", name);
                    //                        @"..\..\RelationshipsHS.png", name);
                    tc.Add(tiw);

                    if (isStandard)
                    {
                        loadedToolboxActivities.Add(activity);
                    }
                }
            }

            return tc;
        }

#if EXTRATOOLBOX
        private void AutoAddActivitiesToToolbox()
        {
            ModelService ms = wd.Context.Services.GetService<ModelService>();
            IEnumerable<ModelItem> activities =
                ms.Find(ms.Root, typeof(Activity));

            List<String> namespacesToIgnore = new List<string>
            {
                "Microsoft.VisualBasic.Activities",
                "System.Activities.Expressions"
            };

            HashSet<Type> activitiesToAdd = new HashSet<Type>();
            foreach (ModelItem item in activities)
            {
                if (!loadedToolboxActivities.Contains(item.ItemType))
                {
                    if (!namespacesToIgnore.Contains(item.ItemType.Namespace))
                    {
                        if (!activitiesToAdd.Contains(item.ItemType))
                        {
                            activitiesToAdd.Add(item.ItemType);
                        }
                    }
                }
            }

            RemoveAutoAddedToolboxCategory();

            if (activitiesToAdd.Count > 0)
            {
                ToolboxCategory autoCat = CreateToolboxCategory(
                    "Auto", activitiesToAdd.ToList<Type>(), false);
                CreateAutoAddedToolboxCategory(autoCat);
            }
        }

        private void RemoveAutoAddedToolboxCategory()
        {
            if (autoAddedCategory != null)
            {
                toolboxControl.Categories.Remove(autoAddedCategory);
                autoAddedCategory = null;
            }
        }

        private void CreateAutoAddedToolboxCategory(ToolboxCategory autoCat)
        {
            //add this category to the top of the list
            List<ToolboxCategory> categories = new List<ToolboxCategory>();
            categories.Add(autoCat);
            categories.AddRange(toolboxControl.Categories);
            toolboxControl.Categories.Clear();
            foreach (var cat in categories)
            {
                toolboxControl.Categories.Add(cat);
            }

            autoAddedCategory = autoCat;
        }

        private void AddReferencedActivitiesToToolbox(String assemblyFileName)
        {
            try
            {
                HashSet<Type> activitiesToAdd = new HashSet<Type>();

                Assembly asm = Assembly.LoadFrom(assemblyFileName);
                if (asm != null)
                {
                    if (referencedCategories.ContainsKey(asm.GetName().Name))
                    {
                        return;
                    }

                    Type[] types = asm.GetTypes();
                    Type activityType = typeof(Activity);

                    foreach (Type t in types)
                    {
                        if (activityType.IsAssignableFrom(t))
                        {
                            if (!activitiesToAdd.Contains(t))
                            {
                                activitiesToAdd.Add(t);
                            }
                        }
                    }
                }

                if (activitiesToAdd.Count > 0)
                {
                    ToolboxCategory cat = CreateToolboxCategory(
                        asm.GetName().Name, activitiesToAdd.ToList<Type>(), false);
                    toolboxControl.Categories.Add(cat);
                    referencedCategories.Add(asm.GetName().Name, cat);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this,
                    exception.Message, "Exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
#endif

        #endregion

        #region Other private methods

        private void UpdateTitle()
        {
            String modified = (isModified ? "*" : String.Empty);
            if (String.IsNullOrEmpty(currentXamlPath))
            {
                this.Title = String.Format("{0} - {1}{2}",
                    originalTitle, "unsaved", modified);
            }
            else
            {
                this.Title = String.Format("{0} - {1}{2}",
                    originalTitle,
                    System.IO.Path.GetFileName(currentXamlPath), modified);
            }
        }

        private Boolean IsCloseAllowed()
        {
            Boolean result = true;

            if (isModified)
            {
                if (MessageBox.Show(this,
                    "Are you sure you want to lose unsaved changes?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                        == MessageBoxResult.No)
                {
                    result = false;
                }
            }

            return result;
        }

#if CONTEXTMENU
        private void CreateContextMenu()
        {
            miAddFlowDecision = new MenuItem();
            miAddFlowDecision.Header = "Add FlowDecision";
            miAddFlowDecision.Name = "miAddFlowDecision";
            miAddFlowDecision.Click +=
                new RoutedEventHandler(AddFlowDecision_Click);
        }
#endif

#if CONTEXTMENU
        private void AddFlowDecision_Click(object sender, RoutedEventArgs e)
        {
            ModelItem selected =
                wd.Context.Items.GetValue<Selection>().PrimarySelection;
            if (selected != null)
            {
                if (selected.ItemType == typeof(Flowchart))
                {
                    ModelProperty mp = selected.Properties["Nodes"];
                    if (mp != null)
                    {
                        mp.Collection.Add(new FlowDecision());
                    }
                }
            }
        }
#endif

        #endregion
    }
}
