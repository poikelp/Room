using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using Unity.Mathematics;
using Housing.Component;


namespace Housing
{
    [UpdateInGroup(typeof(CharacterSystemGroup))]
    public class StartWalkSystem : SystemBase
    {
        public const float WAIT_TIME_LIMIT = 3.0f;
        public const int ROOM_SIZE = 16;
        private Unity.Mathematics.Random random;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Player>();
            
            uint seed = (uint)(DateTime.Now.Second+1);
            random = new Unity.Mathematics.Random(seed);
        }

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;
            // List<(Entity entity, List<int2> path)> datas = new List<(Entity, List<int2>)>();
            List<Entity> entities = new List<Entity>();
            List<List<int2>> pathes = new List<List<int2>>();

            List<Squarea> squareas = new List<Squarea>();
            Entities
                .WithoutBurst()
                .ForEach((in Squarea s) =>
                {
                    squareas.Add(s);
                }).Run();

            Entities
                .WithAll<Player>()
                .WithoutBurst()
                .ForEach((Entity entity, ref CharacterState state, ref ElapsedTime time, ref MoveData moveData) =>
                {
                    if(state.Value == CharacterStates.Wait)
                    {
                        if(time.Value >= WAIT_TIME_LIMIT)
                        {
                            float randomF = random.NextFloat();
                            if(randomF > 0.5f)
                            {
                                int2 start = moveData.goal;
                                int2 newGoal = GenerateRandomPos();
                                
                                var aStar = new AStar(start, newGoal, squareas);
                                List<int2> path = aStar.GetPath();
                                // (Entity, List<int2>) data = (entity, path);
                                // datas.Add(data);
                                if(path.Count > 0)
                                {
                                    state.Value = CharacterStates.Move;
                                    moveData.goal = newGoal;
                                    entities.Add(entity);
                                    pathes.Add(path);
                                }
                            }
                            time.Value = 0;
                        }
                        else
                            time.Value += dt;
                    }
                }).Run();
            // foreach((Entity entity,List<int2> path) data in datas)
            // {
            //     var buffer = GetBuffer<MovePoints>(data.entity);
            //     foreach(int2 point in data.path)
            //     {
            //         float3 p = RoomPos.P2W(point);
            //         buffer.Add(new MovePoints{
            //             Value = p
            //         });
            //     }
                
            // }
            for(int i=0;i<entities.Count;i++)
            {
                var buffer = GetBuffer<MovePoints>(entities[i]);
                foreach(int2 point in pathes[i])
                {
                    float3 p = RoomPos.P2W(point);
                    buffer.Add(new MovePoints{
                        Value = p
                    });
                }
            }
        }

        private int2 GenerateRandomPos()
        {
            int x = random.NextInt(0,ROOM_SIZE-1);
            int y = random.NextInt(0,ROOM_SIZE-1);
            return new int2(x,y);
        }

        
    }
    
}
