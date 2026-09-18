using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public class Sparkonto : Konto
{
    public Sparkonto(KontoArt kontoArt = KontoArt.Standard) : base(kontoArt)
    {
        KontoArt = kontoArt;
    }
    public Sparkonto(decimal startGuthaben, KontoArt kontoArt) : base(startGuthaben, kontoArt)
    {
        if (startGuthaben < 0)
        {
            throw new ArgumentException(
                "Das Startguthaben darf nicht negativ sein.");
        }

        Eroeffnungsdatum = DateTime.Now;
        Guthaben = startGuthaben;
        KontoArt = kontoArt;
    }
    public override void Beziehen(decimal betrag)
    {
        if (betrag <= 0)
        {
            throw new ArgumentException(
                "Der Betrag muss grösser als 0 sein.");
        }

        if (betrag > Guthaben)
        {
            throw new InvalidOperationException(
                "Ein Sparkonto kann nicht überzogen werden.");
        }

        Transaktionsdatum = DateTime.Now;
        Guthaben -= betrag;
    }
}
