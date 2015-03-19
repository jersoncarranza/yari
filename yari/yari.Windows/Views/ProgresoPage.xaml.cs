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
using yari.Models;
using System.Net.Http;
using Windows.Data.Json;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace yari.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ProgresoPage : Page
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


        public ProgresoPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            this.Loaded += ProgresoPage_Loaded;
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
            //txtNameUser.Text = "Bienvenid@ " + parameter.NombreUsuario;
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        List<Usuario> listUser; 
        void ProgresoPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Cargar lista de estudiantes
            var httpClient = new HttpClient();
            HttpResponseMessage request = httpClient.GetAsync(new Uri("http://yari.azurewebsites.net/api/usuario/idcreador/?id=" + oUser.IdUsuario, UriKind.RelativeOrAbsolute)).Result;
            Usuario[] user = JsonConvert.DeserializeObject<Usuario[]>(request.Content.ReadAsStringAsync().Result);
            
            listUser = new List<Usuario>();
            listUser = user.ToList();
            this.studentList.Items.Clear();

            foreach (var item in listUser)
            {
                this.studentList.Items.Add(item.NombreUsuario);
            }
        }

        private void ProjectionGenerate(int n, StackPanel boton)
        {
            PlaneProjection pp = new PlaneProjection();
            pp.RotationX = n;
            boton.Projection = pp;
        }

        private void cvButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var stackP = (StackPanel)sender;
            if (stackP.Name == "spEditarGuardar")
                ProjectionGenerate(-40, spEditarGuardar);
            else if (stackP.Name == "spEliminar")
                ProjectionGenerate(-40, spEliminar);
        }

        private void cvButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var stackP = (StackPanel)sender;
            if (stackP.Name == "spEditarGuardar")
                ProjectionGenerate(0, spEditarGuardar);
            else if (stackP.Name == "spEliminar")
                ProjectionGenerate(0, spEliminar);
        }

        private void cvButton_Tapped(object sender, TappedRoutedEventArgs e)
        {            
            var stackP = (StackPanel)sender;
            if (stackP.Name == "spEditarGuardar")
            {
                if (txtEditarGuardar.Text=="editar")
                {
                    txtNombre.IsReadOnly = false;
                    txtPass.IsReadOnly = false;
                    txtEditarGuardar.Text = "guardar";
                    imgEditarGuardar.Source = new BitmapImage(new Uri("ms-appx:///Images/saveImg.png"));

                }
                else
                {
                    SaveUpdate();
                }  
            }
            else if (stackP.Name == "spEliminar")
            {

            }
        }

        private async void SaveUpdate()
        {
            string msg = "";
            if (txtNombre.Text == "" || txtPass.Text == "")
            {
                if (txtNombre.Text == "")
                    msg = "por favor ingrese un nombre";
                else if (txtPass.Text == "")
                    msg = "por favor ingrese una contraseña";

                MessageDialog messageDialog = new MessageDialog(msg, "error al modificar");
                await messageDialog.ShowAsync();
            }
            else
            {
                string msg2 = "";
                if (txtPass.Text.Length < 3 || txtNombre.Text.Length < 3)
                {
                    if (txtPass.Text.Length < 3)
                        msg = "la contraseña debe tener mínimo 3 caracteres";
                    else if (txtNombre.Text.Length < 3)
                        msg = "el nombre debe tener mínimo 3 caracteres";
                    msg2 = "error al modificar";
                }
                else
                {
                    var postData = new List<KeyValuePair<string, string>>();
                    var content = new FormUrlEncodedContent(postData);
                    var httpClient = new HttpClient();
                    var request = await httpClient.PostAsync(new Uri("http://yari.azurewebsites.net/api/usuario/update/?id=" + iduser + "&idTipe=3&name=" + txtNombre.Text + "&pass=" + txtPass.Text + "&idUserCreador=" + oUser.IdUsuario, UriKind.RelativeOrAbsolute), content);
                    var result = request.Content.ReadAsStringAsync();

                    if (Convert.ToBoolean(result.Result))
                    {
                        msg = "estudiante modificado correctamente";
                        msg2 = "modificado";
                        txtNombre.Text = "";
                        txtPass.Text = "";

                        txtNombre.IsReadOnly = true;
                        txtPass.IsReadOnly = true;
                        txtEditarGuardar.Text = "editar";
                        imgEditarGuardar.Source = new BitmapImage(new Uri("ms-appx:///Images/editarEstudianteImg.png"));
                        gdOpciones.Visibility = Visibility.Collapsed;

                        ProgresoPage_Loaded(null, null);
                    }
                    else
                    {
                        msg = "error al modificar al estudiante";
                        msg2 = "error al modificar";
                    }
                }
                MessageDialog messageDialog = new MessageDialog(msg, msg2);
                await messageDialog.ShowAsync();
            }
        }

        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }

        int iduser;
        private void studentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.studentList.Items.Count > 0)
            {
                gdOpciones.Visibility = Visibility.Visible;
                txtNombre.Text = listUser[studentList.SelectedIndex].NombreUsuario;
                txtPass.Text = listUser[studentList.SelectedIndex].ClaveUsuario;
                iduser = listUser[studentList.SelectedIndex].IdUsuario;

                ListarHistoricoJuego(iduser);
            }
        }

        List<HistoricoJuego> listHistoricoJuego;
        private void ListarHistoricoJuego(int iduser)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage request = httpClient.GetAsync(new Uri("http://yari.azurewebsites.net/api/historicojuego/iduser/?id="+iduser, UriKind.RelativeOrAbsolute)).Result;
            HistoricoJuego[] user = JsonConvert.DeserializeObject<HistoricoJuego[]>(request.Content.ReadAsStringAsync().Result);

            listHistoricoJuego=new List<HistoricoJuego>();
            listHistoricoJuego = user.ToList();
            this.historicoJuegoList.Items.Clear();

            foreach (var item in listHistoricoJuego)
            {
                this.historicoJuegoList.Items.Add(item);
            }
        }


    }
}
