using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace ThePaperWall.WP8.Helpers
{
    public interface IDialogService
    {
        void ShowDialogBox(string title, string text);

        void ShowDialogBox(string caption, string message, string leftbuttonContent, string rightButtonContent, Action leftButtonAction, Action rightButtonAction);
    }
}