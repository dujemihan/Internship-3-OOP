namespace Internship_3_OOP
{
    internal sealed class Reservation : IAuditable
    {
        private readonly List<Reservation> _reservations = [];
        public Guid Guid { get; init; } = Guid.NewGuid();
        private Guid GuestGuid { get; init; }
        private Guid RoomGuid { get; init; }
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public DateTime ModificationTime { get; set; } = DateTime.Now;
        public DateTime CheckInDate { get; private set; }
        public DateTime CheckOutDate { get; private set; }
        public int NightCount { get; private set; }
        public decimal TotalPrice { get; private set; }
        public ReservationStatus Status { get; private set; } = ReservationStatus.Confirmed;

        public Reservation(Guest guest, Room room, DateTime checkIn, DateTime checkOut, int nights)
        {
            if (checkIn < DateTime.Today || checkOut < DateTime.Today)
                throw new ArgumentException("Nije moguće retroaktivno rezervirati.");

            RoomGuid = room.Guid;
            GuestGuid = guest.Guid;
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            NightCount = nights;
            TotalPrice = NightCount * room.NightPrice;
        }
    }
}