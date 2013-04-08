// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using jsonbroker.library.common.log;
using jsonbroker.library.server.broker;
using jsonbroker.library.common.broker;
using jsonbroker.library.common.exception;
using System.Windows.Forms;
using jsonbroker.library.common.work;

namespace jsonbroker.library.windows
{

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class WebBrowserBridge : JavascriptCallbackAdapter
    {
        private static Log log = Log.getLog(typeof(WebBrowserBridge));


        ////////////////////////////////////////////////////////////////////////
        //
        WebBrowser _webBrowser;

        ////////////////////////////////////////////////////////////////////////
        //
        ServicesRegistery _servicesRegistery;

        ////////////////////////////////////////////////////////////////////////
        //
        public WebBrowserBridge(WebBrowser webBrowser, ServicesRegistery servicesRegistery)
        {
            _webBrowser = webBrowser;
            _servicesRegistery = servicesRegistery;
        }

        public void dispatch(String json)
        {
            log.enteredMethod();

            BrokerJob job = new BrokerJob(json, this, _servicesRegistery);
            WorkManager.enqueue(job);
        }

        private void postJavascript(String javascript)
        {
            MethodInvoker invokeScript = delegate
            {
                log.debug(javascript, "javascript");

                // vvv http://www.codeproject.com/tips/60924/Using-WebBrowser-Document-InvokeScript-to-mess-aro.aspx
                _webBrowser.Document.InvokeScript("eval", new object[] { javascript });
                // ^^^ http://www.codeproject.com/tips/60924/Using-WebBrowser-Document-InvokeScript-to-mess-aro.aspx
                log.debug("done");
            };
            _webBrowser.Invoke(invokeScript);

        }

        public void onFault(BrokerMessage request, Exception fault)
        {
            log.enteredMethod();
            String javascript = JavascriptCallbackAdapterHelper.buildJavascriptFault(request, fault);
            postJavascript(javascript);
        }

        public void onResponse(BrokerMessage request, BrokerMessage response)
        {
            log.enteredMethod();
            String javascript = JavascriptCallbackAdapterHelper.buildJavascriptResponse(response);
            postJavascript(javascript);
        }
    }
}
