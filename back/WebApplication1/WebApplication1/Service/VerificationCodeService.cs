using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using WebApplication1.Controllers;
using WebApplication1.Dto;
using WebApplication1.Helper;

namespace WebApplication1.Service
{
    public class VerificationCodeService
    {
        private readonly TimeSpan ExpireAfter = TimeSpan.FromMinutes(5);
        private readonly ConcurrentDictionary<string, (string Code, DateTime CreatedTime)> _verificationCodes = new();


        public bool CheckVerificationCode(UserDto.UserRegistrationDto userRegistrationDto)
        {
            if (_verificationCodes.Any(v => v.Key == userRegistrationDto.UserName))
            {
                if (_verificationCodes[userRegistrationDto.UserName].Code == userRegistrationDto.VerificationCode
                    && DateTime.Now - _verificationCodes[userRegistrationDto.UserName].CreatedTime < ExpireAfter)
                {
                    return true;
                }
            }
            return false;
        }


        public string AddVerificationCodeToDic(UserController.SendCodeRequest req)
        {
            string code = GenerateSomethingHelper.GenerateVerificationCode(6);
            _verificationCodes[req.UserName] = (code, DateTime.Now);
            return code;
        }

        //private readonly IMemoryCache _cache;
        //private readonly ConcurrentDictionary<string, byte> _locks = new();

        //private const int CodeLength = 6;
        //private static readonly TimeSpan ExpireAfter = TimeSpan.FromMinutes(5);//缓存到期时间

        //public VerificationCodeService(IMemoryCache cache) => _cache = cache;

        //public string GenerateCode(string email)
        //{
        //    if (_cache.TryGetValue<string>(email, out var oldCode))
        //        return oldCode;

        //    if (!_locks.TryAdd(email, 0))
        //        throw new InvalidOperationException("验证码已发送，请稍后再试");

        //    try
        //    {
        //        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString($"D{CodeLength}");
        //        var options = new MemoryCacheEntryOptions
        //        {
        //            AbsoluteExpirationRelativeToNow = ExpireAfter,
        //            PostEvictionCallbacks =
        //            {
        //                new PostEvictionCallbackRegistration
        //                {
        //                    EvictionCallback = (key, value, reason, state) =>
        //                        _locks.TryRemove(email, out _)
        //                }
        //            }
        //        };

        //        _cache.Set(email, code, options);
        //        return code;
        //    }
        //    finally
        //    {
        //        _locks.TryRemove(email, out _);
        //    }
        //}

        //public bool ValidateCode(string email, string code)
        //{
        //    var key = email;
        //    if (!_cache.TryGetValue<string>(key, out var cached))
        //        return false;

        //    var ok = string.Equals(cached, code, StringComparison.Ordinal);
        //    if (ok)
        //        _cache.Remove(key);
        //    return ok;
        //}
    }
}
