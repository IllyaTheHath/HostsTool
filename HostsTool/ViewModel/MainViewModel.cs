using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using HostsTool.Command;
using HostsTool.Model;
using HostsTool.Util;
using HostsTool.View;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;

namespace HostsTool.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const String dataFilePath = "hosts.json";

        private ObservableCollection<Source> _sourceList;
        public ObservableCollection<Source> SourceList
        {
            get
            {
                return this._sourceList;
            }
            set
            {
                this._sourceList = value;
                OnPropertyChanged(nameof(SourceList));
            }
        }

        private Source _selectItem;
        public Source SelectedItem
        {
            get
            {
                return this._selectItem;
            }
            set
            {
                this._selectItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        private SnackbarMessageQueue _messageQueue;
        public SnackbarMessageQueue MessageQueue
        {
            get
            {
                return this._messageQueue;
            }
            set
            {
                this._messageQueue = value;
                OnPropertyChanged(nameof(MessageQueue));
            }
        }

        private ICommand _addItemCommand;
        public ICommand AddItemCommand
        {
            get
            {
                if (_addItemCommand == null)
                {
                    _addItemCommand = new DelegateCommand(
                        param => this.AddItem()
                    );
                }
                return _addItemCommand;
            }
        }

        private ICommand _removeItemCommand;
        public ICommand RemoveItemCommand
        {
            get
            {
                if (_removeItemCommand == null)
                {
                    _removeItemCommand = new DelegateCommand(
                        param => this.RemoveItem()
                    );
                }
                return _removeItemCommand;
            }
        }

        private ICommand _moveUpCommand;
        public ICommand MoveUpCommand
        {
            get
            {
                if (_moveUpCommand == null)
                {
                    _moveUpCommand = new DelegateCommand(
                        param => this.MoveUp()
                    );
                }
                return _moveUpCommand;
            }
        }

        private ICommand _moveDownCommand;
        public ICommand MoveDownCommand
        {
            get
            {
                if (_moveDownCommand == null)
                {
                    _moveDownCommand = new DelegateCommand(
                        param => this.MoveDown()
                    );
                }
                return _moveDownCommand;
            }
        }

        private ICommand _saveChangesCommand;
        public ICommand SaveChangesCommand
        {
            get
            {
                if (_saveChangesCommand == null)
                {
                    _saveChangesCommand = new DelegateCommand(
                        param => this.SaveChanges()
                    );
                }
                return _saveChangesCommand;
            }
        }

        private ICommand _updateHostsCommand;
        public ICommand UpdateHostsCommand
        {
            get
            {
                if (_updateHostsCommand == null)
                {
                    _updateHostsCommand = new DelegateCommand(
                        param => this.UpdateHosts()
                    );
                }
                return _updateHostsCommand;
            }
        }

        private ICommand _modifyHostsCommand;
        public ICommand ModifyHostsCommand
        {
            get
            {
                if (_modifyHostsCommand == null)
                {
                    _modifyHostsCommand = new DelegateCommand(
                        param => this.ShowModifyHosts()
                    );
                }
                return _modifyHostsCommand;
            }
        }

        public MainViewModel()
        {
            InitDefaultHosts();
            InitializeList();

            if (this.SourceList.Count == 0)
            {
                AddItem();
            }

            this.SelectedItem = this.SourceList[0];
            this.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(500));
        }

        private void InitDefaultHosts()
        {
            if (!File.Exists(dataFilePath))
            {
                File.Create(dataFilePath).Close();
            }
            if (String.IsNullOrWhiteSpace(File.ReadAllText(dataFilePath)))
            {
                SourceList = new ObservableCollection<Source>()
                {
                    new Source()
                    {
                        SourceGuid = Guid.NewGuid(),
                        SourceTitle = "Localhost",
                        SourceType = SourceType.Local,
                        SourceEnable = true,
                        SourceContent = StaticInfo.DefaultHosts
                    }
                };
                var json = JsonConvert.SerializeObject(SourceList);
                File.WriteAllText(dataFilePath, json);
            }
        }

        private void InitializeList()
        {
            var json = File.ReadAllText(dataFilePath);
            try
            {
                SourceList = JsonConvert.DeserializeObject<ObservableCollection<Source>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }

            #region test data
            //this.SourceList = new ObservableCollection<Source>
            //{
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "本地源1",
            //        SourceType = SourceType.Local,
            //        SourceEnable = true,
            //        SourceContent = "127.0.0.1 website.com"
            //    },
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "网络源1",
            //        SourceType = SourceType.Web,
            //        SourceEnable = true,
            //        SourceUrl = "www.baidu.com"
            //    },
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "本地源2",
            //        SourceType = SourceType.Local,
            //        SourceEnable = false,
            //        SourceContent = "127.0.0.1 website.com"
            //    },
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "网络源2",
            //        SourceType = SourceType.Web,
            //        SourceEnable = false,
            //        SourceUrl = "www.google.com"
            //    }
            //};
            #endregion
        }

        private void AddItem()
        {
            var source = new Source()
            {
                SourceGuid = Guid.NewGuid(),
                SourceTitle = "本地源Localhost",
                SourceType = SourceType.Local,
                SourceEnable = false
            };
            this.SourceList.Add(source);
            this.SelectedItem = source;
        }

        private void RemoveItem()
        {
            if (SelectedItem != null)
            {
                this.SourceList.Remove(SelectedItem);
                this.SelectedItem = this.SourceList.Count > 0 ? this.SourceList[0] : null;
            }
            if (this.SourceList.Count == 0)
            {
                AddItem();
            }
        }

        private void MoveUp()
        {
            if (this.SourceList != null && this.SelectedItem != null)
            {
                var index = this.SourceList.IndexOf(SelectedItem);
                if (index != 0)
                {
                    this.SourceList.Move(index, index - 1);
                }
            }
        }

        private void MoveDown()
        {
            if (this.SourceList != null && this.SelectedItem != null)
            {
                var index = this.SourceList.IndexOf(SelectedItem);
                if (index != this.SourceList.Count - 1)
                {
                    SourceList.Move(index, index + 1);
                }
            }
        }

        private void SaveChanges()
        {
            if (!File.Exists(dataFilePath))
            {
                File.Create(dataFilePath).Close();
            }
            var json = JsonConvert.SerializeObject(SourceList);
            File.WriteAllText(dataFilePath, json);
            MessageQueue.Enqueue("保存成功");
        }

        private async void UpdateHosts()
        {
            String hosts = String.Empty;
            foreach (var source in SourceList)
            {
                if (source.SourceEnable != true)
                    continue;
                try
                {
                    var data = source.SourceType == SourceType.Local ?
                                                    source.SourceContent :
                                                    await Utilities.GetStringAsync(source.SourceUrl);
                    Utilities.MixHosts(ref hosts, data, source.SourceTitle);
                }
                catch (Exception)
                {
                    MessageQueue.Enqueue($"源 \"{source.SourceTitle}\" 内容获取失败，将略过");
                    continue;
                }

            }
            File.WriteAllText(StaticInfo.HostsPath, hosts);
            Utilities.FlushDNS();
            MessageQueue.Enqueue("更新成功");
        }

        private void ShowModifyHosts()
        {
            new ModifyWindow()
            {
                Owner = Application.Current.MainWindow
            }.ShowDialog();
        }
    }
}
