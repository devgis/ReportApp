using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ReportData.Common;

namespace ReportData.Web
{
    /// <summary>
    /// register 的摘要说明
    /// </summary>
    public class rpdata : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //post方式
            //context.Request.Form[""];
            //get方式 
            
            string mobile =context.Request.QueryString["m"];
            if (string.IsNullOrEmpty(mobile))
            {
                mobile = context.Request.Form["m"];
            }
            string password = context.Request.QueryString["pw"];
            if (string.IsNullOrEmpty(password))
            {
                password = context.Request.Form["pw"];
            }
            string newpasswrod = context.Request.QueryString["nw"];
            if (string.IsNullOrEmpty(newpasswrod))
            {
                newpasswrod = context.Request.Form["nw"];
            }
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(password))
            {
                //失败
                context.Response.Write(getJsonpStr(0,"null", "手机号密码不能为空！"));
            }

            else
            {
                try
                {
                    if (ReportDataHelper.CheckMobilePassword(mobile, password))
                    {
                        if (!string.IsNullOrEmpty(newpasswrod))
                        {

                            //修改密码
                            if (ReportDataHelper.ChangePassword(mobile, newpasswrod))
                            {
                                context.Response.Write(getJsonpStr(1, "null", "修改密码成功！"));
                            }
                            else
                            {
                                context.Response.Write(getJsonpStr(0, "null", "修改密码失败！"));
                            }
                        }
                        else
                        {
                            //获取用户数据
                            try
                            {
                                string data = JsonConvert.SerializeObject(ReportData.Common.ReportDataHelper.GetUserData(mobile));
                                context.Response.Write(getJsonpStr(1, data, ""));
                            }
                            catch(Exception ex)
                            {
                                context.Response.Write(getJsonpStr(0, "null", "获取数据异常:" + ex.Message));
                            }
                        }

                    
                    }
                    else
                    {
                        //失败
                        context.Response.Write(getJsonpStr(0, "null", "密码不正确！"));
                    }
                }
                catch(Exception ex)
                {
                    context.Response.Write(getJsonpStr(0, "null", ex.Message));
                }
            }
        }

        private string getJsonpStr2(int rs,string data)
        {
            if (rs == 1)
            {
                return "getData({rs:" + rs + ",data:" + data + "})";
            }
            else
            {
                return "getData({rs:" + rs + ",data:'" + data + "'})";
            }
        }

        private string getJsonpStr(int rs, string data,string message)
        {
            return "getData({\"rs\":" + rs + ",\"data\":" + data + ",\"message\":\"" + message + "\"})";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}