namespace Internship_3_OOP
{
    internal sealed class Reservation : IAuditable
    {
        internal static readonly List<Reservation> All = [];
        public Guid Guid { get; init; } = Guid.NewGuid();
        public Guid GuestGuid { get; init; }
        public Guid RoomGuid { get; init; }
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public DateTime ModificationTime { get; set; } = DateTime.Now;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;

        public Reservation(Guest guest, Room room, DateTime checkIn, DateTime checkOut, bool disallowPast = true)
        {
            if (disallowPast && (checkIn < DateTime.Today || checkOut < DateTime.Today))
                throw new ArgumentException("Nije moguće retroaktivno rezervirati.");

            if (checkOut < checkIn)
                throw new ArgumentException("Odjava mora biti poslije prijave.");

            if (room.IsOccupied(checkIn, checkOut, All))
                throw new Exception("Soba je već zauzeta u tom rasponu.");

            if (guest.HasReservation(checkIn, checkOut, All))
                throw new Exception("Gost već ima rezervaciju u tom rasponu.");

            RoomGuid = room.Guid;
            GuestGuid = guest.Guid;
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            TotalPrice = (CheckOutDate - CheckInDate).Days * room.NightPrice;

            if (checkOut < DateTime.Today)
                Status = ReservationStatus.Finished;

            All.Add(this);
        }

        public override string ToString()
        {
            var room = Room.All.FirstOrDefault(r => r.Guid == RoomGuid);
            ArgumentNullException.ThrowIfNull(room);

            return $"[{Guid.ToString()[..Program.GUID_CHARACTER_COUNT]}] Soba {room.RoomNumber} ({room.FloorNumber}. kat) " +
                $"| {CheckInDate:d} - {CheckOutDate:d} | " +
                $"{(CheckOutDate - CheckInDate).Days} noći | {TotalPrice} € | {Status.GetDisplayName()}";
        }
    }
}