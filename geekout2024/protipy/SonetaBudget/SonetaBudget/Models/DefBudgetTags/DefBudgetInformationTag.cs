using Soneta.Business;
using SonetaBudget.Enums;
using SonetaBudget.Models.Database.Config;
using SonetaBudget.Models.DefBudgetTags;

[assembly: BusinessRow(typeof(DefBudgetInformationTag), TagType.Information)]

namespace SonetaBudget.Models.DefBudgetTags
{
    public class DefBudgetInformationTag : DefBudgetTag
    {
        public DefBudgetInformationTag(RowCreator creator) : base(creator)
        {
        }

        public DefBudgetInformationTag() : base(TagType.Information)
        {
        }
    }
}
