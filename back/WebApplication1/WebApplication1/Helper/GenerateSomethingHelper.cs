using System.Text;

namespace WebApplication1.Helper
{
    public class GenerateSomethingHelper
    {
        /// <summary>
        /// 随机生成一个由大写字母A-Z和数字0-9组成的验证码
        /// </summary>
        /// <param name="length">验证码长度</param>
        /// <returns>生成的验证码字符串</returns>
        public static string GenerateVerificationCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }
    }
}
