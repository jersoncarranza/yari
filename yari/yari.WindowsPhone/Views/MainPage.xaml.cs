using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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
using yari.ViewModels;
using yari.Views;

//using Windows.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace yari
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public static int idUser=0;
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.Loaded += MainPage_Loaded;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            this.AnmIniciar.Begin();
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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.txtPass.Password = "";
            if(idUser!=0)
            {
                var postData = new List<KeyValuePair<string, string>>();
                var content = new FormUrlEncodedContent(postData);
                var httpClient = new HttpClient();
                var request = await httpClient.PostAsync(new Uri("http://yari.azurewebsites.net/api/usuario/logout/?id="+idUser+"&userlogout=0",UriKind.RelativeOrAbsolute),content);
                var result = request.Content.ReadAsStringAsync();
                idUser = int.Parse(result.Result);        
            }
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
            if (canvas.Name == "cvIniciar")
                ProjectionGenerate(-40, cvIniciar);
        }

        private void cvButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var canvas = (Canvas)sender;
            if (canvas.Name == "cvIniciar")
                ProjectionGenerate(0, cvIniciar);
        }

        private async void cvButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string msg;
            cvIniciar.Visibility = Visibility.Collapsed;
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
                    if (idusertype == 3)
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
                        if (idusertype == 1 || idusertype == 2)
                        {
                            msg = "por favor vuelva a escribir correctamente su nombre y contraseña";
                            //updatelogout
                            var postData = new List<KeyValuePair<string, string>>();
                            var content = new FormUrlEncodedContent(postData);
                            httpClient = new HttpClient();
                            var request2 = await httpClient.PostAsync(new Uri("http://yari.azurewebsites.net/api/usuario/logout/?id=" + iduser + "&userlogout=0", UriKind.RelativeOrAbsolute), content);
                            var result = request2.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            msg = "usted ya esta logeado";
                        }
                        MessageDialog messageDialog = new MessageDialog(msg, "error de auntentificación");
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
            cvIniciar.Visibility = Visibility.Visible;
        }


        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }


    }
}
