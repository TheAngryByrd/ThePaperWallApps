using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThePaperWall.Core.Downloads;
using ThePaperWall.Core.Feeds;
using ThePaperWall.Core.Rss;
using ThePaperWall.WP8.ViewModels;

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

        void AddCustomConventions()
        {
            AddViewModels();
            AddView();

            _container.PerRequest<IThemeService, ThemeService>();
            _container.PerRequest<IAsyncDownloadManager, AsyncDownloadManager>();
            _container.PerRequest<IRssReader, RssReader>();

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
    }
}
