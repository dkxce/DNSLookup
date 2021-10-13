using System;
using System.Collections;
using System.Collections.Generic;

namespace DNS
{
    class Program
    {
        static string QueriesToString(List<DNSLookUp.QueryTypes> queries)
        {
            if (queries == null) return "";
            if (queries.Count == 0) return "";
            string res = "";
            foreach (DNSLookUp.QueryTypes q in queries)
                res += (res.Length > 0 ? ", " : "") + q.ToString().Substring(9);
            return res;
        }

        static void OnDataText(string domain, DNSLookUp.QueryTypes qt, string value)
        {
            Console.WriteLine(String.Format("{0} IN {1,-6} {2}", domain, qt.ToString().Replace("DNS_TYPE_", ""), value));
        }

        static void OnResultCode(int code)
        {
            if (code == 0) return;
            System.ComponentModel.Win32Exception ex = new System.ComponentModel.Win32Exception(code);
            Console.WriteLine("DNS Query Result: {0} {1}", code, ex.Message);
        }

        static void Main(string[] args)
        {            
            byte mode = 1;

            List<DNSLookUp.QueryTypes> queries = new List<DNSLookUp.QueryTypes>();
            List<string> domains = new List<string>();

            if (args.Length > 0)
            {
                foreach (string str in args)
                {
                    try { queries.Add((DNSLookUp.QueryTypes)Enum.Parse(typeof(DNSLookUp.QueryTypes), "DNS_TYPE_" + str.ToUpper().Replace("TXT", "TEXT"))); }
                    catch { byte b = 1; if (byte.TryParse(str, out b)) mode = b; else domains.Add(str); };
                };
            };

            if (domains.Count == 0)
            {
                Console.WriteLine("use syntax  :  DNSLookUp.exe [QUERY] <domain>");
                Console.WriteLine("use syntax  :  DNSLookUp.exe [QUERY [QUERY]] <domain> <domain> <domain> ... <domain>");                
                Console.WriteLine("QUERY TYPES :  ALL A NS CNAME TXT SRV MX SOA ... ");
                Console.WriteLine("SAMPLE      :  DNSLookUp.exe A MX TXT yandex.ru");
                return;
            };

            if (queries.Count == 0) queries.Add(DNSLookUp.QueryTypes.DNS_TYPE_ALL);

            Console.WriteLine("MODE {0} : {1}", mode, QueriesToString(queries));
            if (mode == 1)
            {
                foreach (string domain in domains)
                {
                    Console.WriteLine("--- " + domain + " ---");
                    foreach (DNSLookUp.QueryTypes qt in queries)
                    {
                        System.ComponentModel.Win32Exception ex;
                        List<DNSLookUp.ResponseRecord> rr = DNSLookUp.GetRecords(domain, qt, out ex);
                        if ((rr.Count == 0) && (ex != null))
                            Console.WriteLine("DNS Query Result for {2}: {0} {1}", ex.NativeErrorCode, ex.Message, domain);
                        foreach (DNSLookUp.ResponseRecord r in rr)
                            Console.WriteLine(String.Format("{0} IN {1,-6} {2}", r.respName, r.respType.ToString().Replace("DNS_TYPE_", ""), r.respValue));
                    };
                    Console.WriteLine();
                };
            }
            else if (mode == 2)
            {
                foreach (string domain in domains)
                {
                    Console.WriteLine("--- " + domain + " ---");
                    try
                    {
                        if (queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_A) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ALL) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ANY))
                        {
                            System.Net.IPAddress[] a = DNSLookUp.Get_A(domain);
                            foreach (System.Net.IPAddress a_el in a) Console.WriteLine(domain + " IN A      " + a_el.ToString());
                        };
                        if (queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_NS) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ALL) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ANY))
                        {
                            string[] s = DNSLookUp.Get_NS(domain);
                            foreach (string s_el in s) Console.WriteLine(domain + " IN NS     " + s_el);
                        };
                        if (queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_CNAME) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ALL) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ANY))
                        {
                            string[] s = DNSLookUp.Get_CNAME(domain);
                            foreach (string s_el in s) Console.WriteLine(domain + " IN CNAME  " + s_el);
                        };
                        if (queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_TEXT) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ALL) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ANY))
                        {
                            string[] s = DNSLookUp.Get_TXT(domain);
                            foreach (string s_el in s) Console.WriteLine(domain + " IN TXT    " + s_el);
                        };
                        if (queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_SRV) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ALL) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ANY))
                        {
                            string[] s = DNSLookUp.Get_SRV(domain);
                            foreach (string s_el in s) Console.WriteLine(domain + " IN SRV    " + s_el);
                        };
                        if (queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_MX) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ALL) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ANY))
                        {
                            string[] s = DNSLookUp.Get_MX(domain);
                            foreach (string s_el in s) Console.WriteLine(domain + " IN MX     " + s_el);
                        };
                        if (queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_SOA) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ALL) || queries.Contains(DNSLookUp.QueryTypes.DNS_TYPE_ANY))
                        {
                            string[] s = DNSLookUp.Get_SOA(domain);
                            foreach (string s_el in s) Console.WriteLine(domain + " IN SOA    " + s_el);
                        };
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        Console.WriteLine("Error: {0} {1}", ex.NativeErrorCode, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                    };
                    Console.WriteLine();
                };
            }
            else if (mode == 3)
            {
                foreach (string domain in domains)
                {
                    Console.WriteLine("--- " + domain + " ---");
                    System.ComponentModel.Win32Exception ex;
                    List<DNSLookUp.ResponseRecord> rr = DNSLookUp.GetRecords(domain, out ex);
                    if ((rr.Count == 0) && (ex != null))
                        Console.WriteLine("DNS Query Result for {2}: {0} {1}", ex.NativeErrorCode, ex.Message, domain);
                    foreach (DNSLookUp.ResponseRecord r in rr)
                        Console.WriteLine(String.Format("{0} IN {1,-6} {2}", r.respName, r.respType.ToString().Replace("DNS_TYPE_", ""), r.respValue));
                    Console.WriteLine();
                };
            }
            else
            {
                foreach (string domain in domains)
                {
                    Console.WriteLine("--- " + domain + " ---");
                    DNSLookUp.GetRecords(domain, DNSLookUp.QueryTypes.DNS_TYPE_A, OnDataText, OnResultCode);
                    DNSLookUp.GetRecords(domain, DNSLookUp.QueryTypes.DNS_TYPE_NS, OnDataText, OnResultCode);
                    DNSLookUp.GetRecords(domain, DNSLookUp.QueryTypes.DNS_TYPE_CNAME, OnDataText, OnResultCode);
                    DNSLookUp.GetRecords(domain, DNSLookUp.QueryTypes.DNS_TYPE_MX, OnDataText, OnResultCode);
                    DNSLookUp.GetRecords(domain, DNSLookUp.QueryTypes.DNS_TYPE_TEXT, OnDataText, OnResultCode);
                };
            };
        }
    }    
}
