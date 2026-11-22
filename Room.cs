using System;
using System.Collections.Generic;
using System.Text;

namespace Internship_3_OOP
{
    internal class Room : IAuditable
    {
        public Guid Guid { get; init; } = Guid.NewGuid();
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public DateTime ModificationTime { get; set; } = DateTime.Now;
        public int RoomNumber { get; init; }
        public int FloorNumber { get; init; }
        public RoomCategory Category { get; init; }
        public int Capacity { get; init; }
        public decimal NightPrice { get; init; }
    }
}
