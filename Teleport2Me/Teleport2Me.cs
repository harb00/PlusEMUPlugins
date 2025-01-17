﻿using Plus.Plugins;
using Plus.HabboHotel.Rooms.Chat.Commands;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

namespace Teleport2Me;

public class Teleport2Me : IPlugin
{
    
    public void Start()
    {
        var pluginInfo = new Teleport2MeDefinition();
        Logger(pluginInfo.Name + " by " + pluginInfo.Author + " has started.");
    }

    private void Logger(string message) {
        var pluginInfo = new Teleport2MeDefinition();
        var CYAN = "\u001b[34m";
        var WHITE = "\u001b[37m";
        Console.WriteLine(WHITE + "[" + CYAN + pluginInfo.Name + WHITE + "] " + message);    }

}


public class Teleport2MeCommand : ITargetChatCommand
{
    public string Key => "tptome";

    public string PermissionRequired => "tp2me_plugin";

    public string Parameters => "%target% (force)";

    public string Description => "Teleports the specified user infront of you.";

    public bool MustBeInSameRoom => true;

    public Task Execute(GameClient session, Room room, Habbo target, string[] parameters)
    {
        var actorRoomUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
        var targetRoomUser = room.GetRoomUserManager().GetRoomUserByHabbo(target.Id);

        var tileInfrontActor = actorRoomUser.SquareInFront;
        var itemsInfrontActor = room.GetRoomItemHandler().GetFurniObjects(tileInfrontActor.X, tileInfrontActor.Y);

        if (target == session.GetHabbo()) {
            session.SendWhisper("Get a life.");
            return Task.CompletedTask;
        }

        if (session.GetHabbo().Rank < target.Rank) {
            session.SendWhisper("Target user has a higher rank than you.");
            return Task.CompletedTask;
        }

        if (parameters.Length > 1) {
            session.SendWhisper("Wrong syntax. Use :tp2me %target% (optionally) force");
            return Task.CompletedTask;
        }

        if (itemsInfrontActor.Count > 0 && parameters.Length == 0) {
            session.SendWhisper("Blocked by furniture. Force teleportation with - :tptome %target% force");
            return Task.CompletedTask;
        }

        if (parameters.Length == 0) {
            teleportUser(room, targetRoomUser, actorRoomUser, tileInfrontActor);
            return Task.CompletedTask;
        }

        if (parameters[0] == "force") {
            teleportUser(room, targetRoomUser, actorRoomUser, tileInfrontActor);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }


    private void teleportUser(Room room, RoomUser targetRoomUser, RoomUser actorRoomUser, System.Drawing.Point tileInfrontActor) {
            room.GetGameMap().UpdateUserMovement(targetRoomUser.Coordinate, tileInfrontActor, targetRoomUser);
            targetRoomUser.SetPos(tileInfrontActor.X, tileInfrontActor.Y, actorRoomUser.Z);
            targetRoomUser.UpdateNeeded = true;
    }
}
