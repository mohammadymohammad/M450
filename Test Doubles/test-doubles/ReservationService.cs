namespace TestDoublesExercise;

public sealed record Reservation(string CustomerEmail, string EventCode, int SeatCount);

public interface ISeatAvailability
{
    int GetAvailableSeats(string eventCode);
}

public interface IReservationRepository
{
    bool TrySave(Reservation reservation);
    bool TrySaveBatch(IReadOnlyList<Reservation> reservations);
}

public interface IConfirmationSender
{
    void Send(Reservation reservation);
}

public sealed class ReservationService
{
    private readonly ISeatAvailability _availability;
    private readonly IReservationRepository _repository;
    private readonly IConfirmationSender _sender;

    public ReservationService(
        ISeatAvailability availability,
        IReservationRepository repository,
        IConfirmationSender sender)
    {
        _availability = availability ?? throw new ArgumentNullException(nameof(availability));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public bool Reserve(string customerEmail, string eventCode, int seatCount)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            throw new ArgumentException("Eine E-Mail-Adresse ist erforderlich.", nameof(customerEmail));
        }

        if (string.IsNullOrWhiteSpace(eventCode))
        {
            throw new ArgumentException("Ein Veranstaltungscode ist erforderlich.", nameof(eventCode));
        }

        if (seatCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seatCount));
        }

        if (_availability.GetAvailableSeats(eventCode) < seatCount)
        {
            return false;
        }

        var reservation = new Reservation(customerEmail, eventCode, seatCount);
        if (!_repository.TrySave(reservation))
        {
            return false;
        }

        _sender.Send(reservation);
        return true;
    }

    public bool ReserveBatch(IReadOnlyList<Reservation> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (requests.Count == 0)
        {
            throw new ArgumentException("Mindestens eine Reservierung ist erforderlich.", nameof(requests));
        }

        var requestedSeatsByEvent = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var request in requests)
        {
            if (request is null)
            {
                throw new ArgumentException("Eine Reservierung darf nicht null sein.", nameof(requests));
            }

            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                throw new ArgumentException("Eine E-Mail-Adresse ist erforderlich.", nameof(requests));
            }

            if (string.IsNullOrWhiteSpace(request.EventCode))
            {
                throw new ArgumentException("Ein Veranstaltungscode ist erforderlich.", nameof(requests));
            }

            if (request.SeatCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requests), "Die Platzanzahl muss positiv sein.");
            }

            requestedSeatsByEvent.TryGetValue(request.EventCode, out var currentSeatCount);
            requestedSeatsByEvent[request.EventCode] = currentSeatCount + request.SeatCount;
        }

        var hasEnoughSeats = true;
        foreach (var (eventCode, requestedSeatCount) in requestedSeatsByEvent)
        {
            if (_availability.GetAvailableSeats(eventCode) < requestedSeatCount)
            {
                hasEnoughSeats = false;
            }
        }

        if (!hasEnoughSeats || !_repository.TrySaveBatch(requests))
        {
            return false;
        }

        foreach (var request in requests)
        {
            _sender.Send(request);
        }

        return true;
    }
}
