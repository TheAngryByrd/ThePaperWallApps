using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ThePaperWall.WinRT.Fixins
{
    public class ListViewItemCommand
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ListViewItemCommand), new PropertyMetadata(null, CommandPropertyChanged));

        public static void SetCommand(DependencyObject attached, ICommand value)
        {
            attached.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject attached)
        {
            return (ICommand)attached.GetValue(CommandProperty);
        }

        private static void CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Attach click handler
            (d as ListView).ItemClick += listView_ItemClick;
        }

        private static void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get ListView
            var listView = (sender as ListView);

            // Get command
            ICommand command = GetCommand(listView);

            // Execute command
            command.Execute(e.ClickedItem);
        }
    }
}