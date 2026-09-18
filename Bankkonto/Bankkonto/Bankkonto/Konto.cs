using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public enum KontoArt
{
    VIP,
    Standard,
}

public abstract class Konto : IKonto
{
    public const decimal MinimaleStandardBezugslimite = 0m;
    public const decimal MaximaleStandardBezugslimite = 10_000m;
    public string KontoNummer { get; } = Guid.NewGuid().ToString();
    public decimal Guthaben { get; protected set; } = 0m;
    public decimal Bezugslimite { get; private set; }
    public DateTime BezugslimiteGueltigAb { get; private set; }
    public static decimal AktivZins { get; set; }
    public static decimal PassivZins { get; set; }
    public KontoArt KontoArt { get; set; }
    public DateTime Eroeffnungsdatum { get; set; }
    public DateTime Abschlussdatum { get; set; }
    public DateTime Transaktionsdatum { get; set; }
    public bool IstGeschlossen { get; set; } = false;

    protected Konto(KontoArt kontoArt = KontoArt.Standard)
    {
        KontoArt = kontoArt;
        Eroeffnungsdatum = DateTime.Now;
    }

    protected Konto(decimal startGuthaben, KontoArt kontoArt)
        : this(kontoArt)
    {
        PruefeStartguthaben(startGuthaben);
        Guthaben = startGuthaben;
    }

    public virtual void Einzahlen(decimal betrag)
    {
        PruefeBetrag(betrag);

        Transaktionsdatum = DateTime.Now;
        Guthaben += betrag;
    }

    public void Beziehen(decimal betrag)
    {
        PruefeBetrag(betrag);
        BeziehenIntern(betrag);
    }

    protected abstract void BeziehenIntern(decimal betrag);

    protected void PruefeGuthaben(decimal betrag, string fehlermeldung)
    {
        if (betrag > Guthaben)
        {
            throw new InvalidOperationException(fehlermeldung);
        }
    }

    public void setzeBezugslimite(decimal neueBezugslimite, DateTime gueltigAb)
    {
        if (neueBezugslimite < MinimaleStandardBezugslimite ||
            neueBezugslimite > MaximaleStandardBezugslimite)
        {
            throw new ArgumentOutOfRangeException(
                nameof(neueBezugslimite),
                "Die Bezugslimite muss innerhalb der Standardlimiten liegen.");
        }

        Bezugslimite = neueBezugslimite;
        BezugslimiteGueltigAb = gueltigAb;
    }

    public virtual void Transferieren(IKonto konto, decimal betrag)
    {
        if (konto == null)
        {
            throw new ArgumentNullException(nameof(konto));
        }

        PruefeBetrag(betrag);

        if (konto == this)
        {
            throw new InvalidOperationException("Ein Konto kann nicht an sich selbst übertragen.");
        }

        var originalGuthaben = Guthaben;
        try
        {
            konto.Einzahlen(betrag);
            Beziehen(betrag);
        }
        catch
        {
            Guthaben = originalGuthaben;
            throw;
        }

        Transaktionsdatum = DateTime.Now;
    }

    private static void PruefeBetrag(decimal betrag)
    {
        if (betrag <= 0)
        {
            throw new ArgumentException(
                "Der Betrag muss grösser als 0 sein.");
        }
    }

    private static void PruefeStartguthaben(decimal startGuthaben)
    {
        if (startGuthaben < 0)
        {
            throw new ArgumentException(
                "Das Startguthaben darf nicht negativ sein.");
        }
    }

    public virtual void Zinsgutschreibung(int anzahlTage)
    {
        decimal zinssatz;

        if (Guthaben >= 0m && Guthaben < 10_000m)
        {
            zinssatz = AktivZins;
        }
        else if (Guthaben >= 10_000m && Guthaben < 50_000m)
        {
            zinssatz = AktivZins + 0.005m;
        }
        else // Grösser als 50_000.
        {
            if (KontoArt == KontoArt.VIP)
            {
                zinssatz = AktivZins + 0.015m;
            }
            else
            {
                zinssatz = AktivZins + 0.0075m;
            }
        }

        decimal zins = Guthaben * zinssatz * anzahlTage / 365m;

        zins = Math.Round(zins, 2);

        Transaktionsdatum = DateTime.Now;
        Guthaben += zins;
    }

    public virtual void Kontoabschliessen()
    {
        if (IstGeschlossen)
        {
            throw new InvalidOperationException(
                "Das Konto ist bereits abgeschlossen.");
        }
        
        if (Guthaben != 0)
        {
            throw new InvalidOperationException(
                "Das Konto kann nur abgeschlossen werden, wenn das Guthaben 0 ist.");
        }

        Abschlussdatum = DateTime.Now;
        IstGeschlossen = true;
    }
}
