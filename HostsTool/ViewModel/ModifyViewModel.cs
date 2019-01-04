﻿using System;
using System.IO;
using System.Windows.Input;
using HostsTool.Command;
using HostsTool.Util;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace HostsTool.ViewModel
{
    public class ModifyViewModel : ViewModelBase
    {
        private readonly String _hostsTextBackup;

        private String _hostsText;
        public String HostsText
        {
            get
            {
                return this._hostsText;
            }
            set
            {
                this._hostsText = value;
                OnPropertyChanged(nameof(HostsText));
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

        private ICommand _cancelChangesCommand;
        public ICommand CancelChangesCommand
        {
            get
            {
                if (_cancelChangesCommand == null)
                {
                    _cancelChangesCommand = new DelegateCommand(
                        param => this.CancelChanges()
                    );
                }
                return _cancelChangesCommand;
            }
        }

        private ICommand _restoreDefaultCommand;
        public ICommand RestoreDefaultCommand
        {
            get
            {
                if (_restoreDefaultCommand == null)
                {
                    _restoreDefaultCommand = new DelegateCommand(
                        param => this.RestoreDefault()
                    );
                }
                return _restoreDefaultCommand;
            }
        }

        private ICommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new DelegateCommand(
                        param => this.SaveAs()
                    );
                }
                return _saveAsCommand;
            }
        }

        private ICommand _loadFromCommand;
        public ICommand LoadFromCommand
        {
            get
            {
                if (_loadFromCommand == null)
                {
                    _loadFromCommand = new DelegateCommand(
                        param => this.LoadFrom()
                    );
                }
                return _loadFromCommand;
            }
        }

        public ModifyViewModel()
        {
            this.HostsText = File.ReadAllText(StaticInfo.HostsPath);
            this._hostsTextBackup = this.HostsText;
            this.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(500));
        }

        private void SaveChanges()
        {
            File.WriteAllText(StaticInfo.HostsPath, this.HostsText);
            Utilities.FlushDNS();
            MessageQueue.Enqueue("保存成功");
        }

        private void CancelChanges()
        {
            this.HostsText = this._hostsTextBackup;
            MessageQueue.Enqueue("撤销成功");
        }

        private void RestoreDefault()
        {
            this.HostsText = StaticInfo.DefaultHosts;
            File.WriteAllText(StaticInfo.HostsPath, this.HostsText);
            Utilities.FlushDNS();
            MessageQueue.Enqueue("恢复默认成功");
        }

        private void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件(*.txt)|*.txt";
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, this.HostsText);
                MessageQueue.Enqueue("保存成功");
            }
        }

        private void LoadFrom()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                this.HostsText = File.ReadAllText(ofd.FileName);
                MessageQueue.Enqueue("导入成功");
            }
        }
    }
}
