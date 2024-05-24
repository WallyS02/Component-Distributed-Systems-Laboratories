using AxAcroPDFLib;
using AxSHDocVw;
using AxWMPLib;
using System.Windows;

namespace _7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            AxWindowsMediaPlayer axWmp = new AxWindowsMediaPlayer();
            host.Child = axWmp;
            grid1.Children.Add(host);
            axWmp.URL = "C:\\Users\\Ja\\Downloads\\KSR\\Lab\\Lab 3\\COMLab3\\7\\7\\sample.wav";

            System.Windows.Forms.Integration.WindowsFormsHost host1 = new System.Windows.Forms.Integration.WindowsFormsHost();
            AxAcroPDF axAcroPDF = new AxAcroPDF();
            host1.Child = axAcroPDF;
            grid2.Children.Add(host1);
            axAcroPDF.LoadFile("C:\\Users\\Ja\\Downloads\\KSR\\Lab\\Lab 3\\COMLab3\\7\\7\\sample.pdf");

            System.Windows.Forms.Integration.WindowsFormsHost host2 = new System.Windows.Forms.Integration.WindowsFormsHost();
            AxWebBrowser axWebBrowser = new AxWebBrowser();
            host2.Child = axWebBrowser;
            grid3.Children.Add(host2);
            axWebBrowser.Navigate("https://google.com");
        }
    }
}