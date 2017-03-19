using ABC.Navigation;

namespace SolidNavigation.Navigation
{
    public class HomeTarget : INavigationTarget { }

    public class ListTarget : INavigationTarget
    {
        public int ListId { get; private set; }

        public ListTarget(int listId)
        {
            ListId = listId;
        }
    }

    public class SettingsTarget : INavigationTarget { }
}
