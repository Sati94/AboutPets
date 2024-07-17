using WebShopAPI.Model.TodoItem.TodoItemStatus;

namespace WebShopAPI.Model.TodoItem
{
    public class TodoItem
    {
        public int Id { get; set; }

        public string Sender { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public TodoStatus Status { get; set; }
    }
}
