using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using ThePaperWall.WP8.Resources;
using ThePaperWall.WP8.ViewModels;
using Akavache;

namespace ThePaperWall.WP8
{
    public partial class App : Application
    {

        //// <summary>
    /// Provides easy access to the root frame of the Phone Application.
    /// </summary>
    /// <returns>The root frame of the Phone Application.</returns>
    public static PhoneApplicationFrame RootFrame { get; private set; }
 
        public RadRateApplicationReminder reminder;

        /// <summary>
        /// Constructor for the Application object.
    /// </summary>
        public App()
        {
            // Standard XAML initialization
            InitializeComponent();
 
            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;
 
                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;
 
                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
 
                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
      
            BlobCache.ApplicationName = "ThePaperWall";
            BlobCache.LocalMachine.InsertObject("Why", "why why");


            this.reminder = new RadRateApplicationReminder();
            reminder.RecurrencePerUsageCount = 3;
            reminder.AllowUsersToSkipFurtherReminders = true;

            PhoneApplicationService.Current.Closing += App_Exit;
            this.UnhandledException += App_UnhandledException;
            this.Exit += App_Exit;
        }

        void App_Exit(object sender, EventArgs e)
        {
            Shutdown();
        }
  
        private void Shutdown()
        {
            //BlobCache.LocalMachine.Dispose();
            BlobCache.Shutdown().Wait();
        }

        void App_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
           
        }

       
       

    }
}