using Omise.Models;

namespace Omise.Resources
{
    public class CustomerCardResource : BaseResource<Card>,
    IListable<Card>,
    IListRetrievable<Card>,
    IUpdatable<Card, UpdateCardRequest>,
    IDestroyable<Card>
    {
        public CustomerCardResource(IRequester requester, string customerId)
            : base(requester, Endpoint.Api, basePathFor(customerId))
        {
        }

        static string basePathFor(string customerId)
        {
            return $"/customers/{customerId}/cards";
        }
    }
}