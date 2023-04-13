using System.Collections.Generic;
using TrueSync;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Room)]
    [FriendOf(typeof (RoomServerComponent))]
    public class C2Room_ChangeSceneFinishHandler: AMActorHandler<Scene, C2Room_ChangeSceneFinish>
    {
        protected override async ETTask Run(Scene roomScene, C2Room_ChangeSceneFinish message)
        {
            RoomServerComponent roomServerComponent = roomScene.GetComponent<RoomServerComponent>();
            RoomPlayer roomPlayer = roomScene.GetComponent<RoomServerComponent>().GetChild<RoomPlayer>(message.PlayerId);
            roomPlayer.IsJoinRoom = true;
            roomServerComponent.AlreadyJoinRoomCount++;

            if (roomServerComponent.AlreadyJoinRoomCount <= ConstValue.MatchCount)
            {
                // 通知给已加进来的客户端每个玩家的进度
            }

            if (roomServerComponent.AlreadyJoinRoomCount == ConstValue.MatchCount)
            {
                await TimerComponent.Instance.WaitAsync(1000);

                Room2C_EnterMap room2CEnterMap = new Room2C_EnterMap() {UnitInfo = new List<LockStepUnitInfo>()};
                foreach (var kv in roomServerComponent.Children)
                {
                    room2CEnterMap.UnitInfo.Add(new LockStepUnitInfo()
                    {
                        PlayerId = kv.Key, 
                        Position = new TSVector(10, 0, 10), 
                        Rotation = TSQuaternion.identity
                    });
                }
                
                roomScene.GetComponent<BattleComponent>().InitUnit(room2CEnterMap.UnitInfo);

                RoomMessageHelper.BroadCast(roomScene, room2CEnterMap);
            }

            await ETTask.CompletedTask;
        }
    }
}