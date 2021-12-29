using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Management_App.ViewModels
{
    public class TenantViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public bool IsChecked { get; set; }
    }
}
