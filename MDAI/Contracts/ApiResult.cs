﻿namespace MDAI.Contracts
{
    public class ApiResult<T> : ApiResult
    {
        public ApiResult(ApiCode _code, string _msg, T data) : base(_code, _msg)
        {
            Data = data;
        }

        public T Data { set; get; }
    }
    public class ApiResult
    {
        public ApiResult()
        {
        }

        public ApiResult(ApiCode _code, string _msg)
        {
            Code = (int)_code;
            Msg = _msg;
        }

        public int Code { get; set; } = (int)ApiCode.Success;
        public string Msg { get; set; } = "OK";
    }

}
