using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ReportData.Common
{
    public class ReportDataHelper
    {
        /// <summary>
        /// 获取所有客户信息
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllCustom()
        {
            string sql = "select g.sj,g.gkmc,lx.rgje,lx.XFE,lx.FDL,CONVERT(varchar(100), sg.dtDate, 20) as dtDate,sgs.vInvName,sgs.fRealMoney,vtemp.SL,vtemp.SYCS from GUSTOMER g  "
            + "left join GKLX lx on lx.lxdm=g.gklx "
            + "left join SG_GATHERING sg on sg.vVIPCard=g.sj "
            + "left join SG_GATHERINGS sgs on sg.vMBillID=sgs.vMBillID "
            + "left join ( "
            + "select cy.GKDM,ISNULL(sum(cx.sl),0) as SL,(ISNULL(sum(cx.sl),0)-ISNULL( sum(cx.sl_1),0)) as SYCS from CUSTOMERHYMX cx "
            + "left join CUSTOMERHY cy on cy.DJBH=cx.DJBH group by cy.GKDM"
            + ") vtemp on g.GKDM=vtemp.GKDM";
            return SQLServerHelper.GetDataTable(sql);
        }

        /// <summary>
        /// 获取当前客户信息
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static DataTable GetUserData(string mobile)
        {
            //string sql = "select g.gkmc,lx.rgje,lx.XFE,lx.FDL,CONVERT(varchar(100), sg.dtDate, 20) as dtDate ,sgs.vInvName,sgs.fRealMoney,ISNULL(cx.SL,0) as SL,(ISNULL(cx.SL,0)-ISNULL(cx.SL_1,0)) as SYCS from GUSTOMER g  "
            //+ "left join GKLX lx on g.gklx=lx.lxdm "
            //+ "left join SG_GATHERING sg on g.sj=sg.vVIPCard "
            //+ "left join SG_GATHERINGS sgs on sg.vMBillID=sgs.vMBillID "
            //+ "left join CUSTOMERHY cy on g.GKDM=cy.GKDM "
            //+ "left join CUSTOMERHYMX cx on cy.DJBH=cx.DJBH"

            //string sql = "select g.sj,g.gkmc,lx.rgje,lx.XFE,lx.FDL,CONVERT(varchar(100), sg.dtDate, 20) as dtDate,sgs.vInvName,sgs.fRealMoney,vtemp.SL,vtemp.SYCS,sh.PRDT,sh.AMT,sh.[DATE] from GUSTOMER g  "
            //+ "left join GKLX lx on lx.lxdm=g.gklx "
            //+ "left join SG_GATHERING sg on sg.vVIPCard=g.sj "
            //+ "left join SG_GATHERINGS sgs on sg.vMBillID=sgs.vMBillID "
            //+ "left join ( "
            //+ "select cy.GKDM,ISNULL(sum(cx.sl),0) as SL,(ISNULL(sum(cx.sl),0)-ISNULL( sum(cx.sl_1),0)) as SYCS from CUSTOMERHYMX cx "
            //+ "left join CUSTOMERHY cy on cy.DJBH=cx.DJBH group by cy.GKDM"
            //+ ") vtemp on g.GKDM=vtemp.GKDM "
            //+ "left join SALE_HIS sh on g.sj=sh.mobil and sh.PRDT= sgs.vInvName "
            //+ string.Format(" where sgs.fRealMoney >0 and g.sj='{0}'", mobile);

            string sql = string.Format(
                " (select distinct g.sj,g.gkmc,lx.rgje,lx.XFE,lx.FDL,CONVERT(varchar(100), sh.[DATE], 20) as dtDate,sh.PRDT as vInvName,sh.AMT as fRealMoney,vtemp.SL,vtemp.SYCS"
                +"  from GUSTOMER g  "
                +" left join GKLX lx on lx.lxdm=g.gklx left join SG_GATHERING sg on sg.vVIPCard=g.sj "
                +" left join ( select cy.GKDM,ISNULL(sum(cx.sl),0) as SL,(ISNULL(sum(cx.sl),0)-ISNULL( sum(cx.sl_1),0)) as SYCS from CUSTOMERHYMX cx "
                +" left join CUSTOMERHY cy on cy.DJBH=cx.DJBH group by cy.GKDM) vtemp on g.GKDM=vtemp.GKDM"
                +" left join SALE_HIS sh on g.sj=sh.mobil"
                +"  where g.sj='{0}' and sh.PRDT is not null )"
                +"  union"
                +"  (select g.sj,g.gkmc,lx.rgje,lx.XFE,lx.FDL,CONVERT(varchar(100), sg.dtDate, 20) as dtDate,sgs.vInvName,sgs.fRealMoney,vtemp.SL,vtemp.SYCS"
                +"  from GUSTOMER g"  
                +" left join GKLX lx on lx.lxdm=g.gklx left join SG_GATHERING sg on sg.vVIPCard=g.sj "
                +" left join SG_GATHERINGS sgs on sg.vMBillID=sgs.vMBillID "
                +" left join ( select cy.GKDM,ISNULL(sum(cx.sl),0) as SL,(ISNULL(sum(cx.sl),0)-ISNULL( sum(cx.sl_1),0)) as SYCS from CUSTOMERHYMX cx "
                +" left join CUSTOMERHY cy on cy.DJBH=cx.DJBH group by cy.GKDM) vtemp on g.GKDM=vtemp.GKDM"
                +" left join SALE_HIS sh on g.sj=sh.mobil and sh.PRDT= sgs.vInvName"
                +"  where sgs.fRealMoney >0 and g.sj='{0}')"
                , mobile);
            return SQLServerHelper.GetDataTable(sql);
        }

        public static bool CheckMobilePassword(string mobile, string password)
        {
            try
            {
                string sql = string.Format("select * from GUSTOMER where sj={0}", mobile);
                DataTable dt = SQLServerHelper.GetDataTable(sql);
                if (dt == null)
                {
                    throw new Exception("查找失败！");
                }
                else if (dt.Rows.Count <= 0)
                {
                    throw new Exception("用户信息不存在！");
                }

                sql = string.Format("select * from GKWXPWD where mobile={0}", mobile);
                dt = SQLServerHelper.GetDataTable(sql);
                if (dt.Rows != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (password.Equals(dt.Rows[0]["password"].ToString()))
                        {
                            return true;
                        }
                    }
                    else if (dt.Rows.Count == 0)
                    {
                        if (password.Equals("123456"))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool ChangePassword(string mobile, string password)
        {
            try
            {
                string sql = string.Format("select * from GKWXPWD where mobile='{0}'", mobile);
                DataTable dt = SQLServerHelper.GetDataTable(sql);
                sql = string.Empty;
                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    //更新
                    sql = string.Format("update GKWXPWD set password='{1}' where mobile='{0}'", mobile, password);
                }
                else
                {
                    //新增
                    sql = string.Format("insert into GKWXPWD (mobile,password)values('{0}','{1}')", mobile, password);
                }

                return SQLServerHelper.ExecSql(sql, null);
            }
            catch
            {
                return false;
            }
        }
    }
}
