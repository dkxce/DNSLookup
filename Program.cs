using System;
using System.Collections;
using System.Collections.Generic;

namespace DNS
{
    class Program
    {        
        static void OnDataText(string pName, DNSLookUp.QueryTypes qt, string value)
        {
            Console.WriteLine(pName + " IN " + qt.ToString().Replace("DNS_TYPE_", "") + " " + value);
        }

        static void OnResultCode(int code)
        {
            Console.Write("DNS Query Result: " + code.ToString());
            if (code == 1460) Console.Write(" Timeout Expired");
            if (code == 9001) Console.Write(" DNS server unable to interpret format");
            if (code == 9002) Console.Write(" DNS server failure");
            if (code == 9003) Console.Write(" DNS name does not exist");
            if (code == 9004) Console.Write(" DNS request not supported by name server");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("use syntax: DNSLookUp.exe [QUERY] <domain>");
                Console.WriteLine("             QUERY - ALL/A/NS/CNAME/TXT/SRV/MX/SOA");
            }
            else
            {
                string query = "";
                List<string> domains = new List<string>();
                foreach(string str in args)
                {
                    string s = str.ToUpper();
                    if ((s == "A") || (s == "NS") || (s == "CNAME") || (s == "TXT") || (s == "SRV") || (s == "MX") || (s == "SOA") || (s == "ALL"))
                        query += "["+s+"]";
                    else
                        domains.Add(str);
                };                
                foreach(string domain in domains)
                {
                    Console.WriteLine("Resolve "+domain+":");
                    if ((query.IndexOf("[A]") >= 0) || (query.Length == 0))
                    {
                        System.Net.IPAddress[] a = DNSLookUp.Get_A(domain);
                        foreach (System.Net.IPAddress a_el in a) Console.WriteLine(" " + domain + " IN A " + a_el.ToString());
                    };
                    if ((query.IndexOf("[NS]") >= 0) || (query.Length == 0))
                    {
                        string[] s = DNSLookUp.Get_NS(domain);
                        foreach (string s_el in s) Console.WriteLine(" "+domain + " IN NS " + s_el);
                    };
                    if ((query.IndexOf("[CNAME]") >= 0) || (query.Length == 0))
                    {
                        string[] s = DNSLookUp.Get_CNAME(domain);
                        foreach (string s_el in s) Console.WriteLine(" " + domain + " IN CNAME " + s_el);
                    };
                    if ((query.IndexOf("[TXT]") >= 0) || (query.Length == 0))
                    {
                        string[] s = DNSLookUp.Get_TXT(domain);
                        foreach (string s_el in s) Console.WriteLine(" " + domain + " IN TXT " + s_el);
                    };
                    if ((query.IndexOf("[SRV]") >= 0) || (query.Length == 0))
                    {
                        string[] s = DNSLookUp.Get_SRV(domain);
                        foreach (string s_el in s) Console.WriteLine(" " + domain + " IN SRV " + s_el);
                    };
                    if ((query.IndexOf("[MX]") >= 0) || (query.Length == 0))
                    {
                        string[] s = DNSLookUp.Get_MX(domain);
                        foreach (string s_el in s) Console.WriteLine(" " + domain + " IN MX " + s_el);
                    };
                    if ((query.IndexOf("[SOA]") >= 0) || (query.Length == 0))
                    {
                        string[] s = DNSLookUp.Get_SOA(domain);
                        foreach (string s_el in s) Console.WriteLine(" " + domain + " IN SOA " + s_el);
                    };
                    Console.WriteLine();
                };
            };
            return;

            // http://support.microsoft.com/kb/200525

            // with Heijden.DNS.Resolver
            //Heijden.DNS.Resolver rv = new Heijden.DNS.Resolver();
            //Heijden.DNS.Response resp = rv.Query("zbrozek.ru", Heijden.DNS.QType.NS);

            //string DNSServer = "";
            //if (resp.RecordsNS.Length > 0) DNSServer = resp.RecordsNS[0].NSDNAME;
            //if (DNSServer.Length > 0)
            //{
            //    resp = rv.Query(DNSServer, Heijden.DNS.QType.A);
            //    if (resp.RecordsA.Length > 0)
            //    {
            //        DNSServer = resp.RecordsA[0].Address.ToString();
            //        rv = new Heijden.DNS.Resolver(DNSServer);
            //    };
            //};
            //resp = rv.Query("home.zbrozek.ru", Heijden.DNS.QType.A);
            //if (resp.RecordsA.Length > 0)
            //    Console.WriteLine(resp.RecordsA[0].RR.NAME + " IN A " + resp.RecordsA[0].Address.ToString());
            //return;                        

            string site = "zbrozek.ru";
            Console.WriteLine("DNS Lookup: " + site);
            Console.WriteLine();
            DNSLookUp.GetRecords(site, DNSLookUp.QueryTypes.DNS_TYPE_A, OnDataText, OnResultCode);
            DNSLookUp.GetRecords(site, DNSLookUp.QueryTypes.DNS_TYPE_NS, OnDataText, OnResultCode);
            DNSLookUp.GetRecords(site, DNSLookUp.QueryTypes.DNS_TYPE_MX, OnDataText, OnResultCode);
            DNSLookUp.GetRecords(site, DNSLookUp.QueryTypes.DNS_TYPE_TEXT, OnDataText, OnResultCode);
            Console.WriteLine();
            DNSLookUp.GetRecords("wmobj.milokz.ru", DNSLookUp.QueryTypes.DNS_TYPE_TEXT, OnDataText, OnResultCode);
            DNSLookUp.GetRecords("expires.milokz.ru", DNSLookUp.QueryTypes.DNS_TYPE_TEXT, OnDataText, OnResultCode);
            DNSLookUp.GetRecords("web.milokz.ru", DNSLookUp.QueryTypes.DNS_TYPE_TEXT, OnDataText, OnResultCode);
            DNSLookUp.GetRecords("navimap.milokz.ru", DNSLookUp.QueryTypes.DNS_TYPE_TEXT, OnDataText, OnResultCode);
            Console.WriteLine();
            DNSLookUp.GetRecords("navigatorvl.ru", DNSLookUp.QueryTypes.DNS_TYPE_MX, OnDataText, OnResultCode);
            Console.ReadLine(); 
        }
    }    
}
