using System.ComponentModel;


namespace CsPingWPF.Models {
	public class ProgressModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged ( string propertyName ) {
			PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
		}

		private double _progressValue = 0;

		public double ProgressValue {
			get {
				return _progressValue;
			}
			set {
				_progressValue = value;
				NotifyPropertyChanged ("ProgressValue");
			}
		}
		//是否正在进行进度条更新，用于辅助判断按钮的CanExecute事件
		public bool Progressing = false;

		private PingCore PingCore;
		private int PingTotalCount;

		public void SetParameter ( PingCore pingCore, int pingTotalCount ) {
			PingCore = pingCore;
			PingTotalCount = pingTotalCount;
		}
		/// <summary>
		/// 线程中的方法，更新进度条，须先设置参数SetParameter
		/// </summary>
		public void UpdateProgressBarThread () {
			//判断是否已经设置了参数
			if ( PingCore != null ) {
				Progressing = true;
				double tmpValue;
				do {
					tmpValue = PingCore.totalCount / (double) PingTotalCount * 100;
					while ( tmpValue.CompareTo (ProgressValue) == 1 ) {
						ProgressValue++;
						System.Threading.Thread.Sleep (50);
					}

					System.Diagnostics.Debug.WriteLine ("bar" + PingCore.totalCount + "____" + PingTotalCount);
					System.Threading.Thread.Sleep (1000);
				} while ( ProgressValue < 100 );
				Progressing = false;
			}
		}
	}
}
