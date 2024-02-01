using System;
using Amg.Authentication.DomainModel.SeedWorks.Exceptions;

namespace Amg.Authentication.DomainModel.SeedWorks.Base
{
    /// <inheritdoc />
    /// <summary>
    /// کلاس پایه ی إنتیتی ها
    /// Many objects are not fundamentally defined by their attributes, but rather by a thread of continuity and identity.
    /// </summary>
    /// <typeparam name="TId">نوع کلاس شناسه ی إنتیتی</typeparam>
    public abstract class EntityBase<TId> : IEquatable<EntityBase<TId>>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// ایجاد إنتیتی
        /// </summary>
        /// <param name="id">شناسه ی إنتیتی</param>
        protected EntityBase(TId id)
        {
            this.Id = id;
        }

        /// <summary>
        /// شناسه موجودیت
        /// </summary>
        public virtual TId Id { get; private set; }

        #region IEquatable and Override Equals operators

        /// <summary>
        /// بررسی تساوی دو مقدار
        /// </summary>
        /// <param name="entity1">مقدار اول</param>
        /// <param name="entity2">مقدار دوم</param>
        /// <returns>نتیجه صحیح یا غلط بودن بررسی تساوی دو مقدار</returns>
        public static bool operator ==(EntityBase<TId> entity1, EntityBase<TId> entity2)
        {
            if ((object)entity1 == null && (object)entity2 == null)
            {
                return true;
            }

            if ((object)entity1 == null || (object)entity2 == null)
            {
                return false;
            }

            return entity1.Id.ToString() == entity2.Id.ToString();
        }

        /// <summary>
        /// بررسی عدم تساوی دو مقدار
        /// </summary>
        /// <param name="entity1">مقدار اول</param>
        /// <param name="entity2">مقدار دوم</param>
        /// <returns>نتیجه صحیح یا غلط بودن بررسی عدم تساوی دو مقدار</returns>
        public static bool operator !=(EntityBase<TId> entity1, EntityBase<TId> entity2)
        {
            return !(entity1 == entity2);
        }

        /// <inheritdoc />
        public bool Equals(EntityBase<TId> other)
        {
            return this == other;
        }

        /// <inheritdoc />
        public override bool Equals(object entity)
        {
            return entity is EntityBase<TId> && this.Equals((EntityBase<TId>)entity);
        }

        /// <inheritdoc />
        //// ReSharper disable NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            return HashCode.Start.WithHash(this.Id);
            ////return this.Id == null ? 0 : this.Id.GetHashCode();
        }

        #endregion

        /// <summary>
        /// مقداردهی شناسه ی إنتیتی
        /// -- البته فقط در صورت آنکه تاکنون مقداردهی نشده باشد
        /// </summary>
        /// <param name="id">شناسه ی إنتیتی</param>
        public virtual void SetId(TId id)
        {
            if (this.Id.Equals(default(TId)))
            {
                this.Id = id;
            }
        }

        /// <summary>
        /// بررسی صحت موجودیت
        /// </summary>
        protected virtual void Validate()
        {
            if (this.Id.Equals(default(TId)))
            {
                throw new DomainException("Invalid Entity.");
            }
        }
    }
}