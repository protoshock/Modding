DatGamerboi101
datgamerboi101
Home and tires (-8)

BracketProto — 2023-11-07 21:41
@revive follow script (but actually correct this time)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BracketProto.Modding;
using UnityEngine.AI;
public class Follower : CustomMod
Expandera
Follower.cs
3 KB
BracketProto — 2023-11-08 17:36
@revive updated follow script (with smoothing)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BracketProto.Modding;
using UnityEngine.AI;
public class Follower : CustomMod
Expandera
Follower.cs
4 KB
BracketProto — 2024-01-03 02:08
everything about outdated lol
anyway
v8 assets of BlockBuilding
Bifogad filtyp: archive
ModAssets.zip
39.46 MB
﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BracketProto.Modding;
using UnityEngine.AI;
public class Follower : CustomMod
{
    private GameObject player;
    public float speed;
    public float dist;
    private NavMeshAgent agent;
    private Vector3 NetworkPos;
    private Vector3 NetworkRot;
    public class MoveParams
    {
        public Vector3 pos;
        public Vector3 rot;
    }
    void ModStart()
    {
        try
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            InvokeRepeating("SyncPos", 0, .03f);
            InvokeRepeating("FindPlayer", 0, 1);
            agent.speed = speed;
            agent.angularSpeed = speed;
        }
        catch { }
    }

    void SyncPos()
    {
        if (hosting)
        {
            MoveParams moveParams = new MoveParams();
            moveParams.pos = transform.position;
            moveParams.rot = transform.rotation.eulerAngles;



            ModRPC RPC = new ModRPC();
            RPC.modrpctype = "syncmodpos";
            RPC.target = "All";
            RPC.parameters = JsonUtility.ToJson(moveParams);
            SendModRPC(viewid, RPC);
        }
    }
    public void RecieveModRPC(ModRPC RPC)
    {
        try
        {
            if (RPC.modrpctype == "syncmodpos" && RPC.mod_rpc_sender == viewid && !hosting)
            {
                MoveParams moveParams = JsonUtility.FromJson<MoveParams>(RPC.parameters);
                NetworkPos = moveParams.pos;
                NetworkRot = moveParams.rot;
            }
        }
        catch
        {

        }
    }

    void FindPlayer()
    {
        if (hosting)
        {
            GameObject NearestPlayer = GetNearestPlayer(transform.position);
            player = NearestPlayer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hosting)
        {
            if (Vector3.Distance(NetworkPos, transform.position) > 15)
            {
                transform.position = NetworkPos;
                transform.rotation = Quaternion.Euler(NetworkRot);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, NetworkPos, 5 * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(NetworkRot), 5 * Time.deltaTime);
            }
        }
        if (!hosting) return;
        try
        {
            if (player != null)
            {
                Quaternion lookrot = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                Vector3 eulerlook = lookrot.eulerAngles;
                eulerlook.x = 0;
                eulerlook.z = 0;
                transform.rotation = Quaternion.Euler(eulerlook);



                if (Vector3.Distance(player.transform.position, transform.position) > dist)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    agent.isStopped = true;
                }
            }
        }
        catch
        {

        }
    }
}
Follower.cs
4 KB
