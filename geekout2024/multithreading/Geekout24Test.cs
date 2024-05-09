using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Magazyny;
using Soneta.Test;
using Soneta.Tools;
using Soneta.Towary;
using Soneta.Types;

// ReSharper disable AccessToDisposedClosure

namespace Soneta.Business.Test;

public class Geekout24Test : TestBase {
    private readonly List<string> kody = new();

    [Test]
    public async Task Geekout24_01_HowToStartNewThread_ProblemWithSessionState() {
        SessionState.Current.CurrentCulture = CultureInfo.GetCultureInfo("fr-fr");
        WriteLine($"Main thread {SessionState.Current.CurrentCulture}");

        await Task.Run(() => {
            try {
                WriteLine($"Task thread {SessionState.Current.CurrentCulture}");

                Assert.Fail("Never called");
            }
            catch (InvalidOperationException) {
                WriteLine("Exception thrown");
            }
        });

        Assert.Pass("SessionState props cannot be read in thread/task\n");
    }

    [Test]
    public async Task Geekout24_02_HowToStartNewThread_StandardWay() {
        var sstate = SessionState.GetThreadState();

        SessionState.Current.CurrentCulture = CultureInfo.GetCultureInfo("fr-fr");
        WriteLine($"Main thread {SessionState.Current.CurrentCulture}");

        await Task.Run(() => {
            using (LogTimer.StartThreadInfo("Test Thread"))
            using (sstate?.AttachThread()) {
                WriteLine($"Task thread {SessionState.Current.CurrentCulture}");
            }
        });

        Assert.Pass("SessionState props can be read in thread/task when state has attached\n");
    }

    [Test]
    public async Task Geekout24_03_HowToStartNewThread_BetterWay() {
        SessionState.Current.CurrentCulture = CultureInfo.GetCultureInfo("fr-fr");
        WriteLine($"Main thread {SessionState.Current.CurrentCulture}");

        await Concurrent.RunTask(() => { WriteLine($"Task thread {SessionState.Current.CurrentCulture}"); });

        Assert.Pass("SessionState props can be read in task when tash is run by Concurrent.RunTask\n");
    }

    [Test]
    public void Geekout24_04_HowToStartNewThread_ProducerConsumer() {
        var collection = new BlockingCollection<int>();

        var tasks = new Task[3];

        tasks[0] = Concurrent.RunTask(() => {
            for (int i = 0; i < 3; ++i)
                collection.Add(i);
        });
        tasks[1] = Concurrent.RunTask(() => {
            for (int i = 0; i < 3; ++i)
                collection.Add(10 + i);
        });
        tasks[2] = Concurrent.RunTask(() => {
            for (int i = 0; i < 3; ++i)
                collection.Add(20 + i);
        });
        Concurrent.RunTask(() => {
            Task.WaitAll(tasks);
            collection.CompleteAdding();
        }).Wait();

        var arr = new List<int>();

        foreach (int i in collection.GetConsumingEnumerable())
            arr.Add(i);

        WriteLine(arr.Select(i => i.ToString()).Join(", "));

        Assert.AreEqual(9, arr.Count, "There had been 9 values produced");
    }

    //
    //
    //
    //
    //

    private void BuildSessionWithSave(string kod) {
        using Session session = Login.CreateSession(false, false, "Producer Session");

        using (var transaction = session.Logout(true)) {
            var towar = session.GetTowary().Towary.WgKodu[kod];

            WriteLine($"Task Towar: {towar.Kod} - '{towar.NumerKatalogowy}'");
            towar.NumerKatalogowy = "Numer: " + kod.ToLower();

            transaction.Commit();
        }

        session.Save();
    }

    [Test]
    public async Task Geekout24_05_UpdateIndependedValues_WithIndividualSaves() {
        var tasks = new[] {
            Concurrent.RunTask(() => BuildSessionWithSave(kody[0])),
            Concurrent.RunTask(() => BuildSessionWithSave(kody[1])),
            Concurrent.RunTask(() => BuildSessionWithSave(kody[2]))
        };
        await Task.WhenAll(tasks);

        WriteTowary("After saves");
    }

    //
    //
    //
    //
    //

