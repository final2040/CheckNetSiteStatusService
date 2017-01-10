using System;
using System.Collections.Generic;
using Services.Encription;

namespace GeneratePassword
{
    /// <summary>
    /// Genera la constraseña encriptada para la configuración de la aplicación.
    /// </summary>
    class Program
    {
        private static string _encryptKey;
        private static bool _useCustomKeyFlag;
        private static string _password;
        private static bool _getHelpFlag;

        static void Main(string[] args)
        {

            if (args.Length == 0 || args.Length > 2)
            {
                ShowHelp();
            }
            else if (args.Length >= 1)
            {
                try
                {
                    ParseArgs(args);
                    if (!_getHelpFlag)
                    {
                        ShowResult();
                    }
                    else
                    {
                        ShowHelp();
                    }

                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private static void ShowResult()
        {
            Console.WriteLine("Password encriptado con exito, nuevo password: " + Environment.NewLine);
            Console.WriteLine();
            Console.WriteLine(Encrypt());
        }

        private static string Encrypt()
        {

            var key = _useCustomKeyFlag ? new Md5Key(_encryptKey, 100) : new Md5Key("airpak-latam", 100);
            var encryptor = new AesEncryptor(key);
            var encryptedPassword = encryptor.EncryptToBase64(_password);
            return encryptedPassword;
        }

        private static void ShowError(string error)
        {
            Console.WriteLine(error);
            Console.WriteLine();
            ShowHelp();
        }

        private static void ParseArgs(string[] args)
        {
            Parse(GetParam(args));
        }

        private static Dictionary<string, string> GetParam(string[] args)
        {
            Dictionary<string, string> paramDictionary = new Dictionary<string, string>();
            foreach (string argument in args)
            {
                if (argument.Contains("/?"))
                {
                    _getHelpFlag = true;
                }
                else
                {
                    if (argument.Split(':').Length < 2)
                        throw new ArgumentException(string.Format("El parámetro {0} no es válido", argument));

                    paramDictionary.Add(argument.Split(':')[0], argument.Split(':')[1]);
                }
            }
            return paramDictionary;
        }

        private static void Parse(Dictionary<string, string> paramDictionary)
        {
            foreach (var param in paramDictionary)
            {
                if (param.Key == "/password")
                {
                    if (string.IsNullOrWhiteSpace(param.Value))
                        throw new ArgumentException("Debe de especificar una contraseña a encriptar..." + Environment.NewLine);

                    _password = param.Value;
                }
                else if (param.Key == "/key")
                {
                    if (string.IsNullOrWhiteSpace(param.Value))
                        throw new ArgumentException("Debe de especificar una llave de encriptación..." +
                                                    Environment.NewLine);
                    _useCustomKeyFlag = true;
                    _encryptKey = param.Value;
                }
                else
                {
                    throw new ArgumentException($"No se reconoce el parámetro: {param.Key}" + Environment.NewLine);
                }
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("GENERATEPASSWORD /password:password [/key:[encryptkey]] /?");
            Console.WriteLine();
            Console.WriteLine("Descripción: ");
            Console.WriteLine("\tEsta herramienta encripta una contraseña para ser " + Environment.NewLine +
                              "\tutilizada por el archivo de configuración del " + Environment.NewLine +
                              "\tservicio de windows NetMonitor.");
            Console.WriteLine();
            Console.WriteLine("Lista de parámetros:");
            Console.WriteLine("\t/password:contraseña\tLa contraseña que se encriptara este parámetro es obligatorio " + Environment.NewLine +
                              "\t/key:llave\tLlave utilizada para encriptar la contraseña" + Environment.NewLine +
                              "\t/?\tMostrar ayuda" + Environment.NewLine);
            Console.WriteLine();
            Console.WriteLine("Ejemplos:");
            Console.WriteLine("\tGENERATEPASSWORD /password:mypassword");
            Console.WriteLine("\tGENERATEPASSWORD /password:mypassword /key:llaveencriptación");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
