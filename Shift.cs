using System.Text;

namespace Internship_3_OOP
{
    internal class Shift : IAuditable
    {
        public static readonly List<Shift> All = [];
        public Guid Guid { get; private init; } = Guid.NewGuid();
        public DateTime CreationTime { get; private init; } = DateTime.Now;
        public DateTime ModificationTime { get; private set; } = DateTime.Now;
        public List<Guid> EmployeeGuids = [];
        public required DateOnly ShiftDate;
        public required bool IsAfternoon;

        public Shift()
        {
            All.Add(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            builder = builder.AppendLine($"Smjena: {(IsAfternoon ? "Popodnevna" : "Jutarnja")} {ShiftDate}");
            foreach (var employeeGuid in EmployeeGuids)
            {
                var employee = Employee.All.FirstOrDefault(e => e.Guid == employeeGuid);
                ArgumentNullException.ThrowIfNull(employee);
                builder = builder.AppendLine($"\t{employee.ToString()}");
            }

            return builder.ToString();
        }
    }
}
