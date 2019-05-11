using System;
using Microsoft.Xna.Framework;
using static Lost_Island_Ranal.ECS.Component;
using NLua;

//-----------------------------------------------------------------------------
// Created by: Ayran Olckers AKA The Geekiest One
// -2019-
// -Game Development Project-
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
//-----------------------------------------------------------------------------


    /// <summary>
    /// 
    /// The Entity Component System offers a better approach to game design that allows you to concentrate on the actual problems you are solving: the data and behavior that make up your game. It leverages the C# Job System and Burst Compiler enabling you to take full advantage of today's multicore processors. Moving from object-oriented to data-oriented design makes it easier for you to reuse the code and easier for others to understand and work on it.

    ///The Entity Component System ships as an experimental package that currently supports Unity 2018.3 and later.It is important to stress that the Entity Component System is not production ready.
    /// 
    /// 
    /// 
    /// </summary>

namespace Lost_Island_Ranal.ECS
{
    class AI_System : System
    {
        private Tiled_Map tiled_map;

        public AI_System() : base(Types.AI, Types.Body, Types.Physics)
        {
        }

        public void Give_Map(Tiled_Map map) => this.tiled_map = map;
        public void Do_Block(LuaTable table, Entity e, AI ai, Body body, Physics physics)
        {
            int i = 2;
            bool running = true;
            while (running)
            {
                if (i > table.Values.Count) { running = false; break; }

                var key = table[i] as LuaTable;
                var opcode = key[1] as string;

                //Switch Statement to change how an entitiy works
                switch (opcode)
                {
                    case "set_z":
                        {
                            var to = (float)(key[2] as Double?);
                            body.Z = to;
                            break;
                        }
                    case "set_layer":
                        {
                            var to = (float)(key[2] as Double?);
                            var sprite = (Animated_Sprite)e.Get(Types.Animation);
                            sprite.Layer = to;
                            break;
                        }
                    case "entity_within":
                        {
                            var tag = key[2] as string;
                            var dist = (float)(key[3] as double?);

                            var other = World_Ref.Find_With_Tag(tag);
                            if (other != null)
                            {
                                var o_body = (Body)other.Get(Types.Body);
                                var adist = Vector2.Distance(o_body.Position, body.Position);
                                if (adist > dist) i++;
                            }
                            break;
                        }
                    case "track":
                        {
                            var tag = key[2] as string;
                            var other = World_Ref.Find_With_Tag(tag);
                            if (other != null)
                            {
                                var o_body = (Body)other.Get(Types.Body);
                                var dot = body.Angle_To_Other(o_body);
                                var force = (float)(key[3] as double?);
                                physics.Apply_Force(force, dot);
                            }

                            i++;
                            break;
                        }
                    case "block":
                        Do_Block(key, e, ai, body, physics);
                        break;
                    case "block_with_skip":
                        Do_Block(key, e, ai, body, physics); i++;
                        break;
                    case "if_timer":
                        var time = (float)(key[2] as double?);
                        if (ai.Timer < time)
                            i++;
                        break;
                    case "timer_reset": ai.Timer = 0; break;
                    case "new_target_dir":
                        {
                            var by = (float)(key[2] as double?);
                            var rnd = new Random();
                            var dir = (rnd.Next() % (int)by);
                            ai.Target_Angle += (float)((dir * Math.PI) / 180);
                            break;
                        }
                    case "move_towards_target":
                        {
                            var force = (float)(key[2] as double?);
                            physics.Apply_Force(force, ai.Target_Angle);
                            break;
                        }
                    case "collision_with_tag":
                        {
                            var tag = key[2] as string;
                            if (physics.Other != null)
                            {
                                if (!physics.Other.Tags.Contains(tag)) {
                                    i++;
                                }
                                continue;
                            }
                            i++;
                            break;
                        }
                    case "destroy":
                        {
                            var obj = key[2] as string;
                            if (obj == "self")
                                e.Destroy();
                            if (obj == "other")
                                if (physics.Other != null)
                                    physics.Other.Destroy();
                            break;
                        }
                    case "face_move_dir": {
                            var sprite = (Animated_Sprite)e.Get(Types.Animation);
                            if (sprite != null)
                            {

                                if (physics.Velocity.X > 0)
                                    sprite.Scale = new Vector2(1, sprite.Scale.Y);
                                else
                                    sprite.Scale = new Vector2(-1, sprite.Scale.Y);

                            }
                            break;
                        }
                    case "set_animation": {
                            var sprite = (Animated_Sprite)e.Get(Types.Animation);
                            if (sprite != null) {
                                var anim_id = key[2] as string;
                                sprite.Current_Animation_ID = anim_id;
                            }
                            break;
                        }
                    case "velocity_lessthan": {
                            var value = (float)(key[2] as double?);
                            if (value < physics.Current_Speed) {
                                i++;
                            }
                            break;
                        }
                    case "velocity_greaterthen": {
                            var value = (float)(key[2] as double?);
                            
                            if (value < physics.Current_Speed)
                            {
                                i++;
                            }
                            break;
                        }
                    case "set_target": {
                            var to = (float)(key[2] as double?);
                            ai.Target_Angle = (float)((to * Math.PI) / 180);
                        }
                        break;
                    case "print_str":
                        Console.WriteLine("AI: {}", key[2] as string);
                        break;
                    case "set_flag":
                        ai.Flags[(int)(key[2] as double?)] = true;
                        break;
                    case "unset_flag":
                        ai.Flags[(int)(key[2] as double?)] = false;
                        break;
                    case "if_flag": {
                            var flag = ai.Flags[(int)(key[2] as double?)];
                            if (!flag) { i++; }
                            break;
                        }
                    case "if_not_flag":
                        {
                            var flag = ai.Flags[(int)(key[2] as double?)];
                            if (flag) { i++; }
                            break;
                        }
                    default:
                        Console.WriteLine($"Unknown command: { key[1] }");
                        break;
                }
                i++;
            }
        }

        public override void Load(Entity entity)
        {
            var ai = (AI)entity.Get(Types.AI);
            var body = (Body)entity.Get(Types.Body);
        }

        public override void Update(GameTime time, Entity entity)
        {
            var ai = (AI)entity.Get(Types.AI);
            var body = (Body)entity.Get(Types.Body);
            var physics = (Physics)entity.Get(Types.Physics);

            if (ai.TargetEntity_AStar == false)
            {
                var behavior = ai.Behavior;

                Do_Block(behavior, entity, ai, body, physics);
                ai.Timer += (float)time.ElapsedGameTime.TotalSeconds;
            } else
            {
                // A Star baby!                


            }
            //Console.WriteLine(ai.Timer);
        }
    }
}
