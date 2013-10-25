using System;
using System.Threading.Tasks;

namespace ThePaperWall.WP8.Helpers
{
    public interface IDialogService
    {
        Task<bool> ShowDialogBox(string caption, string message, string leftbuttonContent, string rightButtonContent, Func<Task> leftButtonAction, Func<Task> rightButtonAction);

        void ShowDialogBox(string title, string text);

        void ShowDialogBox(string caption, string message, string leftbuttonContent, string rightButtonContent, Action leftButtonAction, Action rightButtonAction);
    }
}