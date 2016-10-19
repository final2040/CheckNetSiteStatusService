using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Services
{
    public static class ConvertToSecureStringExtension
    {
        
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