using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using yari.Common;
using yari.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace yari.Views
{
    public sealed partial class MenuPage : Page
    {
        Usuario oUser = new Usuario();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MenuPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

        }
        
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.oUser = (Usuario)e.Parameter;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        

        private void ProjectionGenerate(int n, Canvas boton)
        {
            PlaneProjection pp = new PlaneProjection();
            pp.RotationX = n;
            boton.Projection = pp;
        }

        private void cvButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var canvas = (Canvas)sender;
            if (canvas.Name == "cvJugar")
                ProjectionGenerate(-40, cvJugar);
            else if (canvas.Name == "cvPuntuacion")
                ProjectionGenerate(-40, cvPuntuacion);                       
        }

        private void cvButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var canvas = (Canvas)sender;
            if (canvas.Name == "cvJugar")
                ProjectionGenerate(0, cvJugar);
            else if (canvas.Name == "cvPuntuacion")
                ProjectionGenerate(0, cvPuntuacion);
        }

        private void cvButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var canvas = (Canvas)sender;
            if (canvas.Name == "cvJugar")
                AnmStartJugar();
            else if (canvas.Name == "cvPuntuacion")
                AnmStartPuntuacion();
        }

        private void AnmStartPuntuacion()
        {
            this.ellPunt.Visibility = Visibility.Visible;
            this.anmCirclePunt.Begin();
            this.anmCirclePunt.Completed += anmCirclePunt_Completed;
        }

        void anmCirclePunt_Completed(object sender, object e)
        {
            this.Frame.Navigate(typeof(PuntuacionesPageWP),oUser);
        }

        private void AnmStartJugar()
        {
            this.ellJugar.Visibility = Visibility.Visible;
            this.anmCircleJugar.Begin();
            this.anmCircleJugar.Completed += anmCircleJugar_Completed;
        }

        void anmCircleJugar_Completed(object sender, object e)
        {
            this.Frame.Navigate(typeof(JugarPageWP),oUser);
        }
        
        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }

    }
}
