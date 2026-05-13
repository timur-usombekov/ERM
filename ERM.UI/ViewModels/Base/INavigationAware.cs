using System.Threading.Tasks;

namespace ERM.UI.ViewModels.Base
{
    public interface INavigationAware
    {
        Task OnNavigatedToAsync();
    }
}