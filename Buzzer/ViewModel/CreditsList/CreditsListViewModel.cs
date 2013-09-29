using System.Collections.ObjectModel;
using System.Linq;
using Buzzer.Common;
using Buzzer.DataAccess;
using Buzzer.ViewModel.Common;

namespace Buzzer.ViewModel.CreditsList
{
   public sealed class CreditsListViewModel : WorkspaceViewModel
   {
      private readonly CreditRepository _repository;

      public CreditsListViewModel(CreditRepository repository)
      {
         Check.NotNull(repository, "repository");
         _repository = repository;

         CreditsList = new ObservableCollection<CreditViewModel>(_repository.QueryDatabase(queryCredits));
      }

      public ObservableCollection<CreditViewModel> CreditsList { get; private set; }

      private CreditViewModel[] queryCredits(BuzzerDatabaseEntities database)
      {
         var query =
            from personsToCredit in database.PersonsToCredits
            join person in database.Persons on personsToCredit.PersonID equals person.ID
            join credit in database.Credits on personsToCredit.CreditID equals credit.ID
            where personsToCredit.IsBorrower
            select new
                      {
                         Credit = credit,
                         Person = person
                      };

         return
            query
               .ToArray()
               .Select(
                  item => new CreditViewModel(
                             item.Credit.CreditNumber,
                             item.Person.Name,
                             item.Credit.CreditAmount,
                             item.Credit.CreditIssueDate,
                             item.Credit.CreditIssueDate,
                             item.Credit.DiscountRate
                             )
               )
               .ToArray();
      }
   }
}