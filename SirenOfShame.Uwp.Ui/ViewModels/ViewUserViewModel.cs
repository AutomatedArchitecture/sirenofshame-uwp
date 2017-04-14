using SirenOfShame.Uwp.Ui.Models;

namespace SirenOfShame.Uwp.Ui.ViewModels
{
    public class ViewUserViewModel
    {
        private readonly PersonDto _person;

        public ViewUserViewModel(PersonDto person)
        {
            _person = person;
        }

        public string DisplayName => _person.DisplayName;
    }
}
