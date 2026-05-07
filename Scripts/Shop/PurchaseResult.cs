using System.Collections.Generic;

namespace KodachiGames.Economy
{
	public abstract class PurchaseResult { }

	public sealed class PurchaseSuccess : PurchaseResult { }

	public sealed class PurchaseNotAffordable : PurchaseResult
	{
		public IReadOnlyList<IAcquisitionRequirement> FailedRequirements { get; }

		public PurchaseNotAffordable(IReadOnlyList<IAcquisitionRequirement> failedRequirements)
		{
			FailedRequirements = failedRequirements;
		}
	}

	public sealed class PurchaseOutOfStock : PurchaseResult { }

	public sealed class PurchaseAlreadyOwned : PurchaseResult { }
}