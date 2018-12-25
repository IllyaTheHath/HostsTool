using System;
using System.Windows;
using System.Windows.Input;
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

        private void Close(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AppBar_MouseMove(Object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
