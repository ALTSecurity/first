using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace ALTSecurity.Web.Models
{
    public static class DbQueries
    {
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["AltSecurityDb"].ConnectionString;

        public static int IsExists(string domainName)
        {
            int company_id = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                {
                    cmd.CommandText = @"SELECT COMPANY_ID
                                        FROM COMPANY
                                        WHERE DOMAIN_NAME = @DOMAIN_NAME";
                    cmd.Parameters.AddWithValue("DOMAIN_NAME", domainName);

                    company_id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return company_id;
        }

        public static int SaveDorkFiles(string domainName, List<DorkFile> files)
        {
            int companyId = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                {
                    cmd.CommandText = @"INSERT INTO COMPANY
                                        (DOMAIN_NAME)
                                        VALUES
                                        (@DOMAIN_NAME);
                                        SELECT SCOPE_IDENTITY()";
                    cmd.Parameters.AddWithValue("DOMAIN_NAME", domainName);

                    companyId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (domainName == "khortitsa.com")
                {
                    using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                    {
                        cmd.CommandText = @"INSERT INTO LEAKAGE
                                        (URL, MENTIONS, COMPANY_ID)
                                        VALUES
                                        (@URL, @MENTIONS, @COMPANY_ID);";
                        cmd.Parameters.AddWithValue("URL", "https://pastebin.com/Lgmvfygb");
                        cmd.Parameters.AddWithValue("MENTIONS", 2);
                        cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (var item in files)
                {
                    int fileId = 0;
                    using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                    {
                        cmd.CommandText = @"INSERT INTO DORKFILE
                                            (FILE_NAME, FILE_URL, COMPANY_ID)
                                            VALUES
                                            (@FILE_NAME, @FILE_URL, @COMPANY_ID);
                                            SELECT SCOPE_IDENTITY()";
                        cmd.Parameters.AddWithValue("FILE_NAME", item.FileName);
                        cmd.Parameters.AddWithValue("FILE_URL", item.Url);
                        cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                        fileId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    foreach(var meta in item.Metadata)
                    {
                        using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                        {
                            cmd.CommandText = @"INSERT INTO METADATA
                                                (TYPE, VALUE, ISAUTH, ISACTUAL, NOTICE, DORKFILE_ID)
                                                VALUES
                                                (@TYPE, @VALUE, @ISAUTH, @ISACTUAL, @NOTICE, @DORKFILE_ID);";
                            cmd.Parameters.AddWithValue("TYPE", (int)meta.Type);
                            cmd.Parameters.AddWithValue("VALUE", meta.Value == null ? string.Empty : meta.Value);
                            cmd.Parameters.AddWithValue("ISAUTH", meta.IsAuth? 1: 0);
                            cmd.Parameters.AddWithValue("ISACTUAL", meta.IsActual ? 1 : 0);
                            cmd.Parameters.AddWithValue("NOTICE", meta.Notice == null ? string.Empty: meta.Notice);
                            cmd.Parameters.AddWithValue("DORKFILE_ID", fileId);

                            cmd.ExecuteNonQuery();
                        }  
                    }
                }
            }

            return companyId;
        }


        public static void SaveServiveInfo(int companyId, List<ServiceResult> resources)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                int subdomainId = 0;
                foreach (var item in resources)
                {
                    using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                    {
                        cmd.CommandText = @"INSERT INTO SUBDOMAIN
                                        (NAME, STATE, COMPANY_ID)
                                        VALUES
                                        (@NAME, @STATE, @COMPANY_ID);
                                        SELECT SCOPE_IDENTITY()";
                        cmd.Parameters.AddWithValue("NAME", item.DomainName);
                        cmd.Parameters.AddWithValue("STATE", (int)item.State);
                        cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                        subdomainId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (item.Services != null && item.Services.Count > 0)
                    {
                        for (var i = 0; i < item.Services.Count; i++)
                        {
                            var service = item.Services[i];
                            using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                            {
                                cmd.CommandText = @"INSERT INTO SERVICE
                                                    (SERVICE_NAME, CVE_ID, SUBDOMAIN_ID)
                                                    VALUES
                                                    (@SERVICE_NAME, @CVE_ID, @SUBDOMAIN_ID)";
                                cmd.Parameters.AddWithValue("SERVICE_NAME", (string.IsNullOrEmpty(service.Service.Product)? service.Service.Name: service.Service.Product) + 
                                    (string.IsNullOrEmpty(service.Service.Version)? string.Empty: service.Service.Version));
                                cmd.Parameters.AddWithValue("CVE_ID", item.Cve[i]);
                                cmd.Parameters.AddWithValue("SUBDOMAIN_ID", subdomainId);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }


        public static RiskModel FillRisks(int companyId)
        {
            RiskModel res = new RiskModel();
            res.TriggeredRules = new List<TriggeredRule>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var dorks = GetMetadataPreview(companyId, connection);

                if(dorks != null && dorks.Count > 0)
                {
                    double[] prob = new double[dorks.Count];
                    for(var i = 0; i < dorks.Count; i++)
                    {
                        double tempProb = 0;
                        if (dorks[i].Metadata.Count > 0)
                        {
                            if (dorks[i].Metadata[0].IsActual)
                            {
                                tempProb += 0.25;
                            }

                            if (dorks[i].Metadata[0].IsAuth)
                            {
                                tempProb += 0.25;
                            }
                        }
                        
                        prob[i] = tempProb;
                    }

                    TriggeredRule mtRule = new TriggeredRule();
                    mtRule.RuleType = RuleType.metadata;
                    mtRule.Data = dorks;
                    mtRule.SourceCount = dorks.Count;

                    res.SourceCount += dorks.Count;

                    if (prob.Length > 0)
                    {
                        if (prob.Length == 1)
                        {
                            mtRule.ActionProbability = prob[0];
                        }
                        else
                        {
                            double mix = 1;
                            for(var j = 0; j < prob.Length; j++)
                            {
                                mix *= 1 - prob[j];
                            }

                            mtRule.ActionProbability = 1 - mix;
                        }
                    }

                    res.TriggeredRules.Add(mtRule);
                }

                var openIps = OpenIpsPreview(companyId, connection);

                if(openIps.Item2 > 0)
                {
                    res.TriggeredRules.Add(new TriggeredRule
                    {
                        RuleType = RuleType.ips,
                        Data = openIps,
                        ActionProbability = (double)openIps.Item1/ (double)openIps.Item2,
                        SourceCount = 1
                    });

                    res.SourceCount++;
                }

                var leakage = GetLeakegePreview(companyId, connection);

                if (!string.IsNullOrEmpty(leakage.Url))
                {
                    res.TriggeredRules.Add(new TriggeredRule
                    {
                        RuleType = RuleType.leakage,
                        Data = leakage,
                        ActionProbability = leakage.Mentions >= 2 ? 0.5 : 0.25,
                        SourceCount = leakage.Mentions
                    });

                    res.SourceCount += leakage.Mentions;
                }

                var vuln = GetVulnPreview(companyId, connection);

                if(vuln.Item2 > 0 && vuln.Item1 > 0)
                {
                    res.TriggeredRules.Add(new TriggeredRule
                    {
                        RuleType = RuleType.versions,
                        Data = vuln,
                        ActionProbability = (double)vuln.Item1/(double)vuln.Item2,
                        SourceCount = vuln.Item1
                    });

                    res.SourceCount+= vuln.Item1;
                }
            }

            return res;
        }


        private static List<DorkFile> GetMetadataPreview(int companyId, SqlConnection connection)
        {
            List<DorkFile> res = new List<DorkFile>();

            using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
            {
                cmd.CommandText = @"SELECT FILE_ID
                                    FROM DORKFILE
                                    WHERE COMPANY_ID = @COMPANY_ID";
                cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            res.Add(new DorkFile
                            {
                                FileId = Convert.ToInt32(reader["FILE_ID"])
                            });
                        }
                    }
                }
            }


            if(res.Count > 0)
            {
                foreach(var item in res)
                {
                    using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                    {
                        cmd.CommandText = @"SELECT TYPE, ISAUTH, ISACTUAL, NOTICE
                                            FROM METADATA
                                            WHERE DORKFILE_ID = @DORKFILE_ID";
                        cmd.Parameters.AddWithValue("DORKFILE_ID", item.FileId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    item.Metadata.Add(new Metadata
                                    {
                                        Type = (MetadataType)Convert.ToInt32(reader["TYPE"]),
                                        IsActual = Convert.ToInt32(reader["ISACTUAL"]) == 1,
                                        IsAuth = Convert.ToInt32(reader["ISAUTH"]) == 1,
                                        Notice = Convert.ToString(reader["NOTICE"])
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }


        private static Tuple<int,int> OpenIpsPreview(int companyId, SqlConnection connection)
        {
            Tuple<int, int> res = null;

            using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
            {
                cmd.CommandText = @"SELECT COUNT(ID) as OPENED,
                                           (SELECT COUNT(ID)
                                            FROM SUBDOMAIN
                                            WHERE COMPANY_ID = @COMPANY_ID ) as CHECKED
                                            FROM SUBDOMAIN
                                            WHERE COMPANY_ID = @COMPANY_ID
                                            AND (STATE = 0 OR STATE = 1)";
                cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            res = new Tuple<int, int>(Convert.ToInt32(reader["OPENED"]), Convert.ToInt32(reader["CHECKED"]));
                        }
                    }
                }
            }

            return res;
        }


        private static Leakege GetLeakegePreview(int companyId, SqlConnection connection)
        {
            Leakege res = new Leakege();

            using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
            {
                cmd.CommandText = @"SELECT URL, MENTIONS
                                    FROM LEAKAGE 
                                    WHERE COMPANY_ID = @COMPANY_ID";
                cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            res.Url = Convert.ToString(reader["URL"]);
                            res.Mentions = Convert.ToInt32(reader["MENTIONS"]);
                        }
                    }
                }
            }

            return res;
        }

        private static Tuple<int, int> GetVulnPreview(int companyId, SqlConnection connection)
        {
            Tuple<int, int> res = null;

            using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
            {
                cmd.CommandText = @"SELECT (SELECT COUNT(s.ID) FROM ALTSECURITY.dbo.SERVICE s
                                     LEFT JOIN ALTSECURITY.dbo.SUBDOMAIN sb ON s.SUBDOMAIN_ID = sb.ID
                                     LEFT JOIN ALTSECURITY.dbo.COMPANY c ON sb.COMPANY_ID = c.COMPANY_ID
                                     WHERE c.COMPANY_ID = @COMPANY_ID) as CHECKED,
                                     COUNT(s.ID) FOUND
                                     FROM ALTSECURITY.dbo.SERVICE s
                                     LEFT JOIN ALTSECURITY.dbo.SUBDOMAIN sb ON s.SUBDOMAIN_ID = sb.ID
                                     LEFT JOIN ALTSECURITY.dbo.COMPANY c ON sb.COMPANY_ID = c.COMPANY_ID
                                     WHERE CVE_ID != '' AND c.COMPANY_ID = @COMPANY_ID";
                cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            res = new Tuple<int, int>(Convert.ToInt32(reader["FOUND"]), Convert.ToInt32(reader["CHECKED"]));
                        }
                    }
                }
            }

            return res;
        }

        public static List<DorkFile> GetDorksList(int companyId)
        {
            List<DorkFile> res = new List<DorkFile>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                {
                    cmd.CommandText = @"SELECT FILE_ID, FILE_NAME, FILE_URL
                                    FROM DORKFILE
                                    WHERE COMPANY_ID = @COMPANY_ID";
                    cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                res.Add(new DorkFile
                                {
                                    FileId = Convert.ToInt32(reader["FILE_ID"]),
                                    FileName = Convert.ToString(reader["FILE_NAME"]),
                                    Url = Convert.ToString(reader["FILE_URL"])
                                });
                            }
                        }
                    }
                }


                if (res.Count > 0)
                {
                    foreach (var item in res)
                    {
                        using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                        {
                            cmd.CommandText = @"SELECT TYPE, ISAUTH, ISACTUAL, NOTICE, VALUE
                                            FROM METADATA
                                            WHERE DORKFILE_ID = @DORKFILE_ID";
                            cmd.Parameters.AddWithValue("DORKFILE_ID", item.FileId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        item.Metadata.Add(new Metadata
                                        {
                                            Type = (MetadataType)Convert.ToInt32(reader["TYPE"]),
                                            IsActual = Convert.ToInt32(reader["ISACTUAL"]) == 1,
                                            IsAuth = Convert.ToInt32(reader["ISAUTH"]) == 1,
                                            Notice = Convert.ToString(reader["NOTICE"]),
                                            Value = Convert.ToString(reader["VALUE"])
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        public static List<ServiceResult> GetOpenSubdomains(int companyId)
        {
            List<ServiceResult> res = new List<ServiceResult>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                {
                    cmd.CommandText = @"SELECT NAME FROM SUBDOMAIN
                                            WHERE COMPANY_ID = @COMPANY_ID
                                            AND STATE = 0";
                    cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                res.Add(new ServiceResult
                                {
                                    DomainName = Convert.ToString(reader["NAME"])
                                });
                            }
                        }
                    }
                }
            }

            return res;
        }

        public static List<CveRes> GetCve(int companyId)
        {
            List<CveRes> res = new List<CveRes>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                {
                    cmd.CommandText = @"SELECT SERVICE_NAME, CVE_ID
                                     FROM SERVICE s
                                     LEFT JOIN SUBDOMAIN sb ON s.SUBDOMAIN_ID = sb.ID
                                     LEFT JOIN COMPANY c ON sb.COMPANY_ID = c.COMPANY_ID
                                     WHERE s.CVE_ID != '' AND c.COMPANY_ID = @COMPANY_ID";
                    cmd.Parameters.AddWithValue("COMPANY_ID", companyId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                res.Add(new CveRes
                                {
                                    Service = Convert.ToString(reader["SERVICE_NAME"]),
                                    CveId = Convert.ToString(reader["CVE_ID"])
                                });
                            }
                        }
                    }
                }
            }
            return res;
        }
    }
}