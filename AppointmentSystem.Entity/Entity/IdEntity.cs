namespace AppointmentSystem.Entity.Entity
{
    public abstract class IdEntity<T> : IEntity
    {
        public T Id { get; set; }

    }
}
