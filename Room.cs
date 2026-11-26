namespace Internship_3_OOP
{
    internal class Room : IAuditable
    {
        internal static readonly List<Room> All = [];
        public Guid Guid { get; init; } = Guid.NewGuid();
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public DateTime ModificationTime { get; set; } = DateTime.Now;
        public int RoomNumber { get; init; }
        public int FloorNumber { get; init; }
        public RoomCategory Category { get; init; }
        public int Capacity { get; init; }
        public decimal NightPrice { get; init; }

        public Room(int number, int floor, RoomCategory category, int capacity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number, "Broj sobe");
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity, "Kapacitet");
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, "Cijena noćenja");

            if (All.Any(r => r.RoomNumber == number))
                throw new ArgumentException("Broj sobe mora biti jedinstven.");

            RoomNumber = number;
            FloorNumber = floor;
            Category = category;
            Capacity = capacity;
            NightPrice = price;

            All.Add(this);
        }

        public bool IsOccupied(DateTime start, DateTime end, IEnumerable<Reservation> compList) =>
            compList.Any(r =>
                r.RoomGuid == Guid &&
                r.Status != ReservationStatus.Canceled &&
                start < r.CheckOutDate &&
                end > r.CheckInDate);

        public override string ToString()
        {
            return $"[{Guid.ToString()[..Program.GUID_CHARACTER_COUNT]}] Soba {RoomNumber} " +
                $"| {FloorNumber}. kat | {Category.GetDisplayName()} | " +
                $"{(Capacity % 10 == 1 ? $"{Capacity} osoba" : $"{Capacity} osobe")} | " +
                $"{NightPrice} €/noć | broj rezervacija: " +
                $"{Reservation.All.Where(r => r.RoomGuid == Guid).Count()}";
        }
    }
}