    private Session BuildSessionWithoutSave(string kod) {
        Session session = Login.CreateSession(false, false, "Producer Session");

        try {
            using var transaction = session.Logout(true);
            var towar = session.GetTowary().Towary.WgKodu[kod];

            WriteLine($"Task Towar: {towar.Kod} - '{towar.NumerKatalogowy}'");
            towar.NumerKatalogowy = "Numer: " + kod.ToLower();

            transaction.Commit();
        }
        catch {
            //
            // Remember to dispose session in case of exception
            //
            session.Dispose();
            throw;
        }

        VerifySession(session);

        return session;
    }

    private void VerifySession(Session session) {
        using (ITransaction tt = session.Logout(true)) {
            session.Events.Invoke();
            tt.Commit();
        }

        session.Verifiers.ValidateVerifiers();
    }

    [Test]
    public async Task Geekout24_06_UpdateIndependedValues_WithSingleDbTransationSave() {
        var sessions = await Task.WhenAll([
            Concurrent.RunTask(() => BuildSessionWithoutSave(kody[0])),
            Concurrent.RunTask(() => BuildSessionWithoutSave(kody[1])),
            Concurrent.RunTask(() => BuildSessionWithoutSave(kody[2]))
        ]);

        WriteTowary("Before save");

        using (var dbTransaction = Database.Logout()) {
            foreach (Session session in sessions)
                session.Save();

            dbTransaction.Commit();
        }

        WriteTowary("After save");
    }

    //
    //
    //
    //
    //

    private void BuildSessionWithJoinToMaster(CountdownEvent countdown, Session masterSession, string kod) {
        using Session session = Login.CreateSession(false, false, "Slave Session");

        session.JoinTo(masterSession);

        using (var transaction = session.Logout(true)) {
            var towar = session.GetTowary().Towary.WgKodu[kod];

            WriteLine($"Task Towar: {towar.Kod} - '{towar.NumerKatalogowy}'");
            towar.NumerKatalogowy = "Numer: " + kod.ToLower();

            transaction.Commit();
        }

        countdown.Signal();

        session.Save();
    }

    [Test]
    public void Geekout24_07_UpdateIndependedValues_WithJoinToMasterSession() {
        var countdown = new CountdownEvent(3);

        using Session masterSession = Login.CreateSession(false, false, "Master Session");

        Concurrent.RunTask(() => BuildSessionWithJoinToMaster(countdown, masterSession, kody[0]));
        Concurrent.RunTask(() => BuildSessionWithJoinToMaster(countdown, masterSession, kody[1]));
        Concurrent.RunTask(() => BuildSessionWithJoinToMaster(countdown, masterSession, kody[2]));

        countdown.Wait();

        WriteTowary("Before save");

        masterSession.Save();

        WriteTowary("After save");
    }

    //
    //
    //
    //
    //

    [Test]
    public void Geekout24_08_DependedValues_UsingRowPack() {
        using Session dokumentSession = Login.CreateSession(false, false, "Dokument Session");
        using Session kontrahentSession = Login.CreateSession(false, false, "Kontrahent Session");

        var kontrahentPack = CreateKontrahent(kontrahentSession);

        CreateDokument(dokumentSession, kontrahentPack);

        kontrahentSession.Save();
        dokumentSession.Save();

        WriteLine("=================== Result ===================");
        var dokument = Session.GetHandel().DokHandlowe.WgDaty[Date.Today]
            .Single(dok => dok.Definicja.Symbol == "ZK");
        WriteLine($"Dokument: {dokument.Numer} - {dokument.Kontrahent.Kod} - {dokument.Kontrahent.Nazwa}");
    }

    private RowPack<Kontrahent> CreateKontrahent(Session session) {
        var kontrahent = new Kontrahent();

        using (var transaction = session.Logout(true)) {
            session.GetCRM().Kontrahenci.AddRow(kontrahent);
            kontrahent.Kod = "Kod1";
            kontrahent.Nazwa = "Nowy kontrahent";
            transaction.Commit();
        }

        var pack = kontrahent.Pack(RowPackOptions.RelatedChanges);

        return pack;
    }

