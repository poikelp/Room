using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny;
using Housing.Component;

namespace Housing
{
    [UpdateInGroup(typeof(CharacterSystemGroup))]
    public class CharacterMoveSystem : SystemBase
    {
        private const int ROOM_SIZE = 16;

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;
            List<Entity> moveEntities = new List<Entity>();
            Entities
                .WithAll<Player>()
                .WithoutBurst()
                .ForEach((Entity entity, ref CharacterState state) => 
                {
                    if(state.Value == CharacterStates.Move)
                    {
                      moveEntities.Add(entity);  
                    }
                }).Run();
            foreach(Entity entity in moveEntities)
            {
                var data = GetComponent<MoveData>(entity);
                var points = GetBuffer<MovePoints>(entity);
                var trans = GetComponent<Translation>(entity);
                var rotation = GetComponent<Rotation>(entity);
                var render = GetComponent<Renderer2D>(entity);

                float3 next = points[data.nextPointIndex].Value;
                float3 vector = next - trans.Value;
                Normalize(ref vector);

                float3 velocity = vector * dt;
                trans.Value += velocity;
                SetComponent<Translation>(entity, trans);

                int2 p = RoomPos.W2P(trans.Value);
                short order = (short)(100 - trans.Value.y*10);
                render.OrderInLayer = order;
                SetComponent<Renderer2D>(entity, render);

                if(vector.x < 0)
                {
                    rotation.Value = quaternion.EulerXYZ(0,math.PI,0);
                }
                else
                {
                    rotation.Value = quaternion.EulerXYZ(0,0,0);
                }
                SetComponent<Rotation>(entity, rotation);

                if(math.distance(trans.Value,next) <= 0.1f)
                {
                    data.nextPointIndex++;
                    if(data.nextPointIndex >= points.Length)
                    {
                        data.nextPointIndex = 0;

                        points.Clear();

                        var state = GetComponent<CharacterState>(entity);
                        state.Value = CharacterStates.Wait;
                        SetComponent<CharacterState>(entity, state);

                        var elapsed = GetComponent<ElapsedTime>(entity);
                        elapsed.Value = 0;
                        SetComponent<ElapsedTime>(entity, elapsed);
                    }
                    SetComponent<MoveData>(entity,data);
                }
            }
        }

        private void Normalize(ref float3 v)
        {
            float length = math.sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
            v /= length;
        }
    }
    
}
