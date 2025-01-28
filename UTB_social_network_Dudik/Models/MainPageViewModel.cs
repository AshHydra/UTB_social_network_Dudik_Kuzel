namespace UTB_social_network_Dudik.Models
{
    public class MainPageViewModel
    {
        public List<ChatViewModel> Chats { get; set; } = new List<ChatViewModel>(); // Ensure it's a list
    }
    public class ChatViewModel
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
    }
}
