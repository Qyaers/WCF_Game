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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatClient.ServiceChat;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        int timer = 5;
        public static bool flag = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bGameSS.Visibility = Visibility.Hidden;
            bRock.Visibility = Visibility.Hidden;
            bPaper.Visibility = Visibility.Hidden;
            bScissors.Visibility = Visibility.Hidden;
        }

        void ConnectUser()
        {
            try {
                if (!isConnected)
                {
                    client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                    ID = client.Connect(tbUserName.Text);
                    tbUserName.IsEnabled = false;
                    tbMessage.IsEnabled = true;
                    bConnDicon.Content = "Disconnect";
                    isConnected = true;
                    bGameSS.Visibility = Visibility.Visible;

                }
            }
            catch (Exception)
            {
                lbChat.Items.Add("Error connection to server, check server settings and if it is server enabled");
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                bGameSS.Visibility = Visibility.Hidden;
                client.Disconnect(ID);
                client = null;
                tbUserName.IsEnabled = true;
                tbMessage.IsEnabled = false;
                bConnDicon.Content = "Connect";
                isConnected = false;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();

            }
            else
            {
                bGameSS.Visibility = Visibility.Visible;
                ConnectUser();
            }

        }
        //---------------------------call back functions/methods----------------------------------------------------------------
        public void MsgCallback(string msg)
        {
            try{ 
                lbChat.Items.Add(msg);
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
            }
            catch (Exception)
            {
                lbChat.Items.Add("Error in MsgCallback");
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
            }
        }

        public void GameCallBack(string msg)
        {
            try
            { 
                lbChat.Items.Add(msg);
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);

                bGameSS.IsEnabled = false;

                bRock.Visibility = Visibility.Visible;
                bPaper.Visibility = Visibility.Visible;
                bScissors.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                lbChat.Items.Add("Error in GameCallBack");
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
            }
        }

        public void TimerCallback(string msg,int counter)
        {
            try {
                if(counter >= 0) { 
                lbChat.Items.Add(msg);
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
                    if(counter == 0)
                    {
                        bGameSS.IsEnabled = true;
                        bScissors.Visibility = Visibility.Hidden;
                        bPaper.Visibility = Visibility.Hidden;
                        bRock.Visibility = Visibility.Hidden;
                        // Условие для кнопок, если он прожат
                        if (flag == false)
                        {
                            client.ResultGame("", ID);
                        }
                    }
                }
            }
            catch (Exception)
            {
                lbChat.Items.Add("Error in TimerCallback");
            }
        }

        public void ResultOfGameCallbak(string msg)
        {
            try
            {
                client.PickingWinner();
                lbChat.Items.Add(msg);
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);

            }
            catch (Exception)
            {
                lbChat.Items.Add("Error in ResultOfGameCallbak");
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
            }
        }

        public void PickingWinnerCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);

            bGameSS.IsEnabled = true;
            bRock.Visibility = Visibility.Hidden;
            bPaper.Visibility = Visibility.Hidden;
            bScissors.Visibility = Visibility.Hidden;
            bRock.IsEnabled = true;
            bScissors.IsEnabled = true;
            bPaper.IsEnabled = true;

        }
        //----------------------------------------------------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //DisconnectUser();
            if (isConnected) { 
                client.Disconnect(ID);
            }
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
                    tbMessage.Text = string.Empty;
                }
            }
        }

        private void lbChat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void bRock_Click(object sender, RoutedEventArgs e)
        {
            //Rock сделать отправку в результат и после блокирование кнопок у клиента
            if (client != null)
            {
                flag = true;

                client.SendMsg("Камень", ID);
                client.ResultGame("Камень", ID);

                bScissors.IsEnabled = false;
                bPaper.IsEnabled = false;
                bRock.IsEnabled = false;
            }

        }

        private void bPaper_Click(object sender, RoutedEventArgs e)
        {
            //Paper
            if (client != null)
            {
                flag = true;

                client.SendMsg("Бумага", ID);
                client.ResultGame("Бумага", ID);
                
                bScissors.IsEnabled = false;
                bPaper.IsEnabled = false;
                bRock.IsEnabled = false;
            }
        }

        private void bScissors_Click(object sender, RoutedEventArgs e)
        {
            //Scissors
            if (client != null)
            {
                flag = true;

                // вместо отправки сообщения меняем значение переменной и передаем ее в таймер
                client.SendMsg("Ножницы", ID);
                client.ResultGame("Ножницы", ID);

                bScissors.IsEnabled = false;
                bPaper.IsEnabled = false;
                bRock.IsEnabled = false;

            }
        }

        private void bGameSS_Click(object sender, RoutedEventArgs e)
        {
            try { 
                client.SendStartGame("Игра начнеться через ", ID);
                client.SendTimer(timer);

            }
            catch (Exception)
            {
                lbChat.Items.Add("Error in button Game start");
                lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
            }
        }

        private void tbMessage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
