using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public interface IKonto
{
    string KontoNummer { get; }
    decimal Guthaben { get; }
    DateTime Eroeffnungsdatum { get; }
    DateTime Abschlussdatum { get; }
    DateTime Transaktionsdatum { get; }

    void Einzahlen(decimal betrag);
    void Beziehen(decimal betrag);
    void setzeBezugslimite(decimal neueBezugslimite, DateTime gueltigAb);
    void Transferieren(IKonto konto, decimal betrag);
    void Zinsgutschreibung(int anzahlTage);
    void Kontoabschliessen();
}
