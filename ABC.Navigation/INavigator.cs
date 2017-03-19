using System;

namespace ABC.Navigation
{
    public interface INavigator
    {
        void ToUrl(string url);
        event EventHandler<INavigationTarget> Navigated;
    }
}
