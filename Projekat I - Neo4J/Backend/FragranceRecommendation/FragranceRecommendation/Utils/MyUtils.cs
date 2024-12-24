namespace FragranceRecommendation.Utils;

public static class MyUtils
{
    public static (bool IsValid, string ErrorMessage) IsValidPassword(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return (false, "Enter the password");
        if (str.Length < 8)
            return (false, "Password must be at least 8 characters");

        bool hasDigit = false;
        foreach (var ch in str)
        {
            if (char.IsDigit(ch))
            {
                hasDigit = true;
                break;
            }
        }

        bool hasLetter = false;
        foreach (var ch in str)
        {
            if (char.IsLetter(ch))
            {
                hasLetter = true;
                break;
            }
        }

        if (!hasDigit)
            return (false, "Password must containt at least 1 number");

        if (!hasLetter)
            return (false, "Password must containt at least 1 letter");

        return (true, "");
    }

    public static (bool IsValid, string ErrorMessage) IsValidString(string str, string stringError)
    {
        if (string.IsNullOrWhiteSpace(str))
            return (false, $"{stringError} cannot be empty");

        if (str.Length < 3)
            return (false, $"{stringError} must be at least 3 characters");

        return (true, string.Empty);
    }

    public static (bool IsValid, string ErrorMessage) IsValidGender(char gender)
    {
        switch (gender)
        {
            case 'M':
                return (true, string.Empty);
            case 'F':
                return (true, string.Empty);
            case 'U':
                return (true, string.Empty);
            default:
                return (false, $"{gender} is not a valid gender. It must be either 'M', 'F', 'U'.");
        }
    }
}