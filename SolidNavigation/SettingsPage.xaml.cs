namespace SolidNavigation
{
    public sealed partial class SettingsPage
    {
        public SettingsPageViewModel ViewModel => (SettingsPageViewModel)DataContext;

        public SettingsPage()
        {
            InitializeComponent();
        }
    }
}
