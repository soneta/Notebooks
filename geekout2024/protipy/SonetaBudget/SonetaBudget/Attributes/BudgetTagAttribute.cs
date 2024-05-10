using Soneta.Business;
using SonetaBudget.Models.Database;
using SonetaBudget.Models.Database.Config;
using System;

namespace SonetaBudget.Attributes
{
    [Serializable]
    public class BudgetTagAttribute : NewRowAttribute
    {
        public BudgetTagAttribute(DefBudgetTag definition)
            : base(definition.ToString(), typeof(BudgetTag))
        {
            Definition = definition;
            Priority = (int)definition.Type;
        }

        public DefBudgetTag Definition { get; }

        public override void InitContext(Context context)
        {
            base.InitContext(context);
            context.Set(context.Session[Definition]);
        }
    }
}
