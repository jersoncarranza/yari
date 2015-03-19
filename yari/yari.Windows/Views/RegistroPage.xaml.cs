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
using yari.Models;
using System.Net.Http;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace yari.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class RegistroPage : Page
    {
        Usuario oUser = new Usuario();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

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


        public RegistroPage()
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.oUser = (Usuario)e.Parameter;
            var parameter = (Usuario)e.Parameter;
            txtNameUser.Text = "Bienvenid@ " + parameter.NombreUsuario;
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void btnGuardar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string msg = "";
            btnGuardar.Visibility = Visibility.Collapsed;
            this.imgLoad.Visibility = Visibility.Visible;
            this.anmLoad.Begin();

            if (txtNombre.Text == "" || txtPass.Password == "")
            {
                this.anmLoad.Stop();
                if (txtNombre.Text == "")
                    msg = "por favor ingrese un nombre";
                else if (txtPass.Password == "")
                    msg = "por favor ingrese una contraseña";

                MessageDialog messageDialog = new MessageDialog(msg, "error al guardar");
                await messageDialog.ShowAsync();
            }
            else
            {
                string msg2 = "";
                if (txtPass.Password.Length < 3 || txtNombre.Text.Length < 3)
                {
                    if (txtPass.Password.Length < 3)
                        msg = "la contraseña debe tener mínimo 3 caracteres";
                    else if(txtNombre.Text.Length < 3)
                         msg = "el nombre debe tener mínimo 3 caracteres";
                    msg2 = "error al guardar";
                }
                else
                {
                    var postData = new List<KeyValuePair<string, string>>();
                    var content = new FormUrlEncodedContent(postData);
                    var httpClient = new HttpClient();
                    var request = await httpClient.PutAsync(new Uri("http://yari.azurewebsites.net/api/usuario/insert/?idTipe=3&name=" + txtNombre.Text + "&pass=" + txtPass.Password + "&idUserCreador=" + oUser.IdUsuario + "&userLogout=0", UriKind.RelativeOrAbsolute), content);
                    var result = request.Content.ReadAsStringAsync();

                    if (Convert.ToBoolean(result.Result))
                    {
                        msg = "estudiante registrado correctamente";
                        msg2 = "guardado";
                        txtNombre.Text = "";
                        txtPass.Password = "";
                    }
                    else
                    {
                        msg = "error al registrar el estudiante";
                        msg2 = "error al guardar";
                    }
                }
                this.anmLoad.Stop();
                MessageDialog messageDialog = new MessageDialog(msg, msg2);
                await messageDialog.ShowAsync();
            }
            this.imgLoad.Visibility = Visibility.Collapsed;
            btnGuardar.Visibility = Visibility.Visible;
        }

        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }

        private void btnGuardar_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            SolidColorBrush sBrush = (SolidColorBrush)btnGuardar.Background;
            btnGuardar.Background = sBrush;
        }


    }
}
