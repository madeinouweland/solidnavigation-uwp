using ABC.Navigation;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;

namespace SolidNavigation.Navigation
{
    public class FrameNavigator
    {
        private UrlSerializer _urlSerializer;
        private Navigator _navigator;
        private SerializableFrame _frame;

        public FrameNavigator(
            SerializableFrame frame,
            Bootstrapper bootstrapper)
        {
            _frame = frame;
            _navigator = bootstrapper.Navigator;
            _urlSerializer = bootstrapper.UrlSerializer;

            _navigator.Navigated += OnNavigated;

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, e) =>
            {
                if (_frame.CanGoBack)
                {
                    _frame.GoBack();
                    e.Handled = true;
                }
            };

            _frame.Navigated += OnFrameNavigated;
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.New) // back or forward navigation
            {
                _navigator.ToUrl(e.Parameter + "");
            }

            if (_frame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

        }

        private Type _oldPageType;

        private void OnNavigated(object sender, INavigationTarget target)
        {
            var url = _urlSerializer.CreateUrl(target);
            var route = _urlSerializer.FindRouteOrDefault(url);

            if (_oldPageType != route.PageType)
            {
                _oldPageType = route.PageType;
                _frame.DataContext = route.PageViewModelFactory?.Invoke(); // if the factory is null, the datacontext will be set to null
            }

            _frame.Navigate(route.PageType, url);
            ApplicationView.GetForCurrentView().Title = url;
        }

        public async Task Resume()
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path);
            var file = await folder.GetFileAsync("navigation.state");
            _frame.Resume(await file.OpenStreamForReadAsync(), _navigator);
        }

        public async Task Suspend()
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path);
            var file = await folder.CreateFileAsync("navigation.state", CreationCollisionOption.ReplaceExisting);

            var input = _frame.GetSuspensionStream();
            using (var output = await file.OpenStreamForWriteAsync())
            {
                await input.CopyToAsync(output);
                await output.FlushAsync();
            }

           // _frame.Resume(await SuspensionManager.GetNavigationState(), _navigator);
        }
    }
}

