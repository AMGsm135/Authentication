namespace Amg.Authentication.Application.Contract.Dtos
{
    public class OtpActivationResult
    {
        public string SecretCode { get; set; }
        public byte[] QrImage { get; set; }

    }
}
