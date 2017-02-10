namespace CSData
{
    public class CsDataFindOptions: CsDataInjectOptions
    {
        public static readonly new CsDataFindOptions Default = new CsDataFindOptions();
        public string Adapter { get; set; } = null;
        //public bool ByPassCache { get; set; } = false;
        //public bool CacheResponse { get; set; } = true;
    }
}