    private void CreateDokument(Session session, RowPack<Kontrahent> kontrahentPack) {
        var kontrahent = kontrahentPack.UnpackTo(session);

        using var transaction = session.Logout(true);
        var dok = new DokumentHandlowy();
        session.GetHandel().DokHandlowe.AddRow(dok);

        dok.Definicja = session.GetHandel().DefDokHandlowych.WgSymbolu["ZK"];
        dok.Magazyn = session.GetMagazyny().Magazyny.WgSymbol["F"];
        dok.Kontrahent = kontrahent;
        dok.Data = Date.Today;

        transaction.Commit();
    }

    //
    //
    //
    //
    //

    private const int NTowarsByThread = 5;
    private int tindex;

    private void TowarProducer(CountdownEvent countdown, Session masterSession,
        BlockingCollection<RowPack<Towar>> towary) {
        using var session = Login.CreateSession(false, false, "Towar generator");
        session.JoinTo(masterSession);

        for (int nr = 0; nr < NTowarsByThread; ++nr) {
            Towar towar;

            using (var t1 = session.Logout(true)) {
                towar = new Towar();

                session.AddRow(towar);

                int index = Interlocked.Increment(ref tindex);
                towar.Kod = "Kod " + index;
                towar.Nazwa = "Nazwa " + index;

                towar.Ceny["Podstawowa", Date.Today].Netto = 100 + index;

                t1.CommitUI();
            }

            towary.Add(towar.Pack(RowPackOptions.RelatedChanges));
            WriteLine("Produced: " + towar.Kod + " - " + towar.Nazwa);

            Thread.Sleep(Random.Shared.Next(300));
        }

        countdown.Signal();

        session.Save();
    }

    private void DokumentConsumer(CountdownEvent countdown, Session masterSession,
        BlockingCollection<RowPack<Towar>> towary) {
        using var session = Login.CreateSession(false, false, "Dokument builder");
        session.JoinTo(masterSession);

        using (var t = session.Logout(true)) {
            var dok = new DokumentHandlowy();
            session.AddRow(dok);
            dok.Definicja = session.GetHandel().DefDokHandlowych.WgSymbolu["ZK"];
            dok.Magazyn = session.GetMagazyny().Magazyny.WgSymbol["F"];
            dok.Kontrahent = session.GetCRM().Kontrahenci.WgKodu["ABC"];
            dok.Data = Date.Today;

            foreach (var towarPack in towary.GetConsumingEnumerable()) {
                var poz = new PozycjaDokHandlowego(dok);
                session.AddRow(poz);

                var towar = towarPack.UnpackTo(session);
                WriteLine("Consumed: " + towar.Kod + " - " + towar.Nazwa);

                poz.Towar = towar;
            }

            t.CommitUI();
        }

        countdown.Signal();

        session.Save();
    }

    [Test]
    public void Geekout24_09_DependedValues_ProducerConsumerScenario() {
        var producerCountdown = new CountdownEvent(3);
        var consumerCountdown = new CountdownEvent(1);

        using Session masterSession = Login.CreateSession(false, false, "Master Session");

        var towary = new BlockingCollection<RowPack<Towar>>();

        Concurrent.RunTask(() => TowarProducer(producerCountdown, masterSession, towary));
        Concurrent.RunTask(() => TowarProducer(producerCountdown, masterSession, towary));
        Concurrent.RunTask(() => TowarProducer(producerCountdown, masterSession, towary));

        Concurrent.RunTask(() => DokumentConsumer(consumerCountdown, masterSession, towary));

        producerCountdown.Wait();

        towary.CompleteAdding();

        consumerCountdown.Wait();

        masterSession.Save();

        WriteLine("=================== Result ===================");
        var dokument = Session.GetHandel().DokHandlowe.WgDaty[Date.Today]
            .Single(dok => dok.Definicja.Symbol == "ZK");
        foreach (var poz in dokument.Pozycje)
            WriteLine(
                $"Pozycja: {poz.Towar.Kod} - {poz.Towar.Nazwa} - {poz.Towar.Ceny["Podstawowa", Date.Today].Netto}");
    }

    //
    //
    //
    //
    //

