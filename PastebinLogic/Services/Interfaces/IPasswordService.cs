namespace PastebinLogic.Services.Interfaces;

internal interface IPasswordService
{
    public (string salt, string hash) GeneratePasswordSaltAndHash(string password);

    public bool PasswordIsValid(string salt, string hash, string password);
}
