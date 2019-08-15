using System;
using System.Diagnostics;
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
        double LastProgressSliderThumbDeltaValue = 0;
        Stopwatch LastProgressSliderDeltaEvent = new Stopwatch();
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

			if(player.controls.currentPosition == 0){
				Current_Time.Text = "00:00";
			}
			else{
				Current_Time.Text = player.controls.currentPositionString;
			}

			Progress_Slider.Maximum = player.currentMedia.duration;
			Max_Time.Text = player.currentMedia.durationString;
		}


		private void VolumeUpdate()
		{
			if (Mute_Toggle.IsChecked == false)
			{
				player.settings.volume = Convert.ToInt32(volume); //Updating the players volume 
				Volume_Slider.Value = volume;  //Setting the sliders pos = to the volume
			}
			else 
			{
				player.settings.volume = Convert.ToInt32(0);
			}
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
			UpdateSongPosition();
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
				if (prevVolume == Volume_Slider.Value){
					volume = prevVolume; //unmuting
				}

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
            double NewPosition = Progress_Slider.Value;
            player.controls.currentPosition = NewPosition;
            LastProgressSliderThumbDeltaValue = NewPosition;
            LastProgressSliderDeltaEvent.Start();
        }

        private void Progress_Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            ProgressSliderChanging = false;
            double NewPosition = Progress_Slider.Value;
            player.controls.currentPosition = NewPosition;
            LastProgressSliderThumbDeltaValue = NewPosition;
            LastProgressSliderDeltaEvent.Stop();
        }

        private void Progress_Slider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //DragDelta runs very often - to prevent lag we throw most of these events out
            if (LastProgressSliderDeltaEvent.ElapsedMilliseconds<300)
            {
                return;
            }

            double NewPosition = Progress_Slider.Value;
            if (Math.Abs(LastProgressSliderThumbDeltaValue-NewPosition) > 1)
            {
                player.controls.currentPosition = NewPosition;
                LastProgressSliderThumbDeltaValue = NewPosition;
                LastProgressSliderDeltaEvent.Restart();
            }
        }
    }
}


