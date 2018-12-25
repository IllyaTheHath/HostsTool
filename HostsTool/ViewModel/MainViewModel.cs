using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using HostsTool.Command;
using HostsTool.Model;
using HostsTool.Util;
using HostsTool.View;

namespace HostsTool.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Source> _sourceList;
        public ObservableCollection<Source> Sourcelist
        {
            get
            {
                return this._sourceList;
            }
            set
            {
                this._sourceList = value;
                OnPropertyChanged(nameof(Sourcelist));
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
            InitDatabase();
            InitializeList();

            if (this.Sourcelist.Count == 0)
            {
                AddItem();
            }

            this.SelectedItem = this.Sourcelist[0];
        }

        private void InitDatabase()
        {
            SQLiteHelper.SetConnection();
            if (!SQLiteHelper.CheckDbExist(@"data.db"))
            {
                SQLiteHelper.CreateDB();
                CreateTable();
            }
            SQLiteHelper.DisposeConnection();
        }

        private void CreateTable()
        {
            String cmdText = "CREATE TABLE IF NOT EXISTS 'source' (" +
                "sourceGuid    TEXT PRIMARY KEY NOT NULL," +
                "sourceTitle   TEXT," +
                "sourceType    INTEGER," +
                "sourceUrl     TEXT," +
                "sourceEnable  INTEGER," +
                "sourceContent TEXT" +
                ")";
            SQLiteHelper.ExecuteNonQuery(cmdText);

            var source = new Source()
            {
                SourceGuid = Guid.NewGuid(),
                SourceTitle = "Localhost",
                SourceType = SourceType.Local,
                SourceEnable = true,
                SourceUrl = null,
                SourceContent = StaticInfo.DefaultHosts
            };
            String cmdTextl = "INSERT INTO 'source' " +
                "(sourceGuid,sourceTitle,sourceType,sourceUrl,sourceEnable,sourceContent) " +
                "VALUES " +
                "(@sourceGuid,@sourceTitle,@sourceType,@sourceUrl,@sourceEnable,@sourceContent)";
            SQLiteHelper.ExecuteNonQuery(cmdTextl,
                SQLiteHelper.CreateParameter("sourceGuid", DbType.String, source.SourceGuid),
                SQLiteHelper.CreateParameter("sourceTitle", DbType.String, source.SourceTitle),
                SQLiteHelper.CreateParameter("sourceType", DbType.Int32, source.SourceType),
                SQLiteHelper.CreateParameter("sourceUrl", DbType.String, source.SourceUrl),
                SQLiteHelper.CreateParameter("sourceEnable", DbType.Int32, source.SourceEnable),
                SQLiteHelper.CreateParameter("sourceContent", DbType.String, source.SourceContent));
        }

        private void InitializeList()
        {
            SQLiteHelper.SetConnection();
            String cmdText = "SELECT * FROM 'source'";
            var items = SQLiteHelper.ExecuteReader<Source>(cmdText);
            SQLiteHelper.DisposeConnection();

            Sourcelist = new ObservableCollection<Source>(items);
            if (Sourcelist != null && Sourcelist.Count > 0)
            {
                this.SelectedItem = Sourcelist[0];
            }

            #region test data
            //this.Sourcelist = new ObservableCollection<Source>
            //{
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "本地源1",
            //        SourceType = SourceType.Local,
            //        SourceEnable = true,
            //        SourceUrl = String.Empty,
            //        SourceContent = "127.0.0.1 website.com"
            //    },
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "网络源1",
            //        SourceType = SourceType.Web,
            //        SourceEnable = true,
            //        SourceUrl = "www.baidu.com",
            //        SourceContent = String.Empty
            //    },
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "本地源2",
            //        SourceType = SourceType.Local,
            //        SourceEnable = false,
            //        SourceUrl = String.Empty,
            //        SourceContent = "127.0.0.1 website.com"
            //    },
            //    new Source()
            //    {
            //        SourceGuid = Guid.NewGuid(),
            //        SourceTitle = "网络源2",
            //        SourceType = SourceType.Web,
            //        SourceEnable = false,
            //        SourceUrl = "www.google.com",
            //        SourceContent = String.Empty
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
                SourceEnable = false,
                SourceUrl = null,
                SourceContent = "127.0.0.1 localhost"
            };
            this.Sourcelist.Add(source);
            this.SelectedItem = source;
        }

        private void RemoveItem()
        {
            SQLiteHelper.SetConnection();
            String cmdText = $"DELETE FROM 'source' WHERE sourceGuid='{SelectedItem.SourceGuid}'";
            SQLiteHelper.ExecuteNonQuery(cmdText);
            SQLiteHelper.DisposeConnection();

            if (SelectedItem != null)
            {
                this.Sourcelist.Remove(SelectedItem);
                this.SelectedItem = this.Sourcelist.Count > 0 ? this.Sourcelist[0] : null;
            }
            if (this.Sourcelist.Count == 0)
            {
                AddItem();
            }
        }

        private void MoveUp()
        {
            if (this.Sourcelist != null && this.SelectedItem != null)
            {
                var index = this.Sourcelist.IndexOf(SelectedItem);
                if (index != 0)
                {
                    this.Sourcelist.Move(index, index - 1);
                }
            }
        }

        private void MoveDown()
        {
            if (this.Sourcelist != null && this.SelectedItem != null)
            {
                var index = this.Sourcelist.IndexOf(SelectedItem);
                if (index != this.Sourcelist.Count - 1)
                {
                    Sourcelist.Move(index, index + 1);
                }
            }
        }

        private void SaveChanges()
        {
            SQLiteHelper.SetConnection();
            String cmdText = "REPLACE INTO 'source'" +
                "(sourceGuid,sourceTitle,sourceType,sourceUrl,sourceEnable,sourceContent) " +
                "VALUES" +
                "(@sourceGuid,@sourceTitle,@sourceType,@sourceUrl,@sourceEnable,@sourceContent)";

            foreach (var item in Sourcelist)
            {
                if (item.SourceType == SourceType.Local)
                    item.SourceUrl = null;
                else if (item.SourceType == SourceType.Web)
                    item.SourceContent = null;
                SQLiteHelper.ExecuteNonQuery(cmdText,
                    SQLiteHelper.CreateParameter("sourceGuid", DbType.String, item.SourceGuid),
                    SQLiteHelper.CreateParameter("sourceTitle", DbType.String, item.SourceTitle),
                    SQLiteHelper.CreateParameter("sourceType", DbType.Int32, item.SourceType),
                    SQLiteHelper.CreateParameter("sourceUrl", DbType.String, item.SourceUrl),
                    SQLiteHelper.CreateParameter("sourceEnable", DbType.Int32, item.SourceEnable),
                    SQLiteHelper.CreateParameter("sourceContent", DbType.String, item.SourceContent));
            }
            SQLiteHelper.DisposeConnection();
        }

        private async void UpdateHosts()
        {
            String hosts = String.Empty;
            foreach (var source in Sourcelist)
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
                    MessageBox.Show(String.Format($"源 \"{source.SourceTitle}\" 内容获取失败，将略过"));
                    continue;
                }

            }
            File.WriteAllText(StaticInfo.HostsPath, hosts);
            Utilities.FlushDNS();
            MessageBox.Show("更新成功！", "Hosts Tool");
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
