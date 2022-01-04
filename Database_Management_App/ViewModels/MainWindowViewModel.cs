using Database_Management_App.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Database_Management_App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            List<Tenant> tenants = LoadConfig<List<Tenant>>("Tenants");
            TenantItems = new ObservableCollection<TenantViewModel>();
            TabItems = new ObservableCollection<TabViewModel>();
            DataGridItems = new ObservableCollection<DataGridViewModel>();
            int idx = 1;
            foreach (var item in tenants)
            {
                TenantItems.Add(new TenantViewModel()
                {
                    Id = idx++,
                    Name = item.Name,
                    ConnectionString = item.ConnectionString,
                    IsChecked = false
                });
            }
            _tenantItemsView = CollectionViewSource.GetDefaultView(TenantItems);
            _tenantItemsView.Filter = TenantItemsFilter;
            TabInit();
        }

        private CancellationTokenSource _cancellationTokenSource;
        public ObservableCollection<TenantViewModel> TenantItems { get; set; }
        public ObservableCollection<DataGridViewModel> DataGridItems { get; set; }
        public ObservableCollection<TabViewModel> TabItems { get; set; }
        private readonly ICollectionView _tenantItemsView;
        private string _searchKeyword;
        private bool _isRunning;
        private string _inputString;
        private string _outputString;
        private string _selectedString;
        private int _selectedTabIndex;

        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    _tenantItemsView.Refresh();
                }
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public string InputString
        {
            get => _inputString;
            set => SetProperty(ref _inputString, value);
        }

        public string OutputString
        {
            get => _outputString;
            set => SetProperty(ref _outputString, value);
        }

        public string SelectedString
        {
            get => _selectedString;
            set => SetProperty(ref _selectedString, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        private bool TenantItemsFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(_searchKeyword))
            {
                return true;
            }

            return obj is TenantViewModel item
                   && item.Name.ToLower().Contains(_searchKeyword!.ToLower());
        }

        public ICommand RunCommand
        {
            get
            {
                return new CommandBase((param) =>
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    IsRunning = true;
                    Clear();

                    Task.Run(() =>
                    {
                        Run();
                        IsRunning = false;
                    }, _cancellationTokenSource.Token);
                });
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return new CommandBase((param) =>
                {
                    _cancellationTokenSource.Cancel();
                });
            }
        }

        public ICommand TabSelectCommand
        {
            get
            {
                return new CommandBase((param) =>
                {
                    if (param != null)
                    {
                        SelectedTabIndex = (int)param;
                    }
                });
            }
        }

        public ICommand AllSelectCommand
        {
            get
            {
                return new CommandBase((param) =>
                {
                    var list = TenantItems.AsEnumerable();
                    if (!string.IsNullOrWhiteSpace(_searchKeyword))
                    {
                        list = TenantItems.Where(d => d.Name.ToLower().Contains(_searchKeyword.ToLower()));
                    }

                    UpdateTenantList(TenantItems.Except(list), false);

                    if (list.Any(d => d.IsChecked == false))
                    {
                        UpdateTenantList(list, true);
                    }
                    else
                    {
                        UpdateTenantList(list, false);
                    }
                    _tenantItemsView.Refresh();

                    void UpdateTenantList(IEnumerable<TenantViewModel> list, bool flag)
                    {
                        if (list == null || list.Count() == 0)
                        {
                            return;
                        }
                        foreach (var item in list)
                        {
                            item.IsChecked = flag;
                        }
                    }
                });
            }
        }

        public void Run()
        {
            if (string.IsNullOrWhiteSpace(_inputString))
            {
                return;
            }
            string sql = !string.IsNullOrWhiteSpace(_selectedString) ? _selectedString : _inputString;
            TabInit();
            int tabIndex = 1;
            Handle((conn, tenant) =>
            {
                var adapter = new MySqlDataAdapter(sql, conn);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count == 0)
                {
                    return;
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < dataSet.Tables.Count; i++)
                    {
                        TabItems.Add(new TabViewModel()
                        {
                            Id = tabIndex,
                            Content = tenant.Name
                        });
                        DataGridItems.Add(new DataGridViewModel(dataSet.Tables[i], tabIndex));
                        tabIndex++;
                    }
                });
            });

            void Handle(Action<MySqlConnection, TenantViewModel> action)
            {
                var list = TenantItems.Where(d => d.IsChecked == true).ToList();
                foreach (var item in list)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                    Println(item.Name);
                    try
                    {
                        using (var conn = GetSqlConnection(item.ConnectionString))
                        {
                            action(conn, item);
                        }
                    }
                    catch (Exception ex)
                    {
                        Println(ex.Message);
                        break;
                    }
                    Println("ok" + Environment.NewLine);
                }
            }
        }

        private MySqlConnection GetSqlConnection(string sqlConnectionString)
        {
            MySqlConnection conn = new MySqlConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }

        private void Println(object message)
        {
            OutputString += $"> {message}{Environment.NewLine}";
        }

        private void Print(object message)
        {
            OutputString += message;
        }

        private void Clear()
        {
            OutputString = string.Empty;
        }

        private void TabInit()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                TabItems.Clear();
                DataGridItems.Clear();
                TabItems.Add(new TabViewModel()
                {
                    Id = -1,
                    Content = "信息",
                    IsSelected = true
                });
                SelectedTabIndex = -1;
            });
        }

        private T LoadConfig<T>(string name, string path = "appsettings.json") where T : class
        {
            string config = File.ReadAllText(path);
            return JObject.Parse(config).GetValue(name).ToObject<T>();
        }
    }
}