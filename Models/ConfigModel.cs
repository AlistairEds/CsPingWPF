using System.ComponentModel;


namespace CsPingWPF.Models {
	public class ConfigModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged ( string propertyName ) {
			PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
		}

		public bool isConfigRight = true;

		private void CheckIpValue ( string value, ref string PropertyName, string PropertyChangeName ) {
			if ( int.TryParse (value, out int outValue) ) {
				if ( outValue > 0 && outValue < 255 ) {
					PropertyName = value;
					isConfigRight = true;
					NotifyPropertyChanged (PropertyChangeName);
				}
			} else {
				isConfigRight = false;
			}
		}
		//104.17.10.1 中的104
		private string _ipTextBoxA = "104";
		/// <summary>
		/// 104.17.10.1 中的104
		/// </summary>
		public string IpTextBoxA {
			get {
				return _ipTextBoxA;
			}
			set {
				CheckIpValue (value, ref _ipTextBoxA, "IpTextBoxA");
			}
		}
		//104.17.10.1 中的17
		private string _ipTextBoxB = "17";
		/// <summary>
		/// 104.17.10.1 中的17
		/// </summary>
		public string IpTextBoxB {
			get {
				return _ipTextBoxB;
			}
			set {
				CheckIpValue (value, ref _ipTextBoxB, "IpTextBoxB");
			}
		}
		//104.17.10.1 中的10
		private string _ipTextBoxC = "1";
		/// <summary>
		/// 104.17.10.1 中的10
		/// </summary>
		public string IpTextBoxC {
			get {
				return _ipTextBoxC;
			}
			set {
				CheckIpValue (value, ref _ipTextBoxC, "IpTextBoxC");
			}
		}
		//104.17.10.1 到 104.17.12.1 中的12
		private string _ipTextBoxD = "2";
		/// <summary>
		/// 104.17.10.1 到 104.17.12.1 中的12
		/// </summary>
		public string IpTextBoxD {
			get {
				return _ipTextBoxD;
			}
			set {
				if ( int.TryParse (value, out int outValue) ) {
					if ( outValue >= int.Parse (_ipTextBoxC) && outValue < 255 ) {
						_ipTextBoxD = value;
						isConfigRight = true;
						NotifyPropertyChanged ("IpTextBoxD");
					}
				} else {
					isConfigRight = false;
				}
			}
		}




		private string _maxFailedCountText = "1";

		public string MaxFailedCountText {
			get {
				return _maxFailedCountText;
			}
			set {
				if ( int.TryParse (value, out int outValue) ) {
					if ( outValue >= 0 && outValue < 50 ) {
						_maxFailedCountText = value;
						NotifyPropertyChanged ("MaxFailedCountText");
						isConfigRight = true;
					} else {
						isConfigRight = false;
					}
				}
			}
		}

		private string _pingIntervalText = "500";

		public string PingIntervalText {
			get {
				return _pingIntervalText;
			}
			set {
				if ( int.TryParse (value, out int outValue) ) {
					if ( outValue > 0 && outValue < 5000) {
						_pingIntervalText = value;
						isConfigRight = true;
						NotifyPropertyChanged ("PingIntervalText");
					} else {
						isConfigRight = false;
					}
				}	

			}
		}

		private string _pingTimesText = "50";

		public string PingTimesText {
			get {
				return _pingTimesText;
			}
			set {
				if ( int.TryParse (value, out int outValue) ) {
					if ( outValue > 0 && outValue < 200) {
						_pingTimesText = value;
						isConfigRight = true;
						NotifyPropertyChanged ("PingTimesText");
					} else {
						isConfigRight = false;
					}
				}
					
			}
		}

		private string _minThreadText = "255";

		public string MinThreadText {
			get {
				return _minThreadText;
			}
			set {
				if ( int.TryParse (value, out int outValue) ) {
					if ( outValue > 0 && outValue < 1000) {
						_minThreadText = value;
						isConfigRight = true;
						NotifyPropertyChanged ("MinThreadText");
					} else {
						isConfigRight = false;
					}
				}
					
			}
		}






	}
}
