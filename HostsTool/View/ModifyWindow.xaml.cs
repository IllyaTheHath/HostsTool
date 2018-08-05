using System.Windows;

using HostsTool.ViewModel;

namespace HostsTool.View
{
    /// <summary>
    /// ModifyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyWindow : Window
    {
        public ModifyWindow()
        {
            InitializeComponent();
            this.DataContext = new ModifyViewModel();
        }
    }
}
