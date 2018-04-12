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
		string path;
		bool FirstRun =true;
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


			//DelayAction(3000, TimerLoop);




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
			Current_Playing.Text = player.currentMedia.name;


		}

		
		private void Looper()
		{
			if (FirstRun)
			{
				Thread.Sleep(50);
				FirstRun = false;
			}
			while (true)
			{
				System.Threading.Thread.Sleep(100);
				
				
				if (player.currentMedia.duration > 1 && player.controls.currentPosition > 1)
				{
					this.Dispatcher.Invoke(() =>
					{
						UpdateSongPosition();
					});
				}
			
				
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

		public static void DelayAction(int millisecond, Action action)
		{
			var timer = new DispatcherTimer();
			timer.Tick += delegate

			{
				action.Invoke();
				timer.Stop();
			};

			timer.Interval = TimeSpan.FromMilliseconds(millisecond);
			timer.Start();
		}

		private void temp(object sender, RoutedEventArgs e)
		{
			UpdateSongPosition();
		}

		private void Close(object sender, System.ComponentModel.CancelEventArgs e)
		{
			LoopingThread.Abort();
		}
	}
}


