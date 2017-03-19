﻿using ABC.Navigation;
using System.Collections.Generic;

namespace SolidNavigation.Navigation
{
    public class Bootstrapper
    {
        private Navigator _navigator;
        public Navigator Navigator => _navigator;
        public UrlSerializer UrlSerializer { get; }

        private List<TodoList> _lists = new List<TodoList>
        {
            new TodoList { Id=1, Title="Groceries"},
            new TodoList { Id=2, Title="Movies"},
            new TodoList { Id=3, Title="Holidays"}
        };

        private List<Todo> _todos = new List<Todo>
        {
            new Todo{ Id=1, ListId=1, Title="Cookies"},
            new Todo{ Id=2, ListId=1, Title="Bananas"},
            new Todo{ Id=3, ListId=1, Title="Water"},
            new Todo{ Id=4, ListId=2, Title="The Godfather"},
            new Todo{ Id=5, ListId=2, Title="Casino"},
            new Todo{ Id=6, ListId=2, Title="Wag the dog"},
            new Todo{ Id=7, ListId=3, Title="USA"},
            new Todo{ Id=8, ListId=3, Title="Tallin"},
        };

        public Bootstrapper()
        {
            UrlSerializer = new UrlSerializer("journal://", new List<Route> {

            _navigator = new Navigator(UrlSerializer);
        }
    }
}