using yari.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace yari.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class YariNowPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
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

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            anmMic.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        public YariNowPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
            { }
        }
    }
}