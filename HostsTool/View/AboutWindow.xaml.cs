using System.Windows;

using HostsTool.ViewModel;

namespace HostsTool.View
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.DataContext = new AboutViewModel();
        }
    }
}
