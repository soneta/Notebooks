using Soneta.Types;

namespace SonetaBudget.Enums
{
    public enum TagType
    {
        [Caption("Informacyjny")]
        Information = 1,
        [Caption("Ostrzegający")]
        Warning,
        [Caption("Błędny")]
        Error
    }
}
