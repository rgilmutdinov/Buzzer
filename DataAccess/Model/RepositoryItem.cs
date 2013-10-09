using Common;

namespace DataAccess.Model
{
   public abstract class RepositoryItem
   {
      protected RepositoryItem()
      {
         Id = NullValues.Id;
      }

      public int Id { get; internal set; }

      public bool IsNew
      {
         get { return Id == NullValues.Id; }
      }
   }
}