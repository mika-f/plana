namespace Plana.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.Title = "Plana for Desktop";

            // workaround until release .NET 9
            // set default window size for MAUI
            // ref: https://github.com/dotnet/maui/issues/7592
            window.Width = 1280;
            window.Height = 720;

            return window;
        }
    }
}
