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
    }
}
