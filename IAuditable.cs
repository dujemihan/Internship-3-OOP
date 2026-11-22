namespace Internship_3_OOP
{
    internal interface IAuditable
    {
        public Guid Guid { get; }
        public DateTime CreationTime { get; }
        public DateTime ModificationTime { get; }
    }
}
