using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using HostsTool.Model;
using HostsTool.Util;

using MaterialDesignThemes.Wpf;

using Newtonsoft.Json;

using Stylet;

namespace HostsTool.ViewModel
{
    public class MainViewModel : Screen
    {
        private const String dataFilePath = "hosts.json";

        private readonly IWindowManager _windowManager;

        public BindableCollection<Source> SourceList { get; set; }

        public Source SelectedItem { get; set; }

        public SnackbarMessageQueue MessageQueue { get; set; }

        public MainViewModel(IWindowManager windowManager)
        {
            this._windowManager = windowManager;

            InitDefaultHosts();
            InitializeList();

            if (SourceList.Count == 0)
            {
                AddItem();
            }

            SelectedItem = SourceList[0];
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(200));
        }

        private void InitDefaultHosts()
        {
            String data = String.Empty;
            if (File.Exists(dataFilePath))
            {
                data = File.ReadAllText(dataFilePath);
            }

            if (String.IsNullOrWhiteSpace(data))
            {
                SourceList = new BindableCollection<Source>
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
                SourceList = JsonConvert.DeserializeObject<BindableCollection<Source>>(json);
            }
            catch (Exception ex)
            {
                this._windowManager.ShowMessageBox(ex.ToString());
                RequestClose();
            }

            #region test data

            //this.SourceList = new BindableCollection<Source>
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

            #endregion test data
        }

        public void AddItem()
        {
            var source = new Source()
            {
                SourceGuid = Guid.NewGuid(),
                SourceTitle = "本地源Localhost",
                SourceType = SourceType.Local,
                SourceEnable = false
            };
            SourceList.Add(source);
            SelectedItem = source;
        }

        public void RemoveItem()
        {
            if (SelectedItem != null)
            {
                SourceList.Remove(SelectedItem);
                SelectedItem = SourceList.Count > 0 ? SourceList[0] : null;
            }
            if (SourceList.Count == 0)
            {
                AddItem();
            }
        }

        public void MoveUp()
        {
            if (SourceList != null && SelectedItem != null)
            {
                var index = SourceList.IndexOf(SelectedItem);
                if (index != 0)
                {
                    SourceList.Move(index, index - 1);
                }
            }
        }

        public void MoveDown()
        {
            if (SourceList != null && SelectedItem != null)
            {
                var index = SourceList.IndexOf(SelectedItem);
                if (index != SourceList.Count - 1)
                {
                    SourceList.Move(index, index + 1);
                }
            }
        }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(SourceList);
            File.WriteAllText(dataFilePath, json);
            MessageQueue.Enqueue("保存成功");
        }

        public async void UpdateHosts()
        {
            var json = JsonConvert.SerializeObject(SourceList);
            File.WriteAllText(dataFilePath, json);
            String hosts = String.Empty;
            foreach (var source in SourceList)
            {
                if (source.SourceEnable != true)
                {
                    continue;
                }

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

        public void ShowModifyHosts()
        {
            //new ModifyWindow()
            //{
            //    Owner = Application.Current.MainWindow
            //}.ShowDialog();
            var viewModel = new ModifyViewModel();
            this._windowManager.ShowDialog(viewModel);
        }

        public void AppBar_MouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((Window)View).DragMove();
            }
        }

        public void Close()
        {
            RequestClose();
            //((Window)this.View).Close();
        }

        public void Minus()
        {
            if (((Window)View).WindowState != WindowState.Minimized)
            {
                ((Window)View).WindowState = WindowState.Minimized;
            }
        }
    }
}