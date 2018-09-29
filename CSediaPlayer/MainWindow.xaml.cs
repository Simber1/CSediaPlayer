using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 

	public partial class MainWindow : Window
	{

		double volume;
		double prevVolume;
        bool ProgressSliderChanging = false;
		string path;
		Thread LoopingThread;


		WMPLib.WindowsMediaPlayer player;
		public MainWindow()
		{
			InitializeComponent();


			player = new WMPLib.WindowsMediaPlayer();

			volume = 25;
			player.settings.volume = Convert.ToInt32(volume);
			player.settings.autoStart = true;
			player.uiMode = "full";
			VolumeUpdate();
			SongSelect();
			LoopingThread = new Thread(Looper);
			LoopingThread.Start();

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
				Console.WriteLine(path);
				player.URL = path;
				Current_Playing.Text = player.currentMedia.name;
            }
		}

		
		private void Looper()
		{
			while (true)
			{
				System.Threading.Thread.Sleep(50);

				this.Dispatcher.Invoke(() =>
				{
					if (player.currentMedia.duration > 1 && player.controls.currentPosition > 1 && !ProgressSliderChanging)
					{
					
						UpdateSongPosition();
					
					}
				});

			}
		}

		private void UpdateSongPosition()
		{
			Progress_Slider.Value = player.controls.currentPosition;
			Current_Time.Text = player.controls.currentPositionString;
			Progress_Slider.Maximum = player.currentMedia.duration;
			Max_Time.Text = player.currentMedia.durationString;
		}


		private void VolumeUpdate()
		{
			player.settings.volume = Convert.ToInt32(volume); //Updating the players volume 
			Volume_Slider.Value = volume;  //Setting the sliders pos = to the volume
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

		private void Mute(object sender, RoutedEventArgs e)
		{
			if (Mute_Toggle.IsChecked == true)
			{
				prevVolume = volume;  //muting
				volume = 0;
				Mute_Toggle.Content = "Unmute";
			}
			else
			{
				volume = prevVolume; //unmuting
				Mute_Toggle.Content = "Mute";
			}

			VolumeUpdate();
		}

		private void VolumeSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
		{ 
			volume = Volume_Slider.Value;
			VolumeUpdate();
		}

		private void temp(object sender, RoutedEventArgs e)
		{
			UpdateSongPosition();
		}

		private void Close(object sender, System.ComponentModel.CancelEventArgs e)
		{
			LoopingThread.Abort();
		}

        private void Progress_Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            ProgressSliderChanging = true;
        }

        private void Progress_Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            ProgressSliderChanging = false;
        }
    }
}


