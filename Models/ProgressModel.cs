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

		private PingCore PingCore;
		private int PingTotalCount;

		public void SetParameter ( PingCore pingCore, int pingTotalCount) {
			PingCore = pingCore;
			PingTotalCount = pingTotalCount;
		}

		public void UpdateProgressBarThread () {
			double tmpValue;
			do {
				tmpValue = PingCore.totalCount / (double) PingTotalCount * 100;
				while ( tmpValue.CompareTo(ProgressValue)==1 ) {
					ProgressValue++;
					System.Threading.Thread.Sleep (50);
				}

				System.Diagnostics.Debug.WriteLine ("bar" + PingCore.totalCount + "____" + PingTotalCount);
				System.Threading.Thread.Sleep (1000);
			} while ( ProgressValue < 100);
		}
	}
}
