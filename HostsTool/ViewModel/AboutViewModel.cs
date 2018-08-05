using System;

using HostsTool.Data;
using HostsTool.Util;

namespace HostsTool.ViewModel
{
    class AboutViewModel : ViewModelBase
    {
        private String _version;
        public String Version
        {
            get
            {
                return this._version;
            }
            set
            {
                this._version = value;
                OnPropertyChanged("Version");
            }
        }

        public AboutViewModel()
        {
            this.Version =String.Format("版本：{0}",SharedInfo.Version);
            Utilities.CheckUpdateAsync();
        }

    }
}
