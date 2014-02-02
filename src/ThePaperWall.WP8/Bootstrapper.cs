using Caliburn.Micro;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Telerik.Windows.Controls;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using ThePaperWall.WP8.Helpers;
using ThePaperWall.WP8.Services;

namespace ThePaperWall.WP8
{
    public class Bootstrapper : PhoneBootstrapper
    {
        PhoneContainer _container;

        protected override void Configure()
        {
            _container = new PhoneContainer();

            _container.RegisterPhoneServices(RootFrame);

            AddCustomConventions();
        }

        protected override void OnLaunch(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            ApplicationUsageHelper.Init("1.0");
        }

        protected override void OnActivate(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                //This will ensure that the ApplicationUsageHelper is initialized again if the application has been in Tombstoned state.
                ApplicationUsageHelper.OnApplicationActivated();
            } 
        }

        void BindVisiblityProperties(IEnumerable<FrameworkElement> frameWorkElements, Type viewModel)
        {
            foreach (var frameworkElement in frameWorkElements)
            {
                var propertyName = frameworkElement.Name + "IsVisible";
                var property = viewModel.GetPropertyCaseInsensitive(propertyName);
                if (property != null)
                {
                    var convention = ConventionManager
                        .GetElementConvention(typeof(FrameworkElement));
                    ConventionManager.SetBindingWithoutBindingOverwrite(
                        viewModel,
                        propertyName,
                        property,
                        frameworkElement,
                        convention,
                        convention.GetBindableProperty(frameworkElement));
                }
            }
        }
    

        void AddCustomConventions()
        {
            AddViewModels();
            AddView();


            var baseBindProperties = ViewModelBinder.BindProperties;
            ViewModelBinder.BindProperties =
                (frameWorkElements, viewModel) =>
                {
                    BindVisiblityProperties(frameWorkElements, viewModel);
                    return baseBindProperties(frameWorkElements, viewModel);
                };

            // Need to override BindActions as well, as it's called first and filters out anything it binds to before
            // BindProperties is called.
            var baseBindActions = ViewModelBinder.BindActions;
            ViewModelBinder.BindActions =
                (frameWorkElements, viewModel) =>
                {
                    BindVisiblityProperties(frameWorkElements, viewModel);
                    return baseBindActions(frameWorkElements, viewModel);
                };

            _container.PerRequest<IThemeService, ThemeService>();
            _container.PerRequest<IAsyncDownloadManager, AsyncDownloadManager>();
            _container.PerRequest<IRssReader, RssReader>();
            _container.PerRequest<ILockscreenHelper, LockscreenHelper>();
            _container.PerRequest<IDownloadHelper, DownloadHelper>();
            _container.PerRequest<IDialogService, DialogService>();
            _container.PerRequest<INotificationService, NotificationService>();

        }

        private void AddViewModels()
        {
            string @namespace = "ThePaperWall.WP8.ViewModels";

            var q = (from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && t.Namespace == @namespace select t).ToList();

            q.ForEach(t => _container.RegisterPerRequest(t, null, t));
        }
        private void AddView()
        {
            string @namespace = "ThePaperWall.WP8.Views";

            var q = (from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && t.Namespace == @namespace select t).ToList();

            q.ForEach(t => _container.RegisterPerRequest(t, null, t));
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            var objTransition = new RadTurnstileTransition();
            
            //objTransition.BackwardOutAnimation = objTransition.BackwardInAnimation;
            //objTransition.ForwardInAnimation = objTransition.ForwardOutAnimation;
            //objTransition.PlayMode = TransitionPlayMode.Consecutively;

            return new RadPhoneApplicationFrame() { Transition = objTransition };
        }
    }
}
