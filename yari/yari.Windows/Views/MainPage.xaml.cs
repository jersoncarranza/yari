using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using yari.Views;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace yari
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.AnmIniciar.Begin();
        }

        private void ProjectionGenerate(int n, Canvas boton)
        {
            PlaneProjection pp = new PlaneProjection();
            pp.RotationX = n;
            boton.Projection = pp;
        }

        private void cvButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var canvas = (Canvas)sender;
            if (canvas.Name == "cvIniciar")
                ProjectionGenerate(-40, cvIniciar);
        }

        private void cvButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var canvas = (Canvas)sender;
            if (canvas.Name == "cvIniciar")
                ProjectionGenerate(0, cvIniciar);
        }

        private void cvButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var canvas = (Canvas)sender;
            if (canvas.Name == "cvIniciar")
                this.Frame.Navigate(typeof(LoginPage));
        }


        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }

    }
}
