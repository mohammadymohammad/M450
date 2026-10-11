# Detailierte Beschreibungen

## Aufgabe 1

Auch ohne produktive Implementierung kann `ReservationService` getestet werden, indem wir im Testprojekt einen handgeschriebenen Stub für `ISeatAvailability` verwenden. Die Schnittstelle sagt bereits, dass `GetAvailableSeats` einen Veranstaltungscode erhält und eine ganze Zahl zurückgibt. Wir nehmen für den Test an, dass dieser Rückgabewert die Anzahl der aktuell verfügbaren Plätze für die angegebene Veranstaltung darstellt und nicht negativ ist.

Der Stub liefert für einen festgelegten Veranstaltungscode eine vorher konfigurierte Zahl zurück, zum Beispiel `3`. So lässt sich prüfen, dass eine Reservierung von 2 Plätzen bei 3 verfügbaren Plätzen gelingt, bei nur 1 Platz abgelehnt wird und bei genau 2 Plätzen gelingt. Repository und Bestätigungsversand ersetzen wir ebenfalls durch einfache Test-Doubles, damit der Test keine Datenbank oder echten Nachrichten benötigt.

Damit testen wir das Verhalten von `ReservationService` unter kontrollierten Antworten der Abhängigkeit. Ob eine spätere produktive Implementierung die Verfügbarkeit tatsächlich korrekt ermittelt, muss separat getestet werden.

## Aufgabe 3

Ein Stub liefert dem Test vorbereitete Rückgabewerte, während ein Spy Aufrufe und übergebene Argumente aufzeichnet, damit der Test sie anschliessend prüfen kann.

## Aufgabe 5

Für die Verfügbarkeit verwende ich einen handgeschriebenen Stub, weil ich für diesen Test nur einen kontrollierten Rückgabewert brauche. Für das Repository verwende ich Moq, damit ich mit `Verify(..., Times.Never())` gezielt prüfen kann, dass bei ungültiger E-Mail-Adresse nicht gespeichert wird. Für den Bestätigungsversand verwende ich einen handgeschriebenen Spy, dessen aufgezeichnete Reservierungen leer bleiben müssen.

## Gruppenreservierung: Fehler beim Bestätigungsversand

Wenn `Send` nach dem erfolgreichen Batch-Speichern eine Exception wirft, sind die Reservierungen bereits gespeichert, aber eine oder mehrere Bestätigungen koennen fehlen. Ein erneuter Aufruf des gesamten Batch-Vorgangs koennte die Buchungen doppelt anlegen. Fachlich kann die Reservierung deshalb bestehen bleiben, waehrend die fehlende Bestaetigung als ausstehend markiert und spaeter wiederholt versendet wird, ohne die Buchung erneut anzulegen.
