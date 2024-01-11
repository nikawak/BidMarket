using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Services
{
    public class PaymentService
    {
        private PaymentSettings _settings;
        private IBraintreeGateway _gateway;

        public PaymentService(IOptions<PaymentSettings> options)
        {
            _settings = options.Value;
            _gateway = new BraintreeGateway(_settings.Environment, _settings.MerchantId, _settings.PublicKey, _settings.PrivateKey);
        }


        public async Task<string> GenerateToken()
        {
            return await _gateway.ClientToken.GenerateAsync();
        }
        public TransactionRequest CreateTransaction(decimal amount, string nonceMethod)
        {
            var request = new TransactionRequest()
            {
                OrderId = Guid.NewGuid().ToString(),
                Amount = amount,
                PaymentMethodNonce = nonceMethod,
            };
            return request;
        }

        public Result<Transaction> StartTransaction(TransactionRequest request)
        {
            var tran = _gateway.Transaction.Sale(request);
            return tran;
        }
    }
}
