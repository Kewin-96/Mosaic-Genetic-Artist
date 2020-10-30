using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeneticMosaicArtist_004
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GMA.GMA_init(this);
            Picture.Picture_init(this);
            start_button.IsEnabled = false;
            triangleCount_slider.IsEnabled = false;
            save_button.IsEnabled = false;
            start_button.Content = "Start";
            /*double y = -99999999;
            double x = 0;
            Console.WriteLine("111: ");
            Console.WriteLine("eeee: " + Math.Atan(y/x));*/
        }
        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            /*if (start_button.Content == "Start")
            {*/
            start_button.IsEnabled = false;
            //start_button.Content = "Stop";
            save_button.IsEnabled = false;
            load_button.IsEnabled = false;
            triangleCount_slider.IsEnabled = false;
            Thread thread = new Thread(GMA.Start);
            thread.Start();
            /*}
            else if (start_button.Content == "Stop")
            {

                start_button.Content = "Start";
            }
            else
            {
                start_button.IsEnabled = false;
                start_button.Content = "CRITICAL ERROR !";
            }*/
        }
        private void triangleCount_slider_released(object sender, MouseButtonEventArgs e)
        {
            GMA.CalcMosaicNDraw();
            start_button.IsEnabled = true;
            save_button.IsEnabled = true;
        }

        private void load_button_Click(object sender, RoutedEventArgs e)
        {
            Picture.Load_Picture();
            triangleCount_slider.IsEnabled = true;
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            Picture.SavePicture();
        }
    }
}
