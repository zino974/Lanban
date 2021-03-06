﻿using Lanban.AccessLayer;
using Lanban.Model;
using System;
using System.Threading;
using System.Web;

namespace Lanban
{
    public class ChartHandlerOperation : HandlerOperation
    {
        ChartAccess myAccess;

        public ChartHandlerOperation(AsyncCallback callback, HttpContext context, Object state)
            : base(callback, context, state)
        {
            myAccess = new ChartAccess();
        }

        public void QueueWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartTask), null);
        }

        private void StartTask(Object workItemState)
        {
            var param = _context.Request.Params;
            int projectID = Convert.ToInt32(param["projectID"]);
            var user = (UserModel)_context.Session["user"];
            try
            {
                if (myAccess.IsProjectMember(projectID, user.User_ID, user.Role))
                {
                    _context.Response.ContentType = "application/json";
                    switch (action)
                    {
                        /***********************************************/
                        // Working with chart in Board.aspx
                        // Get Pie Chart data
                        case "getPieChart":
                            result = myAccess.getPieChart(projectID);
                            break;

                        // Get Bar Chart data
                        case "getBarChart":
                            result = myAccess.getBarChart(projectID);
                            break;

                        // Get Line Graph data
                        case "getBurnUpChart":
                            result = myAccess.getBurnUpChart(projectID);
                            break;
                        // Get Line Graph data
                        case "getBurnDownChart":
                            result = myAccess.getBurnDownChart(projectID);
                            break;
                        // Get Estimation factor data
                        case "getEstimationFactor":
                            result = myAccess.getEstimationFactor(projectID);
                            _context.Response.ContentType = "text/plain";
                            break;
                    }
                }
                else Code = 401;
            }
            catch
            {
                Code = 403;
            }
            finally
            {
                myAccess.Dipose();
                FinishWork();
            }
        }
    }
}