using SolidNavigation.Navigation;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SolidNavigation
{
    sealed partial class App
    {
        private Bootstrapper _bootstrapper;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        private void Init()
        {
            _bootstrapper = new Bootstrapper();
        }

        private SerializableFrame _frame = Window.Current.Content as SerializableFrame;
        private FrameNavigator _frameNavigator;

        private async Task InitFrame(bool tryLoadFrameState)
        {
            if (_frame == null)
            {
                _frame = new SerializableFrame();
                _frameNavigator = new FrameNavigator(_frame, _bootstrapper);

                if (tryLoadFrameState)
                {
                    await _frameNavigator.Resume();
                }
                Window.Current.Content = _frame;
            }
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Init();
            await InitFrame(e.PreviousExecutionState == ApplicationExecutionState.Terminated);

            if (e.PrelaunchActivated == false)
            {
                if (_frame.Content == null)
                {
                    _bootstrapper.Navigator.ToUrl(e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        private async  void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await _frameNavigator.Suspend();
            deferral.Complete();
        }
    }
}
