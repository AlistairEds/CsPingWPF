using CsPingWPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CsPingWPF.ViewModels {
	public class DataViewModel : PingCore.UIFunc {
		//get;set;来让xaml能获取绑定
		public ObservableCollection<DataModel> pingDatas {
			get; set;
		}
		public DelegateCommand BeginButton {
			get; set;
		}
		public DelegateCommand AbortButton {
			get; set;
		}
		public ProgressModel Progress {
			get; set;
		}
		public ConfigModel Config {
			get; set;
		}

		private PingCore pingCore;
		private IniModel iniModel;

		//构造函数 初始化，新的程序入口
		public DataViewModel () {
			pingDatas = new ObservableCollection<DataModel> ();
			BeginButton = new DelegateCommand {
				ExecuteCommand = new Action<object> (BeginButtonCommand),
				CanExecuteCommand = new Func<object, bool> (CanBeginButton)
			};
			AbortButton = new DelegateCommand {
				ExecuteCommand = new Action<object> (AbortButtonCommand)
			};
			Progress = new ProgressModel ();
			Config = new ConfigModel ();
			iniModel = new IniModel ();
			//读入配置文件
			iniModel.SetFilePath (AppDomain.CurrentDomain.BaseDirectory + "config.ini");
			ReadIniFile (iniModel, Config);
		}
		//开始按钮
		bool CanBeginButton ( object parameter ) {
			return Config.isConfigRight;
		}
		void BeginButtonCommand ( object obj ) {
			//清空datagrid
			pingDatas.Clear ();
			//重置进度条
			Progress.ProgressValue = 0;
			//设置参数			
			pingCore = new PingCore (int.Parse (Config.PingTimesText), int.Parse (Config.PingIntervalText), int.Parse (Config.MaxFailedCountText), this);
			//保存配置
			WriteIniFile (iniModel, Config);
			//开启线程池
			StartTheard (Config, Progress, pingCore);


		}
		//终止按钮
		void AbortButtonCommand ( object obj ) {
			if ( pingCore != null ) {
				pingCore.AbortAll ();
				Progress.ProgressValue = 100;
			}
		}


		/// <summary>
		/// 实现接口，进行UI操作
		/// </summary>
		public void UIFunction ( string ip, int rtt, int succeedCount, int failedCount, int maxRTT, int minRTT, int averRTT, bool isDele ) {
			App.Current.Dispatcher.BeginInvoke (new Action (() => {
				//获取或添加指定项目
				DataModel data;
				if ( pingDatas.Any (p => p.IP.Equals (ip)) ) {
					data = pingDatas.First (p => p.IP.Equals (ip));
				} else {
					data = new DataModel ();
					data.IP = ip;
					pingDatas.Add (data);
				}

				if ( isDele ) {
					//删除不可用IP
					pingDatas.Remove (data);
				} else {
					//修改数据

					data.RTT = rtt;
					data.SucceedCount = succeedCount;
					data.FailedCount = failedCount;
					data.MaxRTT = maxRTT;
					data.MinRTT = minRTT;
					data.AverRTT = averRTT;
				}
			}));
		}

		/// <summary>
		/// 使用线程池逐个ping IP
		/// </summary>
		/// <param name="config"></param>
		/// <param name="progress"></param>
		/// <param name="pingCore"></param>
		private void StartTheard ( ConfigModel config, ProgressModel progress, PingCore pingCore ) {
			int minThreads = int.Parse (config.MinThreadText);
			System.Threading.ThreadPool.SetMinThreads (minThreads, minThreads);
			//System.Threading.ThreadPool.SetMaxThreads (500, 500);

			for ( int i = int.Parse (config.IpTextBoxC); i <= int.Parse (config.IpTextBoxD); i++ ) {
				for ( int j = 1; j <= 255; j++ ) {
					string ip = config.IpTextBoxA + "." + config.IpTextBoxB + "." + i + "." + j;
					System.Threading.ThreadPool.QueueUserWorkItem (new System.Threading.WaitCallback (pingCore.PingEachIP),
				ip);
				}
			}
			int totalCount = ( int.Parse (config.IpTextBoxD) - int.Parse (config.IpTextBoxC) + 1 ) * 255 * int.Parse(config.PingTimesText);
			progress.SetParameter (pingCore, totalCount);
			//新建一个线程处理progressbar
			new System.Threading.Thread (new System.Threading.ThreadStart (progress.UpdateProgressBarThread)).Start ();

		}

		/// <summary>
		/// 实现按钮控件接口
		/// </summary>
		public class DelegateCommand : ICommand {
			public Action<object> ExecuteCommand = null;
			public Func<object, bool> CanExecuteCommand = null;
			public event EventHandler CanExecuteChanged;
			/// <summary>
			/// 如果定义了CanExecuteCommand，则由该方法返回是否启用，若未定义，默认返回true
			/// </summary>
			/// <param name="parameter"></param>
			/// <returns></returns>
			public bool CanExecute ( object parameter ) {
				if ( CanExecuteCommand != null ) {
					return this.CanExecuteCommand (parameter);
				} else {
					return true;
				}
			}

			public void Execute ( object parameter ) {
				if ( this.ExecuteCommand != null ) {
					this.ExecuteCommand (parameter);
				}
			}

			public void RaiseCanExecuteChanged () {
				if ( CanExecuteChanged != null ) {
					CanExecuteChanged (this, EventArgs.Empty);
				}
			}
		}


		private void ReadIniFile ( IniModel ini, ConfigModel config ) {

			string section = "PingConfig";
			ini.GetValue (section, "SourceIP", out string sourceIP);
			if ( !sourceIP.Equals ("-1") ) {
				//读入源IP段
				string [] sArray = sourceIP.Split ('.');
				config.IpTextBoxA = sArray [0];
				config.IpTextBoxB = sArray [1];
				config.IpTextBoxC = sArray [2];
				//读入目的IP段
				ini.GetValue (section, "DestinationIP", out string destinationIP);
				string [] dArray = destinationIP.Split ('.');
				config.IpTextBoxD = dArray [2];
				//读入最大失败数
				ini.GetValue (section, "MaxFailedCount", out string maxFailed);
				config.MaxFailedCountText = maxFailed;
				//读入ping间隔
				ini.GetValue (section, "Interval", out string interval);
				config.PingIntervalText = interval;
				//读入ping次数				 
				ini.GetValue (section, "PingTimes", out string pingTimes);
				config.PingTimesText = pingTimes;
				//读入最小线程数
				ini.GetValue (section, "MinThreadCount", out string minThreadCount);
				config.MinThreadText = minThreadCount;

			}
		}

		private void WriteIniFile ( IniModel ini, ConfigModel config ) {
			string section = "PingConfig";
			string sourIP = config.IpTextBoxA + "." + config.IpTextBoxB + "." + config.IpTextBoxC;
			string desIP = config.IpTextBoxA + "." + config.IpTextBoxB + "." + config.IpTextBoxD;
			ini.SetValue (section, "SourceIP", sourIP);
			ini.SetValue (section, "DestinationIP", desIP);
			ini.SetValue (section, "MaxFailedCount", config.MaxFailedCountText);
			ini.SetValue (section, "Interval", config.PingIntervalText);
			ini.SetValue (section, "PingTimes", config.PingTimesText);
			ini.SetValue (section, "MinThreadCount", config.MinThreadText);

		}

	}
}
