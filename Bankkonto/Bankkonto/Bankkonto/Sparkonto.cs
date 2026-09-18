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
        Eroeffnungsdatum = DateTime.Now;
        Guthaben = startGuthaben;
        KontoArt = kontoArt;
    }
    protected override void BeziehenIntern(decimal betrag)
    {
        PruefeGuthaben(betrag, "Ein Sparkonto kann nicht überzogen werden.");

        Transaktionsdatum = DateTime.Now;
        Guthaben -= betrag;
    }
}
