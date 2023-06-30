namespace SA.Common.Models
{
	public class Result
	{
		protected Error _Error;

		public Error Error => _Error;

		public bool HasError
		{
			get
			{
				if (_Error == null)
				{
					return false;
				}
				return true;
			}
		}

		public bool IsSucceeded => !HasError;

		public bool IsFailed => HasError;

		public Result()
		{
		}

		public Result(Error error)
		{
			_Error = error;
		}
	}
}
