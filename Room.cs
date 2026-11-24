using System;
using System.Collections.Generic;
using System.Text;

namespace Internship_3_OOP
{
    internal class Room : IAuditable
    {
        internal static readonly List<Room> _rooms = [];
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

            if (_rooms.FirstOrDefault(r => r.RoomNumber == number) != default)
                throw new ArgumentException("Broj sobe mora biti jedinstven.");

            RoomNumber = number;
            FloorNumber = floor;
            Category = category;
            Capacity = capacity;
            NightPrice = price;

            _rooms.Add(this);
        }

        public bool IsOccupied(DateTime start, DateTime end)
        {
            foreach (var reservation in Reservation._reservations.Where(r => r.RoomGuid == Guid))
            {
                if (reservation.CheckOutDate > start || reservation.CheckInDate > end)
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"[{Guid.ToString()[..Program.GUID_CHARACTER_COUNT]}] Soba {RoomNumber} " +
                $"| {FloorNumber}. kat | {Category} | " +
                $"{(Capacity % 10 == 1 ? $"{Capacity} osoba" : $"{Capacity} osobe")} | " +
                $"{NightPrice} €/noć | broj rezervacija: TODO";
        }
    }
}
