namespace MDAI.Contracts
{
    public enum ApiCode : int
        {
            Success = 10000,
            Unauthorized = 10001,
            Exception = 10002,
            AlreadyExists = 10003,
            NotFoundTenantOrCustomer = 10004,
            NotFoundDevice = 10005,
            NotFoundCustomer = 10006,
            NothingToDo = 10007,
            DoNotAllow = 10008,
            NotFoundTenant = 10009,
            ExceptionDeviceIdentity = 10010,
            RPCFailed = 10011,
            RPCTimeout = 10012,
            CustomerDoesNotHaveDevice = 10013,
            CreateUserFailed = 10014,

            CantFindObject = 10015,
            InValidData = 10016,
            NotFoundProduce = 10017,
            NotFile = 10018,
            Empty = 10019,
        }

}
