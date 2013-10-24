using System.Linq.Expressions;
using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Linq;

namespace ThePaperWall.WinRT.Fixins
{
    public static class CaliburnEx
    {
        public static T GetInstance<T>(this WinRTContainer This, string key = null)
        {
            return (T)This.GetInstance(typeof(T), key);
        }

        //internal static Func<IUriBuilder<TViewModel>> factory = () => new UriBuilderAdapter<TViewModel>();

   
    }

    public static class CaliburnEx<TViewModel>
    {
        private static Func<IUriBuilder<TViewModel>> fac1Tory;

        public static IUriBuilder<TViewModel> GetUriForVM<TViewModel>(this INavigationService This, Func<IUriBuilder<TViewModel>> factory = null)
        {
            fac1Tory = factory;
            return (new UriBuilderAdapter<TViewModel>()).AttachTo(This);
        }
    }

    public interface IUriBuilder<TViewModel>
    {
        IUriBuilder<TViewModel> AttachTo(INavigationService navigationService);

        void Navigate();

        IUriBuilder<TViewModel> WithParam<TValue>(Expression<Func<TViewModel, TValue>> property, TValue value);
    }

    public class UriBuilderAdapter<TViewModel> : UriBuilder<TViewModel>, IUriBuilder<TViewModel>
    {
        public UriBuilder<TViewModel> Base { get; private set; }
        public UriBuilderAdapter()
        {
            Base = new UriBuilder<TViewModel>();
        }
        public IUriBuilder<TViewModel> WithParam<TValue>(Expression<Func<TViewModel, TValue>> property, TValue value)
        {
            Base = Base.WithParam<TValue>(property,value);
            return this;
        }
        public IUriBuilder<TViewModel> AttachTo(INavigationService navigationService)
        {
            Base = base.AttachTo(navigationService);
            return this;
        }

        public void Navigate()
        {
            Base.Navigate();
        }
    }
}
