using ASC.Model;

namespace ASC.Web.Configuration
{
    public class MasterDataCache
    {
        public List<MasterDataKey> MasterDataKeys { get; set; }
            = new List<MasterDataKey>();

        public List<MasterDataValue> MasterDataValues { get; set; }
            = new List<MasterDataValue>();
    }
}