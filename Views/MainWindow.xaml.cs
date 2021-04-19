using System.Windows;
using CsPingWPF.ViewModels;

namespace CsPingWPF {
	
	public partial class MainWindow : Window {

		
		public MainWindow () {
			InitializeComponent ();
			this.DataContext = new DataViewModel ();
			
			
		}

	
		
	}
}
