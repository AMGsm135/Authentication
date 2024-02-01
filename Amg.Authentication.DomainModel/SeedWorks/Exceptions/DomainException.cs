using System;

namespace Amg.Authentication.DomainModel.SeedWorks.Exceptions
{
	public class DomainException : Exception
	{
		/// <inheritdoc />
		public DomainException()
		{
		}

		/// <inheritdoc />
		public DomainException(string message) : base(message)
		{
		}
    }
}