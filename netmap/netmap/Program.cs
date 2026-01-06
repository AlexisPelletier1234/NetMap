
using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Text;

class Program
{
    private static int timeout = 150;


    static readonly Dictionary<int, string> Services = new()
    {
        { 20, "FTP-Data" },
        { 21, "FTP" },
        { 22, "SSH" },
        { 23, "Telnet" },
        { 25, "SMTP" },
        { 53, "DNS" },
        { 67, "DHCP" },
        { 68, "DHCP" },
        { 69, "TFTP" },
        { 80, "HTTP" },
        { 110, "POP3" },
        { 123, "NTP" },
        { 137, "NetBIOS" },
        { 138, "NetBIOS" },
        { 139, "SMB" },
        { 143, "IMAP" },
        { 161, "SNMP" },
        { 162, "SNMP-Trap" },
        { 389, "LDAP" },
        { 443, "HTTPS" },
        { 445, "SMB" },
        { 514, "Syslog" },
        { 587, "SMTP-Submission" },
        { 631, "IPP" },
        { 636, "LDAPS" },
        { 989, "FTPS" },
        { 990, "FTPS" },
        { 993, "IMAPS" },
        { 995, "POP3S" },
        { 1433, "MSSQL" },
        { 1521, "Oracle DB" },
        { 1723, "PPTP" },
        { 1883, "MQTT" },
        { 2049, "NFS" },
        { 2375, "Docker" },
        { 2376, "Docker TLS" },
        { 3306, "MySQL" },
        { 3389, "RDP" },
        { 5432, "PostgreSQL" },
        { 5672, "AMQP/RabbitMQ" },
        { 5900, "VNC" },
        { 5985, "WinRM" },
        { 5986, "WinRM (HTTPS)" },
        { 6379, "Redis" },
        { 6443, "Kubernetes API" },
        { 6667, "IRC" },
        { 8000, "HTTP-Alt" },
        { 8008, "HTTP-Alt" },
        { 8080, "HTTP-Proxy" },
        { 8443, "HTTPS-Alt" },
        { 9000, "SonarQube / HTTP" },
        { 9200, "Elasticsearch" },
        { 9300, "Elastic Transport" }
    };

    static void scanRapide(string ip)
    {

        int[] portsImportants = new int[]
                {
            20, 21, 22, 23, 25,
            53, 67, 68, 69, 80,
            110, 123, 137, 138, 139,
            143, 161, 162, 389, 443,
            445, 514, 587, 631, 636,
            989, 990, 993, 995, 1433,
            1521, 1723, 1883, 2049, 2375,
            2376, 3306, 3389, 5432, 5672,
            5900, 5985, 5986, 6379, 6443,
            6667, 8000, 8008, 8080, 8443,
            9000, 9200, 9300
                };

        for (int i = 0; i < portsImportants.Length; i++)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    var result = client.BeginConnect(ip, portsImportants[i], null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeout);


                    if (success && client.Connected)
                    {
                        string service = Services.ContainsKey(portsImportants[i])
                            ? Services[portsImportants[i]]
                            : "Inconnu";

                        // Lire la bannière
                        string banniere = LireBanniere(client, portsImportants[i]);

                        if (!string.IsNullOrEmpty(banniere))
                            Console.WriteLine($"Port {portsImportants[i]} Ouvert ({service}) → Bannière : {banniere}");
                        else
                            Console.WriteLine($"Port {portsImportants[i]} Ouvert ({service})");
                    }


                }
                catch
                {
                    Console.WriteLine("Erreur lors de la lecture du port " + portsImportants[i]);
                }
            }
            Console.Write($" \rProgression : ({i + 1}/{portsImportants.Length}) ");
        }

    }
    static void scanComplet(string ip)
    {
        const int NbPorts = 65536;
        for (int i = 1; i < NbPorts; i++)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    var result = client.BeginConnect(ip, i, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeout);


                    if (success && client.Connected)
                    {
                        string service = Services.ContainsKey(i) ? Services[i] : "Inconnu";
                        string banniere = LireBanniere(client, i);

                        if (!string.IsNullOrEmpty(banniere))
                            Console.WriteLine($"Port {i} Ouvert ({service}) → Bannière : {banniere}");
                        else
                            Console.WriteLine($"Port {i} Ouvert ({service})");
                    }


                }
                catch
                {
                    Console.WriteLine("Erreur lors de la lecture du port " + i);
                }
            }
            Console.Write($" \rProgression : ({i+1}/{NbPorts}) ");
        }
    }

    private static bool ipValide(string ip)
    {
        bool esValide = false;
        Regex reg = new Regex(@"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."+@"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."+@"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."+@"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$");

        if (reg.IsMatch(ip))
        {
            esValide = true;
        }

        return esValide;
    }

    private static bool optionValide(string options)
    {
        bool esValide = false;

        if (options == "R" || options == "C" || options == "H")
        {
            esValide = true;
        }

        return esValide;
        
    }


    static string LireBanniere(TcpClient client, int port)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = 500;

            // ⚠️ attendre un peu avant de lire
            Thread.Sleep(200);

            byte[] buffer = new byte[512];

            // 🔥 1ère tentative de lecture
            if (stream.DataAvailable)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read > 0)
                    return Encoding.ASCII.GetString(buffer, 0, read).Trim();
            }

            // 🔥 tentative HTTP si rien reçu
            if (port == 80 || port == 8080 || port == 8000 || port == 8008 || port == 443)
            {
                string req = "HEAD / HTTP/1.1\r\nHost: localhost\r\n\r\n";
                byte[] data = Encoding.ASCII.GetBytes(req);
                stream.Write(data, 0, data.Length);

                Thread.Sleep(150);

                if (stream.DataAvailable)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                        return Encoding.ASCII.GetString(buffer, 0, read).Trim();
                }
            }
        }
        catch
        {
            // Rien = pas de bannière → normal
            return "Erreur";
        }

        return "Aucune version disponible ";
    }


    static void aide()
    {

        Console.WriteLine("Usage :");
        Console.WriteLine("  PortScan <option> <ip>");
        Console.WriteLine();
        Console.WriteLine("Options :");
        Console.WriteLine("  R          Scan rapide des ports courants (web, DB, RDP, etc.)");
        Console.WriteLine("  C          Scan complet de 1 à 65535");
        Console.WriteLine("  H Affiche cette aide");
        Console.WriteLine();
        Console.WriteLine("Exemples :");
        Console.WriteLine("  PortScan R 192.168.1.10");
        Console.WriteLine("  PortScan C 10.0.0.5");
        Console.WriteLine("  PortScan H");
        Console.WriteLine();
        Console.WriteLine("Notes :");
        Console.WriteLine("  - IPv4 uniquement (ex. 192.168.0.12).");
        Console.WriteLine("  - Les ports fermés/filtrés ne s’affichent pas.");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("⚖️  Légal & Éthique : scanne uniquement les hôtes et réseaux pour lesquels tu as une autorisation explicite.");
        Console.ResetColor();


    }
    static void Main(string[] args)
    {
        string ip = "";
        string option = "";



        if (args.Length > 0)
        {
            option = args[0].ToUpper();
        }
        if (args.Length > 1)
        {
            ip = args[1];
        }

        
        if (ipValide(ip) && optionValide(option) || ip.Length==0 && option == "H"){
            if (option == "R")
            {
                scanRapide(ip);
            } else if (option == "C")
            {
                scanComplet(ip);
            } else if (option == "H")
            {
                aide();
            }
        } else
        {
            Console.WriteLine("Erreur de syntaxe dans la commande. Option H pour aide");
        }
    }
}
