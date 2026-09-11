using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public class Privatkonto : Konto
{
    public decimal MaximalerUeberziehungsbetrag { get; }
    public KontoStatus Status { get; set; }

    public Privatkonto(decimal maximalerUeberziehungsbetrag)
    {
        MaximalerUeberziehungsbetrag = maximalerUeberziehungsbetrag;
    }

    public override void Beziehen(decimal betrag)
    {
        if (betrag <= 0)
        {
            throw new ArgumentException(
                "Der Betrag muss grösser als 0 sein.");
        }

        if (Guthaben - betrag < -MaximalerUeberziehungsbetrag)
        {
            throw new InvalidOperationException(
                "Der maximale Überziehungsbetrag wurde überschritten.");
        }

        Guthaben -= betrag;
    }
}
