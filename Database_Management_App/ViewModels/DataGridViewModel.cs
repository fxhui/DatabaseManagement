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
        public List<string> ColumnItems { get; set; }
        public DataView DataView { get; set; }
        public DataGridViewModel(DataTable data,int index)
        {
            TabIndex = index;
            ColumnItems = new List<string>();
            DataView = data.DefaultView;
            if (data != null)
            {
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    ColumnItems.Add(data.Columns[i].ColumnName);
                }
            }
        }
    }
}
