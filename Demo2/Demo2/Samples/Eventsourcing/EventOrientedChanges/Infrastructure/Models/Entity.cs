namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models
{
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Id defines entity uniqueness and is used for Equality
        /// comparisons and hash code generation.
        /// </summary>
        public abstract Guid Id { get; internal set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return EqualityComparer<Guid>.Default.Equals(Id, ((Entity)obj).Id);
        }

        public static bool operator ==(Entity lhs, Entity rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Entity lhs, Entity rhs) => !(lhs == rhs);

        public override int GetHashCode()
        {
            if (Id.Equals(default))
            {
                return base.GetHashCode();
            }

            return GetType().GetHashCode() ^ Id.GetHashCode();
        }
    }

    public interface IEntity
    {
        Guid Id { get; }
    }
}