    [Test]
    public void Geekout24_10_BatchOperation_WithPartitions() {
        using Session masterSession = Login.CreateSession(false, false, "Master Session");

        // select ID from Towary
        var ids = masterSession.GetTowary().Towary.AsQuery()
            .Select(t => t.ID)
            .ToList();

        Assert.AreEqual(0, masterSession.GetTowary().Towary.Rows.Loaded.Count,
            "Brak załadowanych towarów, pomimo zapytania o ID");

        var partitions = ids
            .Select((towar, index) => (Towar: towar, Index: index))
            .GroupBy(x => x.Index / 20, x => x.Towar)
            .ToList();

        //
        // Pamiętaj, żeby rekordy zależne umieszczać w jednej partycji
        //
        var countdown = new CountdownEvent(partitions.Count);

        var tasks = partitions.Select(
            p => Concurrent.RunTask(() => UpdatePartition(p, countdown, masterSession)))
            .ToArray();

        countdown.Wait();

        masterSession.Save();

        Task.WaitAll(tasks);

        WriteTowary("Po zmianach");
    }

    private void UpdatePartition(IEnumerable<int> partition, CountdownEvent countdown, Session masterSession) {
        using Session session = Login.CreateSession(false, false, "Partition Session");
        session.JoinTo(masterSession);

        var towary = session.GetTowary().Towary[partition.ToArray()];

        WriteLine($"Partition: {towary.Select(t => t.Kod).Join(",")}");

        using (var transaction = session.Logout(true)) {
            foreach (var towar in towary) {
                towar.NumerKatalogowy = $"Numer: {towar.Kod}: {towar.ID}";
                WriteLine($"Zmiana: {towar.Kod}");

                Thread.Sleep(Random.Shared.Next(100));
            }

            transaction.Commit();
        }

        VerifySession(session);

        countdown.Signal();

        session.Save();
    }

    //
    //
    //
    //
    //

    #region Helpers

    protected override bool EnableDbTransation => false;

    private void WriteLine(string message) => Console.WriteLine(message);

    private IDisposable testState;

    public override void TestSetup() {
        base.TestSetup();
        testState = SessionState.CreateAndAttach();
        PrepareTowary();
        CleanDokumenty();
    }

    public override void TestTearDown() {
        testState.Dispose();
        base.TestTearDown();
        CleanDokumenty();
    }

    private void PrepareTowary() {
        using Session session = Login.CreateSession(false, false, "Producer Session");
        using (var transaction = session.Logout(true)) {
            foreach (var towar in session.GetTowary().Towary.WgKodu.GetNextRows(null, 3, out _).Cast<Towar>()) {
                kody.Add(towar.Kod);
                towar.NumerKatalogowy = "";
            }

            transaction.Commit();
        }

        session.Save();
    }

    private void CleanDokumenty() {
        SaveDispose();

        using (var t = Session.Logout(true)) {
            foreach (DokumentHandlowy dok in Session.GetHandel().DokHandlowe.WgDaty[Date.Today]) {
                if (dok.Stan == StanDokumentuHandlowego.Zatwierdzony)
                    dok.Stan = StanDokumentuHandlowego.Bufor;
            }

            t.Commit();
        }

        using (var t = Session.Logout(true)) {
            foreach (DokumentHandlowy dok in Session.GetHandel().DokHandlowe.WgDaty[Date.Today]) {
                if (dok.Definicja.Symbol == "ZK")
                    dok.Delete();
            }

            t.Commit();
        }

        SaveDispose();

        using (var t = Session.Logout(true)) {
            foreach (Towar towar in Session.GetTowary().Towary.WgNazwy[new FieldCondition.Like("Kod", "Kod*")]) {
                towar.Delete();
            }

            t.Commit();
        }

        SaveDispose();

        using (var t = Session.Logout(true)) {
            var k = Session.GetCRM().Kontrahenci.WgKodu["Kod1"];
            k?.Delete();

            t.Commit();
        }

        SaveDispose();
    }

    private void WriteTowary(string message) {
        WriteLine("================= " + message + " =================");
        using Session session = Login.CreateSession(false, false, "Write Log");
        foreach (var towar in session.GetTowary().Towary.WgKodu.GetNextRows(null, 3, out _).Cast<Towar>()) {
            WriteLine($"Towar: {towar.Kod}, numer katalogowy: '{towar.NumerKatalogowy}'");
        }
    }

    #endregion
}
