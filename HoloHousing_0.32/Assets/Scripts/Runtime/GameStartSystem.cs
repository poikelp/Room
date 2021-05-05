using Unity.Entities;
using Unity.Tiny.Input;
using Housing.Component;

namespace Housing
{
    public class GameStartSystem : SystemBase
    {
        // private bool touch;
        // protected override void OnCreate()
        // {
        //     base.OnCreate();
        //     RequireSingletonForUpdate<Player>();
        // }

        protected override void OnUpdate()
        {
            // var state = GetSingleton<GameState>();
            // var stateEntity = GetSingletonEntity<GameState>();
            // if(state.Value == GameStates.Start)
            // {
            //     bool push = false;
            //     var Input = World.GetExistingSystem<InputSystem>();
            //     if(Input.IsTouchSupported() && touch &&/*Input.TouchCount() > 0*/ Input.GetTouch(0).phase == TouchState.Ended)
            //     {
            //         touch = false;
            //         push = true;
            //     }
            //     if(Input.GetMouseButtonUp(0))
            //     {
            //         push = true;
            //     }
            //     if(Input.IsTouchSupported() && Input.GetTouch(0).phase == TouchState.Began)
            //     {
            //         touch = true;
            //     }

            //     if(push)
            //     {
            //         state.Value = GameStates.InGame;
            //         EntityManager.SetComponentData<GameState>(stateEntity, state);

            //         // var playerEntity = GetSingletonEntity<Player>();
            //         // var animData = GetComponent<SpriteAnimationData>(playerEntity); 
            //         // animData.AnimType = AnimationType.Run;
            //         // animData.ElapsedTime = 0;
            //         // EntityManager.SetComponentData<SpriteAnimationData>(playerEntity, animData);
            //     }
            // }
        }
    }
}
