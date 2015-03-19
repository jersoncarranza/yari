using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using yari.Common;
using yari.Models;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace yari.Views
{
    public sealed partial class JugarPageWP : Page
    {
        Usuario oUser = new Usuario();

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public JugarPageWP()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.Loaded += JugarPageWP_Loaded;
        }

        void JugarPageWP_Loaded(object sender, RoutedEventArgs e)
        {
            //preguntar si ya no tiene un juego iniciado
            //sino partimos desde ahi
            txtNivel.Text = "2";
            txtPuntos.Text = "0";
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
            txtNombre.Text = "nombre: " + this.oUser.NombreUsuario;
            this.navigationHelper.OnNavigatedTo(e);
            this.anmMic.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        int cont = 0;

        private void cvSpeech_Tapped(object sender, TappedRoutedEventArgs e)
        {
            cvTextToSpeech.Visibility = Visibility.Visible;
            cvSpeech.Visibility = Visibility.Collapsed;
            anmBoca.Begin();

            if (bibliotecaList == null)
                ObtenerPalabrasNivel(int.Parse(txtNivel.Text));
            else if (bibliotecaList.Count == cont)
                ObtenerPalabrasNivel(int.Parse(txtNivel.Text) + 1);


            txtPalabra.Text = bibliotecaList[cont].Palabra;
            imgPalabra.Visibility = Visibility.Visible;
            imgPalabra.Source = new BitmapImage(new Uri(bibliotecaList[cont].Imagen));
            cont++;
            TextToSpeech(txtPalabra.Text, "PalabraNueva");

        }

        List<Biblioteca> bibliotecaList;
        private void ObtenerPalabrasNivel(int nivel)
        {
            cont = 0;
            var httpClient = new HttpClient();
            HttpResponseMessage request = httpClient.GetAsync(new Uri("http://yari.azurewebsites.net/api/biblioteca/idniveljuego/?id=" + nivel, UriKind.RelativeOrAbsolute)).Result;
            Biblioteca[] biblioteca = JsonConvert.DeserializeObject<Biblioteca[]>(request.Content.ReadAsStringAsync().Result);

            bibliotecaList = new List<Biblioteca>();
            bibliotecaList = biblioteca.ToList();
        }

        private async void TextToSpeech(string textToRead, string opcion)
        {
            string ssmlText = "";
            using (SpeechSynthesizer speech = new SpeechSynthesizer())
            {
                if ("PalabraNueva" == opcion)
                {
                    ssmlText = "<speak version=\"1.0\" ";
                    ssmlText += "xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"es-ES\">";
                    ssmlText += " Por favor pronuncie conmigo.. " + textToRead;
                    ssmlText += "</speak>";
                    recVoz = false;
                }
                else if ("PalabraIncorrecta" == opcion)
                {
                    ssmlText = "<speak version=\"1.0\" ";
                    ssmlText += "xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"es-ES\">";
                    ssmlText += " No esta correctamente pronunciada. Nuevamente pronuncie..." + textToRead;
                    ssmlText += "</speak>";
                    recVoz = false;
                }
                else if ("PalabraCorrecta" == opcion)
                {
                    ssmlText = "<speak version=\"1.0\" ";
                    ssmlText += "xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"es-ES\">";
                    ssmlText += " Felicitaciones. Palabra pronuciada correctamente";
                    ssmlText += "</speak>";
                    recVoz = true;
                }

                var voiceStream = await speech.SynthesizeSsmlToStreamAsync(ssmlText);
                this.media.SetSource(voiceStream, voiceStream.ContentType);
                this.media.Play();
                anmBoca.Begin();
                this.media.MediaEnded += media_MediaEnded;

            }
        }

        void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            anmBoca.Stop();
            if (recVoz)
            {
                txtPalabra.Text = "";
                cvSpeech.Visibility = Visibility.Visible;
                imgPalabra.Visibility = Visibility.Collapsed;
                anmMic.Begin();
            }
            else
                reconocimientoVoz();
        }

        private bool recVoz;
        private SpeechRecognizer speechRecognizer = new SpeechRecognizer();

        private async void reconocimientoVoz()
        {
            try
            {
                this.speechRecognizer = new SpeechRecognizer();
                await speechRecognizer.CompileConstraintsAsync();
                SpeechRecognitionResult speechRecognitionResult = await this.speechRecognizer.RecognizeWithUIAsync();

                if (txtPalabra.Text.ToLower() + "." == speechRecognitionResult.Text.ToLower())
                    TextToSpeech("", "PalabraCorrecta");//pronunciacion correcta
                else
                    TextToSpeech(txtPalabra.Text, "PalabraIncorrecta");//pronunciacion incorrecta 
            }
            catch (Exception)
            {
            }
        }

        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }



    }
}
