namespace Pinger {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public ViewModel ViewModel { get; }

        public MainWindow() {
            ViewModel = new ViewModel();

            Services.Tracker.Track(App.Settings);

            InitializeComponent();

            Services.Tracker.Track(this);
        }
    }
}