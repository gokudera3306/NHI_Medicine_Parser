using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NHI_Medicine_Parser.Class;

namespace NHI_Medicine_Parser
{
    /// <summary>
    /// MessageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MessageWindow : Window
    {
        private MessageWindow(string message, MessageType type)
        {
            InitializeComponent();
            switch (type)
            {
                case MessageType.ERROR:
                    Icon.Source = new BitmapImage(new Uri(@"Images\Error.png", UriKind.Relative));
                    break;
                case MessageType.SUCCESS:
                    Icon.Source = new BitmapImage(new Uri(@"Images\Success.png", UriKind.Relative));
                    break;
            }
            Message.Text = message;

            OkButton.Focus();
        }

        public static void ShowMessage(string message, MessageType type)
        {
            MessageWindow messageWindow = new MessageWindow(message, type);
            messageWindow.ShowDialog();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Close();
            }
        }
    }
}
