namespace CSData
{
    public enum ConflictOptions
    {
        Merge,
        Replace
    }

    public class CsDataInjectOptions
    {
        public static readonly CsDataInjectOptions Default = new CsDataInjectOptions();

        //public ConflictOptions OnConflict { get; set; } = ConflictOptions.Replace;
        //public bool Notify { get; set; } = true;
    }
}
