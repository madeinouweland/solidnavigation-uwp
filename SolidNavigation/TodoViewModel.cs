namespace SolidNavigation
{
    public class TodoViewModel
    {
        public Todo Todo { get; set; }
        public string Title => Todo.Title;
    }
}
