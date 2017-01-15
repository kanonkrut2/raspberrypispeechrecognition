using System;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace SpeechPlatform1
{
    public class AudioCapturePermissions
    {
        private const int NoCaptureDevicesHResult = -1072845856;

        public static async Task<bool> RequestMicrophonePermission()
        {
            try
            {
                var settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio,
                    MediaCategory = MediaCategory.Speech
                };

                var capture = new MediaCapture();

                await capture.InitializeAsync(settings);
            }
            catch (TypeLoadException)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Media player components are unavailable.");
                await messageDialog.ShowAsync();
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception exception)
            {
                if (exception.HResult != NoCaptureDevicesHResult) throw;

                var messageDialog = new Windows.UI.Popups.MessageDialog("No Audio Capture devices are present on this system.");
                await messageDialog.ShowAsync();
                return false;
            }
            return true;
        }
    }
}
