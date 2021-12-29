using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Management_App.ViewModels
{
    public class TabViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsSelected { get; set; }
    }
}
