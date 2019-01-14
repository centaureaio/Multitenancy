namespace Centaurea.Multitenancy
{
    public struct TenantId
    {
        private readonly string _id;

        public TenantId(string id = DEFAULT)
        {
            this._id = id;
        }


        public const string DEFAULT = "default";
    }
}