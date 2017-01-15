using System;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.Media.SpeechRecognition;

namespace SpeechPlatform1
{
    public sealed partial class MainPage : Page
    {
        private SpeechRecognizer _speechRecognizer;
        private readonly CoreDispatcher _dispatcher;

        public MainPage()
        {
            InitializeComponent();

            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            InitializeRecognizer();
        }

        public async void InitializeRecognizer()
        {
            var permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
                ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationSpeechResources");

                ResetRecognizerIfInitialized();

                _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);

                SetupConstraits();

                await _speechRecognizer.CompileConstraintsAsync();

                _speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;

            }
            else
            {
                throw new Exception("Permission to access capture resources was not given by the user; please set the application setting in Settings->Privacy->Microphone.");
            }
        }

        private void ResetRecognizerIfInitialized()
        {
            if (_speechRecognizer != null)
            {
                _speechRecognizer.Dispose();
                _speechRecognizer = null;
            }
        }

        private void SetupConstraits()
        {
            var onConstrait = new SpeechRecognitionListConstraint(new[] { "lights on", "turn the lights on" }, "on");
            var offConstraint = new SpeechRecognitionListConstraint(new[] { "lights off", "turn the lights off", "off" });
            _speechRecognizer.Constraints.Add(onConstrait);
            _speechRecognizer.Constraints.Add(offConstraint);
        }

        private async void startListenBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _speechRecognizer.RecognizeAsync();
            if (result.Status == SpeechRecognitionResultStatus.Success)
            {
                HandleResult(result);
            }
        }

        private async void startContinousListenBtn_Click(object sender, RoutedEventArgs e)
        {
            await _speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }

        private async void stopContinousListenBtn_Click(object sender, RoutedEventArgs e)
        {
            await _speechRecognizer.ContinuousRecognitionSession.StopAsync();
        }

        private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            HandleResult(args.Result);
        }

        private async void HandleResult(SpeechRecognitionResult result)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var tag = result.Constraint?.Tag ?? "";
                outputTxt.Text = result.Text + " tag: " + tag + " confidence: " + result.Confidence;
                
                //call logics here. 
                //lightswitcher(tag);
            });
        }        
        
    }
}
