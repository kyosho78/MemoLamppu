namespace MemoLamppu
{
    public partial class MainPage : TabbedPage
    {
        private CancellationTokenSource cancellationTokenSource;


        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder
         .LocalApplicationData), "muistio.txt");

        string text = "";


        public MainPage()
        {
            InitializeComponent();

            vahvistusSlider.Maximum = 100;
            vahvistusSlider.Minimum = 0;

            bool doesExist = File.Exists(fileName);

            if (doesExist == true)
            {
                text = File.ReadAllText(fileName);
                if (text.Length > 0)
                {
                    outputLabel.Text = text;
                }
                else
                {
                    outputLabel.Text = "Mitään ei ole talletettu muistioon.";
                }

            }
            else
            {
                outputLabel.Text = "Tervetuloa uudelle käyttäjälle!";
            }
        }

        private void tallennusNappi_Clicked(object sender, EventArgs e)
        {
            text = text + Environment.NewLine + inputKentta.Text;
            File.WriteAllText(fileName, text);

            outputLabel.Text = text;
            inputKentta.Text = "";
        }

        private void poistoNappi_Clicked(object sender, EventArgs e)
        {
            poistoNappi.IsVisible = false;
            vahvistusInfo.IsVisible = true;
            vahvistusSlider.IsVisible = true;
        }

        private void vahvistusSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (vahvistusSlider.Value > 95)
            {
                text = "";
                File.WriteAllText(fileName, text);
                outputLabel.Text = "";
                vahvistusSlider.Value = 0;
                poistoNappi.IsVisible = true;
                vahvistusInfo.IsVisible = false;
                vahvistusSlider.IsVisible = false;
            }

        }


        private void lamppu_on_Clicked(object sender, EventArgs e)
        {
            if (Battery.ChargeLevel > 0.05)
            {
                Flashlight.TurnOnAsync();
                lamppu_on.IsVisible = false;
                lamppu_off.IsVisible = true;
            }
            else
            {
                Vibration.Vibrate();
                DisplayAlert("Virhe", "Puhelimen akku on liian heikko valon sytyttämiseen.", "OK");
            }
        }

        private void lamppu_off_Clicked(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            Flashlight.TurnOffAsync();
            lamppu_on.IsVisible = true;
            lamppu_off.IsVisible = false;
        }

        private async Task BlinkFlashlight(int onDuration, int offDuration, int repeatCount, CancellationToken token)
        {
            try
            {
                for (int i = 0; i < repeatCount; i++)
                {
                    if (token.IsCancellationRequested)
                        break;

                    await Flashlight.TurnOnAsync();
                    await Task.Delay(onDuration, token);
                    await Flashlight.TurnOffAsync();
                    await Task.Delay(offDuration, token);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Virhe", "Taskulamppu ei ole käytettävissä laitteessasi.", "OK");
            }
        }

        private async Task FlashSOS(CancellationToken token)
        {
            try
            {
                int shortDelay = 200;  // Duration for a short blink (milliseconds)
                int longDelay = 600;   // Duration for a long blink (milliseconds)

                for (int i = 0; i < 3; i++)
                {
                    if (token.IsCancellationRequested) break;
                    await Flashlight.TurnOnAsync();
                    await Task.Delay(shortDelay, token);
                    await Flashlight.TurnOffAsync();
                    await Task.Delay(shortDelay, token);
                }

                for (int i = 0; i < 3; i++)
                {
                    if (token.IsCancellationRequested) break;
                    await Flashlight.TurnOnAsync();
                    await Task.Delay(longDelay, token);
                    await Flashlight.TurnOffAsync();
                    await Task.Delay(shortDelay, token);
                }

                for (int i = 0; i < 3; i++)
                {
                    if (token.IsCancellationRequested) break;
                    await Flashlight.TurnOnAsync();
                    await Task.Delay(shortDelay, token);
                    await Flashlight.TurnOffAsync();
                    await Task.Delay(shortDelay, token);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Virhe", "Taskulamppu ei ole käytettävissä laitteessasi.", "OK");
            }
        }

        private async void BlinkFastButton_Clicked(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            await BlinkFlashlight(100, 100, int.MaxValue, cancellationTokenSource.Token);
        }

        private async void SOSButton_Clicked(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            await FlashSOS(cancellationTokenSource.Token);
        }


    }
}
