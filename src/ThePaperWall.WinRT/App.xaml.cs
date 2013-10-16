using Caliburn.Micro;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.WinRT.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ThePaperWall.WinRT.ViewModels;
using System.Reflection;
using ThePaperWall.Core.Rss;
using Akavache;

// The Hub App template is documented at http://go.microsoft.com/fwlink/?LinkId=286574

namespace ThePaperWall.WinRT
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : CaliburnApplication
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            BlobCache.ApplicationName = "ThePaperWall";
            //Because Paul Betts says so
            BlobCache.LocalMachine.InsertObject("dumb", "winrt is dumb");
            DisplayRootViewFor<HubViewModel>();
        }
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

         private WinRTContainer _container;
        protected override void Configure()
        {
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            AddViewModels();
            AddView();

            _container.PerRequest<IThemeService, ThemeService>();
            _container.PerRequest<IAsyncDownloadManager, AsyncDownloadManager>();
            _container.PerRequest<IRssReader, RssReader>();

            //TODO: Register your view models at the container
        }

        private void AddViewModels()
        {
            string @namespace = "ThePaperWall.WinRT.ViewModels";

            RegisterType(@namespace);
        }
  
        private void RegisterType(string @namespace)
        {
            var types = (from t in typeof(App).GetTypeInfo().Assembly.DefinedTypes
                         where t.IsClass && t.Namespace == @namespace
                         select t.AsType()).ToList();
            foreach (var t in types)
            {
                _container.RegisterPerRequest(t, null, t);
            }
        }

        private void AddView()
        {
            string @namespace = "ThePaperWall.WinRT.Views";
            RegisterType(@namespace);
        }

     
        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;
            throw new Exception("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }
    
    }
}
