using System;

namespace Amg.Authentication.DomainModel.SeedWorks.Base
{
    public abstract class AggregateRoot<T> : EntityBase<T>, IAggregateRoot where T : IEquatable<T>
    {
        /// <inheritdoc />
        protected AggregateRoot(T id) : base(id)
        {
        }

        /// <inheritdoc />
        public object GetId()
        {
            return this.Id;
        }
    }
}