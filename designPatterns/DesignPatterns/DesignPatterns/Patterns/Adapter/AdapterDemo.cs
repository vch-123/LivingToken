using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 新接口（客户端期待的接口）
public interface IAudioPlayer
{
    void PlaySound();
}

// 老接口（已有的类）
public class RoundHoleSpeaker
{
    public void PlayRoundSound()
    {
        Console.WriteLine("🔊 老式圆孔音响正在播放声音");
    }
}

// 适配器（让 RoundHoleSpeaker 适配 IAudioPlayer）
public class RoundToSquareAdapter : IAudioPlayer
{
    private readonly RoundHoleSpeaker _oldSpeaker;

    public RoundToSquareAdapter(RoundHoleSpeaker speaker)
    {
        _oldSpeaker = speaker;
    }

    public void PlaySound()
    {
        // 内部调用老接口
        _oldSpeaker.PlayRoundSound();
    }
}


//class Program
//{
//    static void Main()
//    {
//        // 老音响
//        RoundHoleSpeaker roundSpeaker = new RoundHoleSpeaker();

//        // 使用适配器，转换成新接口使用方式
//        IAudioPlayer adapter = new RoundToSquareAdapter(roundSpeaker);

//        adapter.PlaySound();  // ✅ 像用新接口一样调用老功能
//    }
//}
