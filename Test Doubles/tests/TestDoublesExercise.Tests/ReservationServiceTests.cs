using Moq;

namespace TestDoublesExercise.Tests;

public sealed class ReservationServiceTests
{
    [Fact]
    public void Reserve_WithEnoughSeats_ReturnsTrue()
    {
        // Arrange
        var repository = new Mock<IReservationRepository>();
        repository
            .Setup(repo => repo.TrySave(It.Is<Reservation>(reservation =>
                reservation.CustomerEmail == "person@example.com" &&
                reservation.EventCode == "EVENT-1" &&
                reservation.SeatCount == 2)))
            .Returns(true);
        var sender = new RecordingConfirmationSpy();
        var service = new ReservationService(
            new FixedSeatAvailabilityStub("EVENT-1", availableSeats: 3, calls: null),
            repository.Object,
            sender);

        // Act
        var result = service.Reserve("person@example.com", "EVENT-1", 2);

        // Assert
        Assert.True(result);
        repository.Verify(repo => repo.TrySave(It.Is<Reservation>(reservation =>
            reservation.CustomerEmail == "person@example.com" &&
            reservation.EventCode == "EVENT-1" &&
            reservation.SeatCount == 2)), Times.Once());
        var sentReservation = Assert.Single(sender.SentReservations);
        Assert.Equal("person@example.com", sentReservation.CustomerEmail);
        Assert.Equal("EVENT-1", sentReservation.EventCode);
        Assert.Equal(2, sentReservation.SeatCount);
    }

    [Fact]
    public void Reserve_WithTooFewSeats_ReturnsFalse()
    {
        // Arrange
        var calls = new DependencyCallTracker();
        var sender = new RecordingConfirmationSpy(calls);
        var service = new ReservationService(
            new FixedSeatAvailabilityStub("EVENT-1", availableSeats: 1, calls),
            new SuccessfulReservationRepositoryStub(calls),
            sender);

        // Act
        var result = service.Reserve("person@example.com", "EVENT-1", 2);

        // Assert
        Assert.False(result);
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void Reserve_WithExactlyEnoughSeats_Succeeds()
    {
        // Arrange
        var service = CreateService(availableSeats: 2);

        // Act
        var result = service.Reserve("person@example.com", "EVENT-1", 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Reserve_WithZeroSeats_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        var calls = new DependencyCallTracker();
        var service = CreateService(availableSeats: 3, calls);

        // Act:     0 Plätze reservieren.
        var result = Record.Exception(() => service.Reserve("person@example.com", "EVENT-1", 0));

        // Assert:  ArgumentOutOfRangeException; keine Abhängigkeit wurde aufgerufen.
        Assert.IsType<ArgumentOutOfRangeException>(result);
        Assert.Equal(0, calls.AvailabilityCalls);
        Assert.Equal(0, calls.RepositoryCalls);
        Assert.Equal(0, calls.SenderCalls);
    }

    [Fact]
    public void Reserve_WithNegativeSeats_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        var calls = new DependencyCallTracker();
        var service = CreateService(availableSeats: 3, calls);

        // Act:     -1 Plätze reservieren.
        var result = Record.Exception(() => service.Reserve("person@example.com", "EVENT-1", -1));

        // Assert:  ArgumentOutOfRangeException; keine Abhängigkeit wurde aufgerufen.
        Assert.IsType<ArgumentOutOfRangeException>(result);
        Assert.Equal(0, calls.AvailabilityCalls);
        Assert.Equal(0, calls.RepositoryCalls);
        Assert.Equal(0, calls.SenderCalls);
    }

    [Fact]
    public void Reserve_WithEmptyEventCode_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        var calls = new DependencyCallTracker();
        var service = CreateService(availableSeats: 3, calls);
        // Act:     Mit leerem Veranstaltungscode reservieren.
        var result = Record.Exception(() => service.Reserve("person@example.com", "", 2));
        // Assert:  ArgumentException; keine Abhängigkeit wurde aufgerufen.
        Assert.IsType<ArgumentException>(result);
        Assert.Equal(0, calls.AvailabilityCalls);
        Assert.Equal(0, calls.RepositoryCalls);
        Assert.Equal(0, calls.SenderCalls);
    }

