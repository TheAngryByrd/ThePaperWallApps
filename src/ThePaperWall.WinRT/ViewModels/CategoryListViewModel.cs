using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveCaliburn;

namespace ThePaperWall.WinRT.ViewModels
{
    public class CategoryListViewModel : ReactiveScreen
    {
        public CategoryListViewModel()
        {

        }

        public string Category { get; set; }

        protected override async Task OnActivate()
        {
            
        }
    }
}
