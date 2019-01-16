namespace Centaurea.Multitenancy
{
    public class TenantId
    {
        public override bool Equals(object obj) => obj is TenantId tenantId && _id.Equals(tenantId._id);
        
        public override int GetHashCode()
        {
            return (_id != null ? _id.GetHashCode() : 0);
        }
        
        private readonly string _id = DEFAULT;

        public TenantId(string id = DEFAULT)
        {
            _id = id;
        }
        
        public const string DEFAULT = "default";
        public static readonly TenantId DEFAULT_ID = new TenantId();
    }
}