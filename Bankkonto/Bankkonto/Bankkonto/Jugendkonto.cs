using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public class Jugendkonto : Konto
{
    protected override void BeziehenIntern(decimal betrag)
    {
        PruefeGuthaben(betrag, "Ein Jugendkonto kann nicht überzogen werden.");

        Transaktionsdatum = DateTime.Now;
        Guthaben -= betrag;
    }

    public Jugendkonto(KontoArt kontoArt = KontoArt.Standard) : base(kontoArt) 
    {
        KontoArt = kontoArt;
    }

    public Jugendkonto(decimal startGuthaben, KontoArt kontoArt) : base(startGuthaben, kontoArt)
    {
        Eroeffnungsdatum = DateTime.Now;
        Guthaben = startGuthaben;
        KontoArt = kontoArt;
    }
}
