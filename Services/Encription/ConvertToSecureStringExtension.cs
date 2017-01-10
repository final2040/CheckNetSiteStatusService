using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Services.Encription
{
    /// <summary>
    /// Clase de extension para String y SecureString que añade metodos para convertir de
    /// SecureString a String y visceversa
    /// </summary>
    public static class ConvertToSecureStringExtension
    {
        /// <summary>
        /// Coonvierte una cadena SecureString a String
        /// </summary>
        /// <param name="secureString"></param>
        /// <returns>Cadena de texto</returns>
        public static string ConvertToString(this SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("secureString");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        
        /// <summary>
        /// Convierte una cadena de texto a un SecureString
        /// </summary>
        /// <param name="unSecureString"></param>
        /// <returns>SecureString con el contenido de la cadena</returns>
        public static SecureString ConvertToSecureString(this string unSecureString)
        {
            if (unSecureString == null)
                throw new ArgumentNullException("unSecureString");

            var securePassword = new SecureString();
            foreach (char c in unSecureString)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}