namespace UTB_social_network_Dudik.Models
{
    public class MainPageViewModel
    {
        public List<ChatViewModel> UserChats { get; set; } = new List<ChatViewModel>();
    }

    public class ChatViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGroupChat { get; set; }
    }
}
