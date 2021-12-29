using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Management_App.ViewModels
{
    public class DataGridViewModel : ViewModelBase
    {
        public int TabIndex { get; set; }
        public DataView DataView { get; set; }
        public DataGridViewModel(DataTable data, int index)
        {
            TabIndex = index;
            DataView = data.DefaultView;
        }
    }
}
