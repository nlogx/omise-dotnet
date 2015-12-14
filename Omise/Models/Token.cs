using System;
using System.Threading.Tasks;
using Omise.Models;
using Omise.Resources;
using Omise;

namespace Omise.Models {
    public partial class Token {
        ChargeResource charges;

        protected ChargeResource Charges {
            get { return charges ?? (charges = new ChargeResource(Requester)); }
        }

        public async Task<Charge> Charge(long amount, string currency) {
            return await Charge(new CreateChargeRequest
                {
                    Amount = amount,
                    Currency = currency,
                    Card = Id
                });
        }

        public async Task<Charge> Charge(CreateChargeRequest request) {
            request.Card = Id;
            return await Charges.Create(request);
        }

        protected ChargeResource GetChargeResource() {
            return new ChargeResource(Requester);
        }
    }
}

