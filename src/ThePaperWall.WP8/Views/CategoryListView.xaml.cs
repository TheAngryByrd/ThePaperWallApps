
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ReactiveUI;
using ThePaperWall.WP8.ViewModels;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SlideView;
using System.Threading.Tasks;
using ThePaperWall.ViewModels;
using System.Reactive;
using System.Reactive.Linq;

namespace ThePaperWall.WP8.Views
{
    public partial class CategoryListView : PhoneApplicationPage
    {
        private const int NumberOfPictureToLoad = 3;

        public CategoryListView()
        {
            InitializeComponent();
            this.listBox.SetValue(InteractionEffectManager.IsInteractionEnabledProperty, true);
            this.slideView.SetValue(InteractionEffectManager.IsInteractionEnabledProperty, true);
            InteractionEffectManager.AllowedTypes.Add(typeof(RadDataBoundListBoxItem));
            InteractionEffectManager.AllowedTypes.Add(typeof(SlideViewItem));
            this.listBox.RealizedItemsBufferScale = 1.5;          
            WhenDataContextIsSet()
                       .Subscribe(_ => Loaded());             
        }      

        public void Loaded()
        {
            ListBoxDataRequested
             .Throttle(TimeSpan.FromMilliseconds(700))
             .Select(x => NumberOfPictureToLoad)             
             .InvokeCommand(ViewModel.AddMorePicturesCommand);

            SlideAnimationCompleted
               .Where(_ => SlideViewsNextItemIsTheFirstItem ||
                           SlideViewsNextItemIsTheLastItem)
               .Select(x => NumberOfPictureToLoad)
               .InvokeCommand(ViewModel.AddMorePicturesCommand);

            SlideViewTapped
                .Select(_ => this.slideView.SelectedItem as CategoryItem)
                .InvokeCommand(ViewModel.FullScreenCommand);

            ListBoxSelectionChanged
                .Select(e => e.EventArgs.AddedItems[0] as CategoryItem)
                .Subscribe(item => {
                    slideView.SelectedItem = item;
                });
        }
  
        private bool SlideViewsNextItemIsTheLastItem
        {
            get {return slideView.NextItem == slideView.ItemsSource.ElementAt(slideView.ItemsSource.Count() - 1);}
        }
  
        private bool SlideViewsNextItemIsTheFirstItem
        {
            get {return slideView.NextItem == slideView.ItemsSource.ElementAt(0);}
        }

        //Using a region. Observables should just be the defacto event in .NET
        #region Coverting .NET events to Observables
        private IObservable<IObservedChange<CategoryListView, object>> WhenDataContextIsSet()
        {
            return this.ObservableForProperty(x => x.DataContext);
        }

        public IObservable<EventPattern<EventArgs>> ListBoxDataRequested
        {
            get
            {
                return Observable.FromEventPattern<EventArgs>(ev => listBox.DataRequested += ev, ev => listBox.DataRequested -= ev);
            }
        }
        public IObservable<EventPattern<object>> SlideAnimationCompleted
        {
            get
            {
                return Observable.FromEventPattern(ev => slideView.SlideAnimationCompleted += ev,
                    ev => slideView.SlideAnimationCompleted -= ev);
            }
        }
        public IObservable<EventPattern<System.Windows.Input.GestureEventArgs>> SlideViewTapped
        {
            get
            {

                return Observable.FromEventPattern<System.Windows.Input.GestureEventArgs>(ev => slideView.Tap += ev,
                    ev => slideView.Tap -= ev);
            }
        }

        public IObservable<EventPattern<SelectionChangedEventArgs>> ListBoxSelectionChanged
        {
            get
            {
                return Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(ev => listBox.SelectionChanged += ev,
                    ev => listBox.SelectionChanged -= ev);
            }
        }
        #endregion


        public CategoryListViewModel ViewModel
        {
            get
            { return DataContext as CategoryListViewModel; }
        }  
    }
}
