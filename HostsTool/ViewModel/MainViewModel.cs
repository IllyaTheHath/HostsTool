using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using HostsTool.Command;
using HostsTool.Data;
using HostsTool.Model;
using HostsTool.Util;
using HostsTool.View;

namespace HostsTool.ViewModel
{
    class MainViewModel : ViewModelBase
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
                OnPropertyChanged("SourceList");
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
                OnPropertyChanged("SelectedItem");
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

        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new DelegateCommand(
                        param => this.ShowAbout()
                    );
                }
                return _aboutCommand;
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
                SourceType = SourceTypes.Local,
                SourceEnable = true,
                SourceUrl = null,
                SourceContent = SharedInfo.DefaultHosts
            };
            String cmdTextl = "INSERT INTO 'source'" +
                "(sourceGuid,sourceTitle,sourceType,sourceUrl,sourceEnable,sourceContent) " +
                "VALUES" +
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
            String cmdText = "SELECT * FROM `source`";
            var items = SQLiteHelper.ExecuteReader<Source>(Source.Create, cmdText);
            Sourcelist = new ObservableCollection<Source>(items);
            SQLiteHelper.DisposeConnection();
            if (Sourcelist.Count > 0)
            {
                this.SelectedItem = Sourcelist[0];
            }
        }

        private void AddItem()
        {
            var source = new Source()
            {
                SourceGuid = Guid.NewGuid(),
                SourceTitle = "本地源",
                SourceType = SourceTypes.Local,
                SourceEnable = true,
                SourceUrl = null,
                SourceContent = "127.0.0.1 website.com"
            };
            Sourcelist.Add(source);
        }

        private void RemoveItem()
        {
            SQLiteHelper.SetConnection();
            String cmdText = String.Format("DELETE FROM 'source' WHERE sourceGuid='{0}'",
                SelectedItem.SourceGuid);
            SQLiteHelper.ExecuteNonQuery(cmdText);
            SQLiteHelper.DisposeConnection();

            this.Sourcelist.Remove(SelectedItem);
            if (Sourcelist.Count > 0)
                this.SelectedItem = Sourcelist[0];
            else
                this.SelectedItem = null;
        }

        private void MoveUp()
        {
            if (SelectedItem == Sourcelist.First())
            {
                return;
            }
            Sourcelist.Move(Sourcelist.IndexOf(SelectedItem), Sourcelist.IndexOf(SelectedItem) - 1);
        }

        private void MoveDown()
        {
            if (SelectedItem == Sourcelist.Last())
            {
                return;
            }
            Sourcelist.Move(Sourcelist.IndexOf(SelectedItem), Sourcelist.IndexOf(SelectedItem) + 1);
        }

        private void SaveChanges()
        {
            SQLiteHelper.SetConnection();
            String cmdText = "REPLACE INTO 'source'" +
                "(sourceGuid,sourceTitle,sourceType,sourceUrl,sourceEnable,sourceContent) " +
                "VALUES" +
                "(@sourceGuid,@sourceTitle,@sourceType,@sourceUrl,@sourceEnable,@sourceContent)";
            foreach(var item in Sourcelist)
            { 
                if(item.SourceType == SourceTypes.Local)
                {
                    item.SourceUrl = null;
                }
                else if(item.SourceType == SourceTypes.Web)
                {
                    item.SourceContent = null;
                }
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
                    var piece = source.SourceType == SourceTypes.Local ?
                                                    source.SourceContent :
                                                    await Utilities.GetStringAsync(source.SourceUrl);
                    Utilities.MixHosts(ref hosts, piece, source.SourceTitle);
                }
                catch (Exception)
                {
                    MessageBox.Show(String.Format("源 \"{0}\" 内容获取失败，将略过", source.SourceTitle));
                    continue;
                }
                
            }
            System.IO.File.WriteAllText(SharedInfo.HostsPath, hosts);
            Utilities.FlushDNS();
            MessageBox.Show("更新成功！","Hosts Tool");
        }
        
        private void ShowModifyHosts()
        {
            ModifyWindow mw = new ModifyWindow();
            mw.ShowDialog();
        }

        private void ShowAbout()
        {
            AboutWindow aw = new AboutWindow();
            aw.ShowDialog();
        }
    }
}
