using yari.Common;
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
using Windows.UI.Popups;
//using Windows.Web.Http;
using Windows.Data.Json;
using System.Net.Http;
using Windows.UI;
using yari.Models;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace yari.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LoginPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public static int idUser = 0;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public LoginPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.txtPass.Password = "";
            if (idUser != 0)
            {
                var postData = new List<KeyValuePair<string, string>>();
                var content = new FormUrlEncodedContent(postData);
                var httpClient = new HttpClient();
                var request = await httpClient.PostAsync(new Uri("http://yari.azurewebsites.net/api/usuario/logout/?id=" + idUser + "&userlogout=0", UriKind.RelativeOrAbsolute), content);
                var result = request.Content.ReadAsStringAsync();
                idUser = int.Parse(result.Result);
            }
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }

        private async void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            string msg;
            btnAceptar.Visibility = Visibility.Collapsed;
            this.imgLoad.Visibility = Visibility.Visible;
            this.anmLoad.Begin();

            if (txtNombre.Text == "" || txtPass.Password == "")
            {
                this.anmLoad.Stop(); 
                if (txtNombre.Text == "")
                    msg = "por favor ingrese su nombre";
                else
                    msg = "por favor ingrese su contraseña";
                MessageDialog messageDialog = new MessageDialog(msg, "error de auntentificación");
                await messageDialog.ShowAsync();
            }
            else
            {
                var httpClient = new HttpClient();
                var request = await httpClient.GetAsync(new Uri("http://yari.azurewebsites.net/api/usuario/login/?name=" + txtNombre.Text + "&pass=" + txtPass.Password, UriKind.RelativeOrAbsolute));
                var txtJson = await request.Content.ReadAsStringAsync();
                JsonValue jsonList = JsonValue.Parse(txtJson);

                if (txtJson != "null")
                {
                    this.anmLoad.Stop(); 
                    var iduser = jsonList.GetArray().GetNumberAt(0);
                    var idusertype = jsonList.GetArray().GetNumberAt(1);
                    if (idusertype == 2)
                    {
                        idUser = int.Parse(iduser.ToString());
                        Usuario oUser = new Usuario();
                        oUser.IdUsuario = idUser;
                        oUser.NombreUsuario = txtNombre.Text;
                        oUser.IdTipoUsuario = Convert.ToInt32(idusertype);
                        this.Frame.Navigate(typeof(PanelAdminPage), oUser);
                    }
                    else if (idusertype == 3)
                    {
                        idUser = int.Parse(iduser.ToString());
                        Usuario oUser = new Usuario();
                        oUser.IdUsuario = idUser;
                        oUser.NombreUsuario = txtNombre.Text;
                        oUser.IdTipoUsuario = Convert.ToInt32(idusertype);
                        this.Frame.Navigate(typeof(MenuPage), oUser);
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("usted ya esta logeado", "error de auntentificación");
                        await messageDialog.ShowAsync();
                    }           
                }
                else
                {
                    this.anmLoad.Stop();
                    MessageDialog messageDialog = new MessageDialog("por favor vuelva a escribir correctamente su nombre y contraseña", "error de auntentificación");
                    await messageDialog.ShowAsync();
                }

            }
            this.imgLoad.Visibility = Visibility.Collapsed;
            btnAceptar.Visibility = Visibility.Visible;
        }
        
        private void btnAceptar_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush sBrush = (SolidColorBrush)btnAceptar.Background;
            btnAceptar.Background = sBrush;
        }
    }
}
