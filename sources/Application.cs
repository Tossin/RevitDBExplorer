﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using AdW = Autodesk.Windows;

namespace RevitDBExplorer
{
    
    public class Application : IExternalApplication
    {
        public static IntPtr RevitWindowHandle;
        public static UIApplication UIApplication;
        public static FontFamily DefaultFontFamily;

        public Result OnStartup(UIControlledApplication application)
        {
            RevitWindowHandle = application.MainWindowHandle;

            var panel = application.CreateRibbonPanel("Explorer");
            var cmdType = typeof(Command);
            var pushButtonData = new PushButtonData(cmdType.FullName, "Revit DB\r\nExplorer", cmdType.Assembly.Location, cmdType.FullName);
            pushButtonData.Image = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.16.png", UriKind.RelativeOrAbsolute));
            pushButtonData.LargeImage = new BitmapImage(new Uri("pack://application:,,,/RevitDBExplorer;component/Resources/RDBE.Icon.32.png", UriKind.RelativeOrAbsolute));
            panel.AddItem(pushButtonData);

            var tab = ComponentManager.Ribbon.FindTab("Modify");
            if (tab != null)
            {
                var adwPanel = new AdW.RibbonPanel();
                adwPanel.CopyFrom(panel.GetRibbonPanel());
                tab.Panels.Add(adwPanel);
            }


            ExternalExecutor.CreateExternalEvent();
            MemberAccessorFactory.Init();
            ValueContainerFactory.Init();
            MemberStreamerForTemplates.Init();
            RevitDocumentationReader.Init();
            RevitDatabaseQueryService.Init();
            EventMonitor.Register(application);

            application.Idling += Application_Idling;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public static DateTime LastTimeWhen;        
        private void Application_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            LastTimeWhen = DateTime.Now;            
        }        
    }
}