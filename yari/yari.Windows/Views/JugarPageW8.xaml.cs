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
using Windows.Media.SpeechSynthesis;
using yari.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Bing.Speech;
using Windows.UI.Core;
using System.Diagnostics;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace yari.Views
{
    public sealed partial class JugarPageW8 : Page
    {
        Usuario oUser = new Usuario();

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

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
            txtNombre.Text = "nombre: "+this.oUser.NombreUsuario;
            navigationHelper.OnNavigatedTo(e);
            this.anmMic.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        public JugarPageW8()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            this.Loaded += JugarPageW8_Loaded;
        }

        SpeechRecognizer SR;
        private bool recVoz;

        void JugarPageW8_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply credentials from the Windows Azure Data Marketplace.
            var credentials = new SpeechAuthorizationParameters();
            credentials.ClientId = "2yariBingSpeech2014";
            credentials.ClientSecret = "2yariBingSpeechClientSecret";
            // Initialize the speech recognizer.
            SR = new SpeechRecognizer("es-ES", credentials);

            //preguntar si ya no tiene un juego iniciado
            //sino partimos desde ahi
            txtNivel.Text = "2";
            txtPuntos.Text = "0";
        }

        int cont = 0;
        private void cvSpeech_Tapped(object sender, TappedRoutedEventArgs e)
        {
            cvTextToSpeech.Visibility = Visibility.Visible;
            cvSpeech.Visibility = Visibility.Collapsed;
            anmBoca.Begin();

            if (bibliotecaList == null)
                ObtenerPalabrasNivel(int.Parse(txtNivel.Text));
            else if(bibliotecaList.Count==cont)
                ObtenerPalabrasNivel(int.Parse(txtNivel.Text)+1);


            txtPalabra.Text = bibliotecaList[cont].Palabra;
            imgPalabra.Visibility = Visibility.Visible;
            imgPalabra.Source = new BitmapImage(new Uri(bibliotecaList[cont].Imagen));
            cont++;
            TextToSpeech(txtPalabra.Text, "PalabraNueva");

        }

        List<Biblioteca> bibliotecaList;
        private void ObtenerPalabrasNivel(int nivel)
        {
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
                txtEscuchando.Visibility = Visibility.Collapsed;
                anmBoca.Begin();
                this.media.MediaEnded += media_MediaEnded;
                band = 0;
            }
        }

        int band = 0;
        void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            anmBoca.Stop();
            if (recVoz)
            {
                txtPalabra.Text = "";
                cvSpeech.Visibility = Visibility.Visible;
                imgPalabra.Visibility = Visibility.Collapsed;
                //txtPalabra.IsReadOnly = false;
                anmMic.Begin();
            }
            else
            {
                if (band == 0)
                {
                    txtEscuchando.Visibility = Visibility.Visible;
                    reconocimientoVoz();
                    band++;
                }
            }
        }


        private async void reconocimientoVoz()
        {
            try
            {
                // Start speech recognition.
                var speechRecognitionResult = await SR.RecognizeSpeechToTextAsync();

                // Search the text with some confidence.
                if (speechRecognitionResult.TextConfidence != SpeechRecognitionConfidence.Rejected)
                {
                    await this.ProcessResult(speechRecognitionResult);
                }
                else
                {
                    TextToSpeech(txtPalabra.Text, "PalabraIncorrecta");
                }
            }
            catch (System.Exception ex)
            {
                // Swallow exception, if any.
                Debug.WriteLine(ex);
            }
        }

        private async Task ProcessResult(SpeechRecognitionResult result)
        {
            string s = "";
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                s = result.Text;
            });

            if (txtPalabra.Text.ToLower() + "." == s.ToLower())
                TextToSpeech("", "PalabraCorrecta");//pronunciacion correcta
            else
                TextToSpeech(txtPalabra.Text, "PalabraIncorrecta");//pronunciacion incorrecta 

           
        }

        private void imgYariNow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(YariNowPage));
        }

    }
}
