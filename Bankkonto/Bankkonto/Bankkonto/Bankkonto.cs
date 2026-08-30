using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public class Bankkonto
{
    public string KontoNummer { get; private set; } = Guid.NewGuid().ToString();
    public decimal Guthaben { get; private set; }
    public static decimal AktivZins { get; set; }
    public static decimal PassivZins { get; set; }


    public void Einzahlen(decimal betrag)
    {
        if (betrag <= 0)
        {
            throw new ArgumentException("Der Betrag muss grösser als 0 sein.");
        }

        Guthaben += betrag;
    }

    public void Beziehen(decimal betrag)
    {
        if (betrag <= 0)
        {
            throw new ArgumentException("Der Betrag muss grösser als 0 sein.");
        }

        if (betrag > Guthaben)
        {
            throw new InvalidOperationException("Nicht genügend Guthaben auf dem Konto.");
        }

        Guthaben -= betrag;
    }

    public void Transferieren(Bankkonto konto, decimal betrag)
    {
        if (betrag <= 0)
        {
            throw new ArgumentException("Der Betrag muss grösser als 0 sein.");
        }

        if (betrag > Guthaben)
        {
            throw new InvalidOperationException("Nicht genügend Guthaben auf dem Konto.");
        }

        Guthaben -= betrag;
        konto.Einzahlen(betrag);
    }

    public void Zinsgutschreibung(int anzahlTage)
    {
        if (Guthaben > 0)
        {
            Guthaben += Guthaben * AktivZins * anzahlTage / 365;
        }
        else if (Guthaben < 0)
        {
            Guthaben += Guthaben * PassivZins * anzahlTage / 365;
        }
    }

    public void Kontoabschliessen()
    {
        Guthaben = 0;
    }

}
