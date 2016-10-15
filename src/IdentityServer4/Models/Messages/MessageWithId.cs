using IdentityModel;

namespace IdentityServer4.Models.Messages
{
    public class MessageWithId<TModel> : Message<TModel>
    {
        public MessageWithId(TModel data) : base(data)
        {
            Id = CryptoRandom.CreateUniqueId();
        }

        public string Id { get; set; }
    }
}