﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector2 _position = _packet.ReadVector2();
        float _rotation = _packet.ReadFloat();

        Debug.Log("Tried to spawn a player");

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);

    }

    public static void SpawnAnimal(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _species = _packet.ReadString();
        Vector2 _position = _packet.ReadVector2();
        float _rotation = _packet.ReadFloat();

        GameManager.instance.SpawnAnimal(_id, _species, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector2 _position = _packet.ReadVector2();
        float _rotation = _packet.ReadFloat();
        int _health = _packet.ReadInt();
        bool _attack = _packet.ReadBool();
        bool _spectating = _packet.ReadBool();

        Transform player = GameManager.players[_id].transform;
        player.transform.position = _position;
        player.GetComponent<PlayerManager>().rotation = _rotation;
        player.GetComponent<PlayerManager>().health = _health;
        player.GetComponent<PlayerManager>().attack = _attack;
        if (_spectating && !player.GetComponent<PlayerManager>().spectate)
        {
            player.GetComponent<PlayerManager>().spectate = _spectating;
            player.GetComponent<PlayerManager>().setDeadSprite();
        }

    }

    public static void AnimalData(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _species = _packet.ReadString();
        Vector2 _position = _packet.ReadVector2();
        float _rotation = _packet.ReadFloat();
        int _health = _packet.ReadInt();

        if (!GameManager.animals.ContainsKey(_id))
        {
            Debug.LogWarning("Animal " + _id + " does not exist!");
        }
        else if (_species != GameManager.animals[_id].species)
        {
            Debug.LogWarning("Animal " + _id + " does not match species");
        }
        else
        {
            GameManager.animals[_id].transform.position = _position;
            GameManager.animals[_id].GetComponent<AnimalManager>().rotation = _rotation;
            GameManager.animals[_id].GetComponent<AnimalManager>().health = _health;
        }
    }

    public static void InitializeGame(Packet _packet)
    {
        int length = _packet.ReadInt();
        for (int i = 0; i < length; i++)
        {
            int id = _packet.ReadInt();
            short x = _packet.ReadShort();
            short y = _packet.ReadShort();
            if (id != -1)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().spawnTree(id, x, y);
            }
        }
        length = _packet.ReadInt();
        for (int i = 0; i < length; i++)
        {
            int id = _packet.ReadInt();
            short x = _packet.ReadShort();
            short y = _packet.ReadShort();
            if (id != -1)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().spawnRock(id, x, y);
            }
        }
    }

    public static void UpdateHp(Packet _packet)
    {
        string _type = _packet.ReadString();
        int _id = _packet.ReadInt();
        int _hp = _packet.ReadInt();
        if (_type == "tree")
        {
            GameManager.trees[_id].hp = _hp;
        }
        else if (_type == "rock")
        {
            GameManager.rocks[_id].hp = _hp;
        }
        else if (_type == "animal")
        {
            GameManager.animals[_id].health = _hp;
        }
    }

    public static void UpdateInventory(Packet _packet)
    {
        int _wood = _packet.ReadInt();
        int _rock = _packet.ReadInt();
        int _meat = _packet.ReadInt();

        PlayerController player = GameManager.players[Client.instance.myId].GetComponent<PlayerController>();

        player.inventory["wood"] = _wood;
        player.inventory["rock"] = _rock;
        player.inventory["meat"] = _meat;
    }
}
