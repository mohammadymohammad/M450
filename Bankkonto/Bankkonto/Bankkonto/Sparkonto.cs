using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public class Sparkonto : Konto
{
    public KontoStatus Status { get; set; }
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

        Guthaben -= betrag;
    }
}
