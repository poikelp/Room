using Unity.Mathematics;

namespace Housing
{
    static public class RoomPos
    {
        static public float3 P2W(int2 pos)
        {
            float x = pos.x * 0.5f + pos.y * -0.5f;
            float y = pos.x * 0.25f + pos.y * 0.25f;
            
            return new float3(x, y, 0);
        }

        static public int2 W2P(float3 translation)
        {
            float totalXY = translation.y/0.25f;
            float differenceXY = translation.x/0.5f;

            int x = (int)(((totalXY + differenceXY)/2.0f)/1);
            int y = (int)(((totalXY - differenceXY)/2.0f)/1);
            
            return new int2(x,y);;
        }

        
    }
}
