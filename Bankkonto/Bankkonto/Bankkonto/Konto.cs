using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public enum KontoStatus
{
    VIP,
    Standard,
}

public abstract class Konto : IKonto
{
    public string KontoNummer { get; } = Guid.NewGuid().ToString();
    public decimal Guthaben { get; protected set; }
    public static decimal AktivZins { get; set; }
    public static decimal PassivZins { get; set; }
    public KontoStatus Status { get; set; }

    public virtual void Einzahlen(decimal betrag)
    {
        if (betrag <= 0)
        {
            throw new ArgumentException(
                "Der Betrag muss grösser als 0 sein.");
        }

        Guthaben += betrag;
    }

    public abstract void Beziehen(decimal betrag);

    public virtual void Transferieren(IKonto konto, decimal betrag)
    {
        if (konto == null)
        {
            throw new ArgumentNullException(nameof(konto));
        }

        if (betrag <= 0)
        {
            throw new ArgumentException(
                "Der Betrag muss grösser als 0 sein.");
        }

        Beziehen(betrag);
        konto.Einzahlen(betrag);
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
        else if (Guthaben >= 50_000m && Guthaben < 100_000m)
        {
            if (Status == KontoStatus.VIP)
            {
                zinssatz = AktivZins + 0.015m;
            }
            else
            {
                zinssatz = AktivZins + 0.0075m;
            }
        }
        else
        {
            return;
        }

        decimal zins = Guthaben * zinssatz * anzahlTage / 365m;

        zins = Math.Round(zins, 2);

        Guthaben += zins;
    }

    public virtual void Kontoabschliessen()
    {
        Guthaben = 0;
    }
}
