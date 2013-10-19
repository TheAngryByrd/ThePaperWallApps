using Caliburn.Micro;
using ReactiveCaliburn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThePaperWall.WinRT.Fixins;

namespace ThePaperWall.WinRT.ViewModels
{
    public class BaseReactiveScreen : ReactiveScreen
    {
        private readonly INavigationService navigationService;

        public BaseReactiveScreen(INavigationService navigationService = null)
        {  
            this.navigationService = navigationService ?? (((App)App.Current).Container).GetInstance<INavigationService>(); ;
        }

        public void GoBack()
        {
            navigationService.GoBack();
        }

        public bool CanGoBack
        {
            get
            {
                return navigationService.CanGoBack;
            }
        }
    }
}
