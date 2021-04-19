using System.ComponentModel;


namespace CsPingWPF.Models {
	public class DataModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged ( string propertyName ) {
			PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
		}

		private string _ip;
		private int _rtt;
		private int _maxRTT;
		private int _minRTT;
		private int _averRTT;
		private int _succeedCount;
		private int _failedCount;

		public string IP {
			get {
				return _ip;
			}
			set {
				_ip = value;
				NotifyPropertyChanged ("IP");
			}
		}

		public int RTT {
			get {
				return _rtt;
			}
			set {
				_rtt = value;
				NotifyPropertyChanged ("RTT");
			}
		}

		public int MaxRTT {
			get {
				return _maxRTT;
			}
			set {
				_maxRTT = value;
				NotifyPropertyChanged ("MaxRTT");
			}
		}

		public int MinRTT {
			get {
				return _minRTT;
			}
			set {
				_minRTT = value;
				NotifyPropertyChanged ("MinRTT");
			}
		}

		public int AverRTT {
			get {
				return _averRTT;
			}
			set {
				_averRTT = value;
				NotifyPropertyChanged ("AverRTT");
			}
		}

		public int SucceedCount {
			get {
				return _succeedCount;
			}
			set {
				_succeedCount = value;
				NotifyPropertyChanged ("SucceedCount");
			}
		}

		public int FailedCount {
			get {
				return _failedCount;
			}
			set {
				_failedCount = value;
				NotifyPropertyChanged ("FailedCount");
			}
		}

	}
}
