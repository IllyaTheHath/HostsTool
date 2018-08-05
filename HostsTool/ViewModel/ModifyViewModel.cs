using System;
using System.IO;
using System.Windows.Input;

using Microsoft.Win32;

using HostsTool.Command;
using HostsTool.Data;
using HostsTool.Util;

namespace HostsTool.ViewModel
{
    class ModifyViewModel : ViewModelBase
    {
        private String _hostsTextBackup;

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
                OnPropertyChanged("HostsText");
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

        private ICommand _restoreCommand;
        public ICommand RestoreCommand
        {
            get
            {
                if (_restoreCommand == null)
                {
                    _restoreCommand = new DelegateCommand(
                        param => this.Restore()
                    );
                }
                return _restoreCommand;
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

        private ICommand _readFromCommand;
        public ICommand ReadFromCommand
        {
            get
            {
                if (_readFromCommand == null)
                {
                    _readFromCommand = new DelegateCommand(
                        param => this.ReadFrom()
                    );
                }
                return _readFromCommand;
            }
        }

        public ModifyViewModel()
        {
            this.HostsText = File.ReadAllText(SharedInfo.HostsPath);
            this._hostsTextBackup = this.HostsText;
        }

        private void SaveChanges()
        {
            File.WriteAllText(SharedInfo.HostsPath, this.HostsText);
            Utilities.FlushDNS();
        }

        private void CancelChanges()
        {
            this.HostsText = this._hostsTextBackup;
        }

        private void Restore()
        {
            this.HostsText = SharedInfo.DefaultHosts;
            File.WriteAllText(SharedInfo.HostsPath, this.HostsText);
            Utilities.FlushDNS();
        }

        private void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件(*.txt)|*.txt";
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, this.HostsText);
            }
        }

        private void ReadFrom()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                this.HostsText = File.ReadAllText(ofd.FileName);
            }
        }
        
    }
}

