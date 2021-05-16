using System.Text;

namespace CsPingWPF.Models {
	public class PingCore {
		//每个IP需要ping的次数
		private int _pingTimes;
		//两次ping之间的间隔毫秒
		private int _interval;
		//每个IP允许的最多失败次数，超过则不再ping此IP
		private int _maxFailedCount;
		//UI接口
		private UIFunc _uIFunc;
		//已经ping的总数
		public int currentPingCount = 0;
		//终止线程标志
		private bool isAbort = false;

		


		/// <summary>
		/// PingCore类，初始化设置ping的参数
		/// </summary>
		/// <param name="pingTimes">每个IP需要ping的次数</param>
		/// <param name="interval">两次Ping之间的间隔毫秒</param>
		/// <param name="maxFailedCount">每个IP允许的最多超时失败次数，超过则不再ping此IP</param>
		/// <param name="uIFunc">传入一个实现了uiFunc接口的类，用于每次ping结束后接收数据和操作UI</param>
		public PingCore ( int pingTimes, int interval, int maxFailedCount, UIFunc uIFunc ) {
			_pingTimes = pingTimes;
			_interval = interval;
			_maxFailedCount = maxFailedCount;
			_uIFunc = uIFunc;
		}

		public void AbortAll (bool abort = true) {
			isAbort = abort;
		}

		/// <summary>
		/// 实现ping操作的核心方法
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="timeout"></param>
		/// <param name="roundtripTime"></param>
		/// <returns></returns>
		public static bool Ping ( string ip, int timeout, ref long roundtripTime ) {
			System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping ();
			System.Net.NetworkInformation.PingOptions pingOptions = new System.Net.NetworkInformation.PingOptions {
				DontFragment = true
			};
			byte [] buffer = Encoding.ASCII.GetBytes ("Test");
			System.Net.NetworkInformation.PingReply pingReply = p.Send (ip, timeout, buffer, pingOptions);

			if ( pingReply.Status == System.Net.NetworkInformation.IPStatus.Success ) {
				roundtripTime = pingReply.RoundtripTime;

				return true;
			} else {
				return false;
			}
		}

		public interface UIFunc {
			void UIFunction ( string ip, int rtt, int succeedCount, int failedCount, int maxRTT, int minRTT, int averRTT, bool isDele );
		}

		

		/// <summary>
		/// 传入string类型的IP地址，将数据返回到接口UIFunc
		/// </summary>
		/// <param name="ip"></param>
		public void PingEachIP ( object ip ) {
			long roundTripTime = 0;
			int failedCount = 0;
			int succeedCount = 0;
			int maxRTT = 0;
			int minRTT = 0;
			int sumRTT = 0;
			int averRTT = 0;
			string IP = (string) ip;
			bool isDele = false;
			//int currentPingCount = 0;

			for ( int i = 1; i <= _pingTimes && !isAbort; i++ ) {
				if ( Ping (IP, 1000, ref roundTripTime) ) {
					int rtt = (int) roundTripTime;
					succeedCount++;
					sumRTT += rtt;
					if ( rtt > maxRTT )
						maxRTT = (int) roundTripTime;
					if ( minRTT == 0 ) {
						minRTT = rtt;
					} else {
						if ( rtt < minRTT )
							minRTT = rtt;
					}					
					averRTT = sumRTT / succeedCount;
				} else {
					failedCount++;					
					if ( failedCount > _maxFailedCount ) {
						isDele = true;
						//currentPingCount += _pingTimes - succeedCount - failedCount + 1;
						//System.Diagnostics.Debug.WriteLine (ip + "-total:" + currentPingCount + " succeed:" + succeedCount + " failed:" + failedCount);
						_uIFunc.UIFunction (IP, 0, succeedCount, failedCount, maxRTT, minRTT, averRTT, isDele);
						break;
					}
				}

				//currentPingCount++;
				//接口，由外部实现，用于处理数据和操作UI
				_uIFunc.UIFunction (IP, (int) roundTripTime, succeedCount, failedCount, maxRTT, minRTT, averRTT, isDele);
				System.Threading.Thread.Sleep (_interval);
			}
			currentPingCount += _pingTimes;
			//System.Diagnostics.Debug.WriteLine (ip + "-total:"+ currentPingCount + " succeed:" + succeedCount + " failed:" + failedCount);



		}
	}
}
