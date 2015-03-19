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
using Bing.Speech;
using Windows.Media.SpeechSynthesis;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace yari.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class YariNowPage : Page
    {

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

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            anmMic.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        public YariNowPage()
        {
            this.InitializeComponent();
            this.Loaded += YariNowPage_Loaded;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        SpeechRecognizer SR;
        private bool recVoz;
        void YariNowPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply credentials from the Windows Azure Data Marketplace.
            var credentials = new SpeechAuthorizationParameters();
            credentials.ClientId = "2yariBingSpeech2014";
            credentials.ClientSecret = "2yariBingSpeechClientSecret";

            // Initialize the speech recognizer.
            SR = new SpeechRecognizer("es-ES", credentials);
        }
        

        private async void cvSpeech_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (txtPalabra.Text == "")
            {
                MessageDialog messageDialog = new MessageDialog("Por favor ingrese una palabra", "atención !!");
                await messageDialog.ShowAsync();
            }
            else
            {
                cvTextToSpeech.Visibility = Visibility.Visible;
                cvSpeech.Visibility = Visibility.Collapsed;
                anmMic.Stop();
                txtPalabra.IsReadOnly = true;
                recVoz = false;
                TextToSpeech(txtPalabra.Text, "PalabraNueva");
            }
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
                    ssmlText += " Felicitaciones. Has pronunciado correctamente";
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
                txtPalabra.IsReadOnly = false;
                anmMic.Begin();
            }
            else
            {
                txtEscuchando.Text = "escuchando...";
                txtEscuchando.Visibility = Visibility.Visible;
                reconocimientoVoz();
            }
        }
        
        private async void reconocimientoVoz()
        {
            try
            {
                // Start speech recognition.
                var speechRecognitionResult = await SR.RecognizeSpeechToTextAsync();
                txtEscuchando.Text = "procesando...";

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

            txtEscuchando.Visibility = Visibility.Collapsed;
        }
    }
}
