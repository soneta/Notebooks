using Soneta.Business.UI;

namespace SonetaBudget.UI.UIElements.Builders
{
    internal abstract class ProjectBudgetTagsBuilderBase<T> where T: ContainerElement
    {
        public ProjectBudgetTagsBuilderBase(T component)
            => Component = component;

        protected T Component { get; }

        public virtual T CreateComponent()
            => Component;
    }
}
