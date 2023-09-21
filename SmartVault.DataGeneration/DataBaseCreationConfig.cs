namespace SmartVault.DataGeneration
{
    public record ConnectionStrings(string DefaultConnection);
    public record DataBaseCreationConfig(string DatabaseFileName, ConnectionStrings ConnectionStrings)
    {
        public string FormatedConnectionString()
        {
            return string.Format(this?.ConnectionStrings?.DefaultConnection ?? "", this?.DatabaseFileName);
        }
    }


}