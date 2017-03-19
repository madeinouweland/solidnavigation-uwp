using ABC.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidNavigation.Navigation
{
    public class Navigator
    {
        private UrlSerializer _urlSerializer;

        public Navigator(UrlSerializer router)
        {
            _urlSerializer = router;
        }

        public void ToUrl(string url)
        {
            ToTarget(_urlSerializer.CreateNavigationTarget(url));
        }

        private void ToTarget(INavigationTarget target)
        {
            Navigated?.Invoke(this, target);
            switch (target)
            {
                case ListTarget l:
                    SelectedListChanged?.Invoke(this, l.ListId);
                    break;
                default:
                    break;
            }
        }

        public void ToHome()
        {
            ToTarget(new HomeTarget());
        }

        public void ToList(int id)
        {
            ToTarget(new ListTarget(id));
        }

        public void ToSettings()
        {
            ToTarget(new SettingsTarget());
        }

        public event EventHandler<INavigationTarget> Navigated;

        public event EventHandler<int> SelectedListChanged;

    }
}
