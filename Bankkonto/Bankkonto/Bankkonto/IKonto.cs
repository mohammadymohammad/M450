using System;
using System.Collections.Generic;
using System.Text;

namespace Bankkonto;

public interface IKonto
{
    string KontoNummer { get; }
    decimal Guthaben { get; }

    void Einzahlen(decimal betrag);
    void Beziehen(decimal betrag);
    void Transferieren(IKonto konto, decimal betrag);
    void Zinsgutschreibung(int anzahlTage);
    void Kontoabschliessen();
}
