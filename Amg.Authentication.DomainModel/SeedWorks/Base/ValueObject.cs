using System;
using System.Linq;

namespace Amg.Authentication.DomainModel.SeedWorks.Base
{
	/// <summary>
	/// کلاس پایه ی شیء باارزش یا ولیو-آبجکت
	/// Many objects have no conceptual identity. These objects describe some characteristic of a thing.
	/// تمام خصوصیت های یک شیء باارزش باید فقط خواندنی هستند
	/// </summary>
	/// <typeparam name="TValueObject">The type of this value object</typeparam>
	public abstract class ValueObject<TValueObject> : IEquatable<TValueObject>
		where TValueObject : ValueObject<TValueObject>
	{
		#region IEquatable and Override Equals operators

		/// <summary>
		/// The ==.
		/// </summary>
		/// <param name="left">
		/// The left.
		/// </param>
		/// <param name="right">
		/// The right.
		/// </param>
		/// <returns>
		/// The bool
		/// </returns>
		public static bool operator ==(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
		{
			return left?.Equals(right) ?? Equals(right, null);
		}

		/// <summary>
		/// The !=.
		/// </summary>
		/// <param name="left">
		/// The left.
		/// </param>
		/// <param name="right">
		/// The right.
		/// </param>
		/// <returns>
		/// The bool
		/// </returns>
		public static bool operator !=(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
		{
			return !(left == right);
		}

		/// <inheritdoc />
		public virtual bool Equals(TValueObject other)
		{
			if ((object)other == null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			// compare all public properties
			var publicProperties = this.GetType().GetProperties();

			if (publicProperties.Any())
			{
				return publicProperties.All(p =>
				{
					var left = p.GetValue(this, null);
					var right = p.GetValue(other, null);

					if (left == null && right == null)
					{
						return true;
					}

					if (left == null || right == null)
					{
						return false;
					}

					if (left is TValueObject)
					{
						////check not self-references...
						return ReferenceEquals(left, right);
					}

					return left.Equals(right);
				});
			}

			return true;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var item = obj as ValueObject<TValueObject>;

			return (object)item != null && this.Equals((TValueObject)item);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode.Start
				.WithHashProperties(this, true);

			////unchecked
			////{
			////    //// Overflow is fine, just wrap
			////    var hashCode = 31;
			////    var changeMultiplier = false;
			////    const int Index = 1;

			////    ////compare all public properties
			////    var publicProperties = this.GetType().GetProperties();

			////    if (!publicProperties.Any()) return hashCode;

			////    foreach (var value in publicProperties.Select(item => item.GetValue(this, null)))
			////    {
			////        if (value != null)
			////        {
			////            hashCode = (hashCode * (changeMultiplier ? 59 : 114)) + value.GetHashCode();
			////            changeMultiplier = !changeMultiplier;
			////        }
			////        else
			////        {
			////            hashCode = hashCode ^ (Index * 13); ////only for support {"a",null,null,"a"} <> {null,"a","a",null}
			////        }
			////    }

			////    return hashCode;
			////}
		}

		#endregion

		/// <summary>
		/// بررسی صحت اطلاعات ولیو-آبجکت
		/// </summary>
		protected abstract void Validate();
	}
}