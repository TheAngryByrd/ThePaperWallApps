using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ThePaperWall.WP8.Helpers
{
    public class DialogService : IDialogService 
    {
        public void ShowDialogBox(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK);
        }

        public void ShowDialogBox(string caption, string message, string leftbuttonContent, string rightButtonContent, Action leftButtonAction, Action rightButtonAction)
        {
            var messagebox = new CustomMessageBox()
            {
                Caption = caption,
                Message = message,
                LeftButtonContent = leftbuttonContent,
                RightButtonContent = rightButtonContent
            };

            var tcs = new TaskCompletionSource<object>();

            messagebox.Dismissed += (s, e) =>
            {
                switch (e.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        leftButtonAction();
                        break;
                    case CustomMessageBoxResult.RightButton:
                        rightButtonAction();
                        break;
                    case CustomMessageBoxResult.None:
                        break;
                };
            };

            messagebox.Show();
        }
        public Task<bool> ShowDialogBox(string caption, string message, string leftbuttonContent, string rightButtonContent, Func<Task> leftButtonAction, Func<Task> rightButtonAction)
        {
            var messagebox = new CustomMessageBox()
            {
                Caption = caption,
                Message = message,
                LeftButtonContent = leftbuttonContent,
                RightButtonContent = rightButtonContent
            };

            var tcs = new TaskCompletionSource<bool>();
            messagebox.Dismissed += async (s, e) =>
            {
                switch (e.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                         await leftButtonAction();
                        tcs.SetResult(true);
                        break;
                    case CustomMessageBoxResult.RightButton:
                        await rightButtonAction();

                        tcs.SetResult(true);
                        break;
                    case CustomMessageBoxResult.None:

                        tcs.SetResult(false);
                        break;
                };
            };

            messagebox.Show();

            return tcs.Task;
        }
    }
}
