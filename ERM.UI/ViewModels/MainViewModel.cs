using ERM.Infrastructure;

namespace ERM.UI.ViewModels
{
    public class MainViewModel
    {
        private readonly AppDbContext _context;

        public MainViewModel(AppDbContext context)
        {
            _context = context;
        }
    }
}