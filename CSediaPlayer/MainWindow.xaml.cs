using System;
using System.Windows;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        string path;
        WMPLib.WindowsMediaPlayer player;

        public MainWindow()
        {
            player = new WMPLib.WindowsMediaPlayer();

            player.settings.volume = 25;
            player.settings.autoStart = true;

            InitializeComponent();

            SongSelect();
        }

        private void SongSelect()
        {
            
            path = null;
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Music (*.mp3)|*.mp3;*.m4a;*.wav|All Files (*.*)|*.*";
            file.FilterIndex = 1;
            file.Multiselect = false;

            if (file.ShowDialog() == true)
            {
                path = file.FileName;
            }
            Console.WriteLine(path);
            player.URL = path;
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            player.controls.pause();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            player.controls.play();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            player.controls.stop();
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            SongSelect();
        }

    }



}
