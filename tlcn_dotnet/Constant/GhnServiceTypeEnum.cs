namespace tlcn_dotnet.Constant
{
    public enum GhnServiceTypeEnum
    {
        NHANH = 1,
        CHUAN = 2,
        TIETKIEM = 3,
    }

    public static class GhnServiceId
    {
        public static int GetServiceId(GhnServiceTypeEnum serviceType)
        {
            switch (serviceType)
            {
                case GhnServiceTypeEnum.NHANH:
                    return 53319;
                case GhnServiceTypeEnum.CHUAN:
                    return 53320;
                case GhnServiceTypeEnum.TIETKIEM:
                    return 53321;
                default:
                    return 53330;
            }
        }
    }
}
