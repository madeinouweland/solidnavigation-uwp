namespace SolidNavigation
{
    public class TodoListViewModel
    {
        public TodoList TodoList { get; set; }
        public string Title => TodoList.Title;
    }
}
