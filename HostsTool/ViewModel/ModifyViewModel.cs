using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using HostsTool.Util;

using MaterialDesignThemes.Wpf;

using Microsoft.Win32;

using Stylet;

namespace HostsTool.ViewModel
{
    public class ModifyViewModel : Screen
    {
        private readonly String _hostsTextBackup;

        public String HostsText { get; set; }

        public SnackbarMessageQueue MessageQueue { get; set; }

        public ModifyViewModel()
        {
            HostsText = File.ReadAllText(StaticInfo.HostsPath);
            this._hostsTextBackup = HostsText;
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(200));
        }

        public void SaveChanges()
        {
            File.WriteAllText(StaticInfo.HostsPath, HostsText);
            Utilities.FlushDNS();
            MessageQueue.Enqueue("保存成功");
        }

        public void CancelChanges()
        {
            HostsText = this._hostsTextBackup;
            MessageQueue.Enqueue("撤销成功");
        }

        public void RestoreDefault()
        {
            HostsText = StaticInfo.DefaultHosts;
            File.WriteAllText(StaticInfo.HostsPath, HostsText);
            Utilities.FlushDNS();
            MessageQueue.Enqueue("恢复默认成功");
        }

        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件(*.txt)|*.txt";
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, HostsText);
                MessageQueue.Enqueue("保存成功");
            }
        }

        public void LoadFrom()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                HostsText = File.ReadAllText(ofd.FileName);
                MessageQueue.Enqueue("导入成功");
            }
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
            this.RequestClose();
            //((Window)View).Close();
        }
    }
}