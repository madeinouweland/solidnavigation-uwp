using SolidNavigation.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SolidNavigation
{
    public class MainPageViewModel
    {
        private Navigator _navigator;
        private List<Todo> _todos;

        public List<TodoListViewModel> TodoLists { get; set; }
        public ObservableCollection<TodoViewModel> Todos { get; set; } = new ObservableCollection<TodoViewModel>();

        public event EventHandler<TodoListViewModel> SelectedListChanged;

        public MainPageViewModel(Navigator navigator, List<TodoList> lists, List<Todo> todos)
        {
            _navigator = navigator;

            _todos = todos;

            TodoLists = (from l in lists
                         select new TodoListViewModel { TodoList = l }).ToList();

            navigator.SelectedListChanged += Navigator_SelectedListChanged;
        }

        private void Navigator_SelectedListChanged(object sender, int listId)
        {
            Todos.Clear();
            foreach (var todo in _todos.Where(x => x.ListId == listId))
            {
                Todos.Add(new TodoViewModel { Todo = todo });
            }

            var vm = TodoLists.FirstOrDefault(x => x.TodoList.Id == listId);
            SelectedListChanged?.Invoke(this, vm);
        }

        public void GotoSettingsPage()
        {
            _navigator.ToSettings();
        }

        public void GotoList(int id)
        {
            _navigator.ToList(id);
        }
    }
}
