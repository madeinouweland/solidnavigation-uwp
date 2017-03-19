using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace SolidNavigation
{
    public sealed partial class MainPage
    {
        public MainPageViewModel ViewModel => (MainPageViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();
            DataContextChanged += MainPage_DataContextChanged;
        }

        private MainPageViewModel _oldViewModel;

        private void MainPage_DataContextChanged(Windows.UI.Xaml.FrameworkElement sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
        {
            if (_oldViewModel == null & args.NewValue is MainPageViewModel)
            {
                _oldViewModel = (MainPageViewModel)args.NewValue;
                _oldViewModel.SelectedListChanged += _oldViewModel_SelectedListChanged;
            }

            if (args.NewValue == null && _oldViewModel != null)
            {
                _oldViewModel.SelectedListChanged -= _oldViewModel_SelectedListChanged;
            }
        }

        private void _oldViewModel_SelectedListChanged(object sender, TodoListViewModel e)
        {
            var list = ListViewTodoLists.ContainerFromItem(e);
            var index = ListViewTodoLists.IndexFromContainer(list);
            ListViewTodoLists.SelectedIndex = index;
        }

        private void ListView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            ViewModel.GotoList(((TodoListViewModel)e.ClickedItem).TodoList.Id);
        }
    }
}
