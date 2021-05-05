using Unity.Entities;
using Unity.Jobs;
using Unity.Tiny.UI;

using urlopener;
using Housing.Component;

namespace Housing
{

    /// <summary>
    /// 部屋のレイアウトをツイートする
    /// </summary>
    /// <remarks>
    /// TinyではSystem.Convertが使えない
    /// </remarks>

    [UpdateInGroup(typeof(MyUISystemGroup))]
    public class TweetSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entity eClicked = Entity.Null;
            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            Entity twiButtonEntity = uiSystem.GetEntityByUIName("Twitter");
        
            Entities.ForEach((Entity e, in UIState state) => {
                if(state.IsClicked)
                {
                    eClicked = e;
                }
            }).Run();

            if(eClicked != null && eClicked != Entity.Null)
            {
                if(eClicked == twiButtonEntity)
                {
                    // 家具をクエリパラメータに変換する
                    /*
                        反転-識別ID-座標
                        X-XXXXXXX-XXXXXXXX

                        家具1つあたり16bit
                    */
                    string bits = "";
                    Entities
                        .WithoutBurst()
                        .ForEach((in Furniture furniture, in InRoomPos pos) =>
                        {
                            byte id = furniture.ID;
                            byte flip = (byte)(furniture.isFlip?0b1000_0000:0b0000_0000);
                            id = (byte)(id | flip);
                            byte p = (byte)(pos.Value.x + pos.Value.y*16);
                            bits += ConvertByteToBitString(id);
                            bits += ConvertByteToBitString(p);
                        }).Run();
                    
                    var base64 = ConvertBitsToBase64URLString(bits);
                    var query = "";
                    if(!base64.Equals(""))
                    {
                        query = "?room=" + base64;
                    }
                    
                    URLOpener.OpenURL("マイルームを作りました%0ahttps://poi-parfait.me/Room"+query);
                }
            }
        }

        private string ConvertByteToBitString(byte b)
        {
            string ret = "";
            for(;b>0;b>>=1)
            {
                ret = (b&1) + ret;
            }

            for(;ret.Length<8;)
            {
                ret = 0 + ret;
            }
            return ret;
        }
        private string ConvertBitsToBase64URLString(string data)
        {
            string ret = "";
            string bit6 = "";
            for(int i=0;i<data.Length;i++)
            {
                bit6 += data[i];
                if(bit6.Length == 6)
                {
                    int index = ConvertBit6ToInt(bit6);
                    ret += ConversionTable[index];
                    bit6 = "";
                }
            }
            if(bit6.Length > 0)
            {
                for(;bit6.Length<6;)
                {
                    bit6 += '0';
                }
                int index = ConvertBit6ToInt(bit6);
                ret += ConversionTable[index];
            }
            return ret;
        }
        private int ConvertBit6ToInt(string bit6)
        {
            int index =
                ((bit6[0]=='1')?1:0) * 32
                + ((bit6[1]=='1')?1:0) * 16
                + ((bit6[2]=='1')?1:0) * 8
                + ((bit6[3]=='1')?1:0) * 4
                + ((bit6[4]=='1')?1:0) * 2
                + ((bit6[5]=='1')?1:0) * 1;
            return index;
        }
        private char[] ConversionTable = new char[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
            'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
            'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', '-', '_'
        };
    }
    
}

