using System;
using System.Linq;
using System.Text.RegularExpressions;

public static class PasswordValidator
{
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        // Verifica se a senha tem pelo menos 8 caracteres
        if (password.Length < 8)
        {
            return false;
        }

        // Verifica se contém pelo menos uma letra
        if (!password.Any(char.IsLetter))
        {
            return false;
        }

        // Verifica se contém pelo menos um número
        if (!password.Any(char.IsDigit))
        {
            return false;
        }

        // Verifica se contém pelo menos um caractere especial
        var specialChars = @"[!@#$%^&*(),.?"":{}|<>]";
        if (!Regex.IsMatch(password, specialChars))
        {
            return false;
        }

        return true;
    }
}