    [Fact]
    public void Reserve_WithWhitespaceEmail_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Handgeschriebener Verfügbarkeits-Stub und Sender-Spy, Repository-Mock.
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var sender = new RecordingConfirmationSpy(calls);
        var service = new ReservationService(
            new FixedSeatAvailabilityStub("EVENT-1", availableSeats: 3, calls),
            repository.Object,
            sender);

        // Act:     Mit einer E-Mail-Adresse aus Leerzeichen reservieren.
        var result = Record.Exception(() => service.Reserve("   ", "EVENT-1", 2));

        // Assert:  ArgumentException; keine Abhängigkeit wurde aufgerufen.
        Assert.IsType<ArgumentException>(result);
        Assert.Equal(0, calls.AvailabilityCalls);
        repository.Verify(repo => repo.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void Reserve_WhenSavingFails_DoesNotSendConfirmation()
    {
        // Arrange: Stub mit genügend Plätzen, Repository-Mock liefert false.
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        repository
            .Setup(repo => repo.TrySave(It.Is<Reservation>(reservation =>
                reservation.CustomerEmail == "person@example.com" &&
                reservation.EventCode == "EVENT-1" &&
                reservation.SeatCount == 2)))
            .Returns(false);
        var sender = new RecordingConfirmationSpy(calls);
        var service = new ReservationService(
            new FixedSeatAvailabilityStub("EVENT-1", availableSeats: 3, calls),
            repository.Object,
            sender);

        // Act:     Reservierung versuchen.
        var result = service.Reserve("person@example.com", "EVENT-1", 2);

        // Assert:  Ergebnis ist false, Speichern fand statt, Bestätigung nicht.
        Assert.False(result);
        repository.Verify(repo => repo.TrySave(It.Is<Reservation>(reservation =>
            reservation.CustomerEmail == "person@example.com" &&
            reservation.EventCode == "EVENT-1" &&
            reservation.SeatCount == 2)), Times.Once());
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void Reserve_WithTooFewSeats_DoesNotCallRepository()
    {
        // Arrange: Stub mit zu wenigen Plätzen und Repository-Mock.
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var service = new ReservationService(
            new FixedSeatAvailabilityStub("EVENT-1", availableSeats: 1, calls),
            repository.Object,
            new NoOpConfirmationSenderStub(calls));

        // Act:     Reservierung versuchen.
        var result = service.Reserve("person@example.com", "EVENT-1", 2);

        // Assert:  TrySave wurde nicht aufgerufen.
        Assert.False(result);
        repository.Verify(repo => repo.TrySave(It.IsAny<Reservation>()), Times.Never());
    }

    [Fact]
    public void ReserveBatch_WhenCombinedSeatsExceedCapacity_DoesNotSaveOrSend()
    {
        // Arrange: Beide Reservierungen passen einzeln, zusammen überschreiten sie die Kapazität.
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var sender = new RecordingConfirmationSpy(calls);
        var service = CreateBatchService(
            new Dictionary<string, int> { ["EVENT-1"] = 3 },
            repository,
            sender,
            calls);
        var requests = new[]
        {
            new Reservation("person@example.com", "EVENT-1", 2),
            new Reservation("person2@example.com", "EVENT-1", 2)
        };

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.False(result);
        Assert.Equal(new[] { "EVENT-1" }, calls.AvailabilityEventCodes);
        repository.Verify(repo => repo.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>()), Times.Never());
        repository.Verify(repo => repo.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void ReserveBatch_WithEnoughSeats_SavesAllOnceAndSendsInRequestOrder()
    {
        // Arrange
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var requests = new[]
        {
            new Reservation("first@example.com", "EVENT-1", 2),
            new Reservation("second@example.com", "EVENT-1", 1),
            new Reservation("third@example.com", "EVENT-2", 2)
        };
        repository
            .Setup(repo => repo.TrySaveBatch(It.Is<IReadOnlyList<Reservation>>(saved =>
                saved.SequenceEqual(requests))))
            .Returns(true);
        var sender = new RecordingConfirmationSpy(calls);
        var service = CreateBatchService(
            new Dictionary<string, int>
            {
                ["EVENT-1"] = 3,
                ["EVENT-2"] = 2
            },
            repository,
            sender,
            calls);

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.True(result);
        Assert.Equal(1, calls.AvailabilityEventCodes.Count(code => code == "EVENT-1"));
        Assert.Equal(1, calls.AvailabilityEventCodes.Count(code => code == "EVENT-2"));
        repository.Verify(repo => repo.TrySaveBatch(It.Is<IReadOnlyList<Reservation>>(saved =>
            saved.SequenceEqual(requests))), Times.Once());
        repository.Verify(repo => repo.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Equal(requests, sender.SentReservations);
    }

    [Fact]
    public void ReserveBatch_WhenBatchSaveFails_DoesNotSendConfirmations()
    {
        // Arrange
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var requests = new[]
        {
            new Reservation("person@example.com", "EVENT-1", 2)
        };
        repository
            .Setup(repo => repo.TrySaveBatch(It.Is<IReadOnlyList<Reservation>>(saved =>
                saved.SequenceEqual(requests))))
            .Returns(false);
        var sender = new RecordingConfirmationSpy(calls);
        var service = CreateBatchService(
            new Dictionary<string, int> { ["EVENT-1"] = 2 },
            repository,
            sender,
            calls);

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.False(result);
        repository.Verify(repo => repo.TrySaveBatch(It.Is<IReadOnlyList<Reservation>>(saved =>
            saved.SequenceEqual(requests))), Times.Once());
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void ReserveBatch_WithNullRequests_ThrowsBeforeCallingDependencies()
    {
        // Arrange
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var sender = new RecordingConfirmationSpy(calls);
        var service = CreateBatchService(
            new Dictionary<string, int>(),
            repository,
            sender,
            calls);

        // Act
        var exception = Record.Exception(() => service.ReserveBatch(null!));

        // Assert
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Empty(calls.AvailabilityEventCodes);
        repository.Verify(repo => repo.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void ReserveBatch_WithEmptyRequests_ThrowsBeforeCallingDependencies()
    {
        // Arrange
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var sender = new RecordingConfirmationSpy(calls);
        var service = CreateBatchService(
            new Dictionary<string, int>(),
            repository,
            sender,
            calls);

        // Act
        var exception = Record.Exception(() => service.ReserveBatch(Array.Empty<Reservation>()));

        // Assert
        Assert.IsType<ArgumentException>(exception);
        Assert.Empty(calls.AvailabilityEventCodes);
        repository.Verify(repo => repo.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }

    [Theory]
    [InlineData("", "EVENT-1", 1)]
    [InlineData("   ", "EVENT-1", 1)]
    [InlineData("person@example.com", "", 1)]
    [InlineData("person@example.com", "   ", 1)]
    [InlineData("person@example.com", "EVENT-1", 0)]
    [InlineData("person@example.com", "EVENT-1", -1)]
    public void ReserveBatch_WithInvalidRequest_ThrowsBeforeCallingDependencies(
        string customerEmail,
        string eventCode,
        int seatCount)
    {
        // Arrange
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var sender = new RecordingConfirmationSpy(calls);
        var service = CreateBatchService(
            new Dictionary<string, int>(),
            repository,
            sender,
            calls);
        var request = new Reservation(customerEmail, eventCode, seatCount);

        // Act
        var exception = Record.Exception(() => service.ReserveBatch(new[] { request }));

        // Assert
        if (seatCount <= 0)
        {
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }
        else
        {
            Assert.IsType<ArgumentException>(exception);
        }

        Assert.Empty(calls.AvailabilityEventCodes);
        repository.Verify(repo => repo.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }

    [Fact]
    public void ReserveBatch_WithInvalidLaterRequest_ValidatesBeforeCallingDependencies()
    {
        // Arrange
        var calls = new DependencyCallTracker();
        var repository = new Mock<IReservationRepository>();
        var sender = new RecordingConfirmationSpy(calls);
        var service = CreateBatchService(
            new Dictionary<string, int> { ["EVENT-1"] = 2 },
            repository,
            sender,
            calls);
        var requests = new[]
        {
            new Reservation("person@example.com", "EVENT-1", 1),
            new Reservation("", "EVENT-1", 1)
        };

        // Act
        var exception = Record.Exception(() => service.ReserveBatch(requests));

        // Assert
        Assert.IsType<ArgumentException>(exception);
        Assert.Empty(calls.AvailabilityEventCodes);
        repository.Verify(repo => repo.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>()), Times.Never());
        Assert.Empty(sender.SentReservations);
    }

    private static ReservationService CreateService(int availableSeats, DependencyCallTracker? calls = null) => new(
        new FixedSeatAvailabilityStub("EVENT-1", availableSeats, calls),
        new SuccessfulReservationRepositoryStub(calls),
        new NoOpConfirmationSenderStub(calls));

    private static ReservationService CreateBatchService(
        IReadOnlyDictionary<string, int> availableSeats,
        Mock<IReservationRepository> repository,
        RecordingConfirmationSpy sender,
        DependencyCallTracker calls) => new(
            new FixedSeatAvailabilityStub(availableSeats, calls),
            repository.Object,
            sender);

    private sealed class DependencyCallTracker
    {
        private readonly List<string> _availabilityEventCodes = [];

        public int AvailabilityCalls { get; private set; }
        public int RepositoryCalls { get; private set; }
        public int SenderCalls { get; private set; }
        public IReadOnlyList<string> AvailabilityEventCodes => _availabilityEventCodes;

        public void RecordAvailabilityCall(string eventCode)
        {
            AvailabilityCalls++;
            _availabilityEventCodes.Add(eventCode);
        }

        public void RecordRepositoryCall() => RepositoryCalls++;
        public void RecordSenderCall() => SenderCalls++;
    }

    private sealed class FixedSeatAvailabilityStub : ISeatAvailability
    {
        private readonly IReadOnlyDictionary<string, int> _availableSeats;
        private readonly DependencyCallTracker? _calls;

        public FixedSeatAvailabilityStub(
            string eventCode,
            int availableSeats,
            DependencyCallTracker? calls)
            : this(new Dictionary<string, int> { [eventCode] = availableSeats }, calls)
        {
        }

        public FixedSeatAvailabilityStub(
            IReadOnlyDictionary<string, int> availableSeats,
            DependencyCallTracker? calls)
        {
            _availableSeats = availableSeats;
            _calls = calls;
        }

        public int GetAvailableSeats(string requestedEventCode)
        {
            _calls?.RecordAvailabilityCall(requestedEventCode);
            return _availableSeats.TryGetValue(requestedEventCode, out var availableSeats)
                ? availableSeats
                : throw new InvalidOperationException($"Unerwarteter Veranstaltungscode: {requestedEventCode}");
        }
    }

    private sealed class SuccessfulReservationRepositoryStub(DependencyCallTracker? calls) : IReservationRepository
    {
        public bool TrySave(Reservation reservation)
        {
            calls?.RecordRepositoryCall();
            return true;
        }

        public bool TrySaveBatch(IReadOnlyList<Reservation> reservations) => true;
    }

    private sealed class NoOpConfirmationSenderStub(DependencyCallTracker? calls) : IConfirmationSender
    {
        public void Send(Reservation reservation)
        {
            calls?.RecordSenderCall();
        }
    }

    private sealed class RecordingConfirmationSpy(DependencyCallTracker? calls = null) : IConfirmationSender
    {
        private readonly List<Reservation> _sentReservations = [];

        public IReadOnlyList<Reservation> SentReservations => _sentReservations;

        public void Send(Reservation reservation)
        {
            calls?.RecordSenderCall();
            _sentReservations.Add(reservation);
        }
    }
}
