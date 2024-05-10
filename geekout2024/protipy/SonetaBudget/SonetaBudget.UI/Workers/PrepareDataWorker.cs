using Soneta.Business;
using Soneta.Core;
using Soneta.Tools;
using Soneta.Zadania;
using Soneta.Zadania.Budzetowanie;
using Soneta.Zadania.UI;
using SonetaBudget.Budget;
using SonetaBudget.Models.Database;
using System;

// Należy odkomentować rejestrację workera, aby była widoczna akcja na liście projektów
[assembly: Worker(typeof(PrepareDataWorker), typeof(Projekty))]

namespace Soneta.Zadania.UI
{
    internal class PrepareDataWorker
    {
        [Context]
        public Session Session { get; set; }

        [Action("Przygotuj dane",
            Priority = 1,
            Icon = ActionIcon.ConfirmMessage,
            Mode = ActionMode.SingleSession | ActionMode.Progress,
            Target = ActionTarget.ToolbarWithText)]
        public void ConfirmNotification()
        {
            using (var transaction = Session.Logout(true))
            {
                for (var i = 1; i < 10000; i++)
                {
                    var Budzetowany = new Projekt
                    {
                        Nazwa = "Projekt ".TranslateIgnore() + i.ToString()
                    };

                    Session.AddRow(Budzetowany);
                    Budzetowany.Definicja = Session.GetZadania().DefProjektow.WgSymbolu.GetFirst();

                    var projectBudget = new BudzetProjektu
                    {
                        Projekt = Budzetowany
                    };

                    Session.AddRow(projectBudget);

                    projectBudget.Symbol = Budzetowany.Symbol.Replace("?", i + 1.ToString());
                    projectBudget.Nazwa = Budzetowany.Nazwa;
                    projectBudget.Podstawowy = true;
                    projectBudget.Stan = StanBudzetu.Bufor;

                    var planVersion = new WersjaPlanu(projectBudget, DefPlanVersions.GetBasicSystemDefinition(Budzetowany.Session), WersjePlanu.BasePlanVersionName);

                    Session.AddRow(planVersion);

                    if (i % 3 == 0)
                        Session.AddRow(new BudgetTag(projectBudget, Session.GetSonetaBudget().DefBudgetTags.BySymbol["ERR"].GetFirst()) { Description = "Do poprawy" });
                    else if (i % 2 == 0)
                        Session.AddRow(new BudgetTag(projectBudget, Session.GetSonetaBudget().DefBudgetTags.BySymbol["WAR"].GetFirst()) { Description = "Do sprawdzenia" });
                    else
                        Session.AddRow(new BudgetTag(projectBudget, Session.GetSonetaBudget().DefBudgetTags.BySymbol["INFO"].GetFirst()) { Description = "Do akceptacji" });
                }

                transaction.CommitUI();
            }
        }
    }
}
