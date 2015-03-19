using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using yari.Common;
using yari.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace yari.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PuntuacionesPageWP : Page
    {
        Usuario oUser = new Usuario();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public PuntuacionesPageWP()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.Loaded += PuntuacionesPageWP_Loaded;
        }

        List<HistoricoJuego> listHistoricoJuego;
        void PuntuacionesPageWP_Loaded(object sender, RoutedEventArgs e)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage request = httpClient.GetAsync(new Uri("http://yari.azurewebsites.net/api/historicojuego/iduser/?id=" + oUser.IdUsuario, UriKind.RelativeOrAbsolute)).Result;
            HistoricoJuego[] user = JsonConvert.DeserializeObject<HistoricoJuego[]>(request.Content.ReadAsStringAsync().Result);

            listHistoricoJuego = new List<HistoricoJuego>();
            listHistoricoJuego = user.ToList();
            this.historicoJuegoList.Items.Clear();

            foreach (var item in listHistoricoJuego)
            {
                this.historicoJuegoList.Items.Add(item);
            }
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

        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }


    }
}
