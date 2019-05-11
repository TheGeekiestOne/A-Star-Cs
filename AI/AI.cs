
using NLua;
using System.Collections.Generic;


//-----------------------------------------------------------------------------
// Created by: Ayran Olckers AKA The Geekiest One
// -2019-
// -Game Development Project-
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
//-----------------------------------------------------------------------------

/// <summary>
/// 
/// 
/// AI manager to get states and set states for various entities
/// 
/// 
///// Description
//Artemis is a high performance Entity System framework for games, originally written in Java by Arni Arent and Tiago Costa, now ported to C#. 
//Its goal is to provide a clean API to abstract your game world organization into entities, components and systems.
//Artemis has no dependencies (for PC, in Xbox and Windows Phone 7 we have one) and can be used with any game framework or library, 2D or 3D, and even multiplatform, if you use it with Mono/MonoTouch/Mono4Android.
/// </summary>

namespace Lost_Island_Ranal.ECS
{
    class AI : Component
    {
        public LuaTable Behavior { get; }
        public float Timer { get; set; } = 0;
        public float Target_Angle { get; set; } = 0;

        public List<bool> Flags { get; set; }

        public bool TargetEntity_AStar { get; set; } = false;
        public Entity Target;

        public AI(Entity target) : base(Types.AI)
        {
            Target = target;
        }



   ////     if body and anim and physics then

   ////         local player = engine:Get_Player()
			////if player then


   ////             local p_body = engine:Get_Component(player, "Body")
			////	if p_body then

   ////                 local dist = engine:Dist(body.X, body.Y, p_body.X, p_body.Y)
			////		if (dist<mx_dist) then

   ////                     anim.Current_Animation_ID = "ranged-attack";
			////		else
			////			anim.Current_Animation_ID = "ranged-idle";
			////		end
   ////             end

			////	if anim.Current_Frame == 9 - 1 and fn.Table.shoot_timer <= 0 then
   ////                 fn.Table.shoot_timer = 1


   ////                 local bullet = engine:Spawn(shulk_bullet, body.Center.X - 4, body.Center.Y - 4)

   ////                 local bphysics = engine:Get_Component(bullet, "Physics")

   ////                 local bbody = engine:Get_Component(bullet, "Body")


   ////                 local angle = math.atan2(
   ////                     p_body.Y - bbody.Y,
   ////                     p_body.X - bbody.X
   ////                 )


   ////                 bphysics.DestroyOnCollision = true;

			////		bphysics.Vel_X = math.cos(angle) * 100
			////		bphysics.Vel_Y = math.sin(angle) * 100
			////		bphysics.Friction = 1
			////	end

   ////             local dt = engine:Get_DT()

   ////             fn.Table.shoot_timer = fn.Table.shoot_timer - dt

   ////             handle_player_hit(entity, engine, 1, 100)

        //Calls from the LUA Ai File.
        public AI(LuaTable behavior) : base(Types.AI)
        {
            Behavior = behavior;
            Flags = new List<bool>();
            for (int i = 0; i < 16; i++)
                Flags.Add(false);
        }
    }
}
