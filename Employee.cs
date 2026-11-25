namespace Internship_3_OOP
{
    internal class Employee : IAuditable
    {
        public static readonly List<Employee> All = [];
        public Guid Guid { get; } = Guid.NewGuid();
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public DateTime ModificationTime { get; private set; } = DateTime.Now;

        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required int BirthYear { get; init; }
        public required Gender Gender { get; init; }
        public required EmployeePosition Position { get; init; }

        public Employee()
        {
            All.Add(this);
        }

        public override string ToString() =>
            $"{Position.GetDisplayName()}: {Name} {Surname} ({BirthYear}.)";
    }
}
