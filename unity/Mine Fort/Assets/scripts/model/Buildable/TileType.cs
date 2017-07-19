﻿#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software,
// and you are welcome to redistribute it under certain conditions; See
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion
using System;
using System.Collections.Generic;
using System.Xml;
using MoonSharp.Interpreter;
using Rimworld.Jobs;
using Rimworld.OrderActions;
using Rimworld.model;

[MoonSharpUserData]
public class TileType : TagObject, IPrototypable, IEquatable<TileType>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TileType"/> class.
    /// </summary>
    public TileType()
    {
        PathfindingModifier = 0.0f;
        PathfindingWeight = 1.0f;
    }

    #region MeuCodigo
    public string fileName = "undef";
    public string name;

    internal void LoadFromCSV(string[] lineData)
    {
        TileType td = this;
        int i = 0;
        td.name = lineData[i++];
        td.fileName = lineData[i++];
        td.BaseMovementCost = float.Parse(lineData[i++]);
        string[] tags = lineData[i++].Split(',');
        foreach (string tag in tags)
        {
            td.AddTag(tag);
        }
    }
    #endregion MeuCodigo

    /// <summary>
    /// Gets the empty tile type prototype.
    /// </summary>
    /// <value>The empty tile type.</value>
    public static TileType Empty
    {
        get { return PrototypeManager.TileType.Get("empty"); }
    }

    /// <summary>
    /// Gets the floor tile type prototype.
    /// </summary>
    /// <value>The floor tile type.</value>
    public static TileType Floor
    {
        get { return PrototypeManager.TileType.Get("floor"); }
    }

    /// <summary>
    /// Unique TileType identifier.
    /// </summary>
    /// <value>The tile type.</value>
    public string Type { get; private set; }

    /// <summary>
    /// Gets the base movement cost.
    /// </summary>
    /// <value>The base movement cost.</value>
    public float BaseMovementCost { get; private set; }

    /// <summary>
    /// Gets or sets the TileType's pathfinding weight which is multiplied into the Tile's final PathfindingCost.
    /// </summary>
    /// <value>The pathfinding weight.</value>
    public float PathfindingWeight { get; private set; }

    /// <summary>
    /// Gets or sets the TileType's pathfinding modifier which is added into the Tile's final PathfindingCost.
    /// </summary>
    /// <value>The pathfinding modifier.</value>
    public float PathfindingModifier { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="TileType"/> links to neighbours.
    /// </summary>
    /// <value><c>true</c> if links to neighbours; otherwise, <c>false</c>.</value>
    public bool LinksToNeighbours { get; private set; }

    /// <summary>
    /// Gets the lua function that can be called to determine if this instance can build here lua.
    /// </summary>
    /// <value>The name of the lua function.</value>
    public string CanBuildHereLua { get; private set; }

    /// <summary>
    /// Gets the localization code.
    /// </summary>
    /// <value>The localization code.</value>
    public string LocalizationCode { get; private set; }

    /// <summary>
    /// Gets the localized description.
    /// </summary>
    /// <value>The localized description.</value>
    public string UnlocalizedDescription { get; private set; }

    /// <summary>
    /// The order action to create tileType.
    /// </summary>
    public Dictionary<string, OrderAction> OrderActions { get; private set; }

    public static bool operator ==(TileType left, TileType right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TileType left, TileType right)
    {
        return !Equals(left, right);
    }

    public bool Equals(TileType other)
    {
        return string.Equals(Type, other.Type);
    }

    public override bool Equals(object obj)
    {
        return Equals((TileType)obj);
    }

    public override int GetHashCode()
    {
        return Type != null ? Type.GetHashCode() : 0;
    }

    public override string ToString()
    {
        return Type;
    }

    public T GetOrderAction<T>() where T : OrderAction
    {
        OrderAction orderAction;
        if (OrderActions.TryGetValue(typeof(T).Name, out orderAction))
        {
            return (T)orderAction;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Determines whether this tile type is allowed to be built on the given tile.
    /// </summary>
    /// <returns><c>true</c> if the ile type is allowed to be built on the given tile; otherwise, <c>false</c>.</returns>
    /// <param name="tile">The tile to build on.</param>
    public bool CanBuildHere(Tile tile)
    {
        if (CanBuildHereLua == null)
        {
            return true;
        }

        DynValue value = FunctionsManager.TileType.Call(CanBuildHereLua, tile);
        if (value != null)
        {
            return value.Boolean;
        }

        UnityDebugger.Debugger.Log("Lua", "Found no lua function " + CanBuildHereLua);
        return false;
    }

    /// <summary>
    /// Reads the prototype from the specified XML reader.
    /// </summary>
    /// <param name="parentReader">The XML reader to read from.</param>
    public void ReadXmlPrototype(XmlReader parentReader)
    {
        Type = parentReader.GetAttribute("type");
        OrderActions = new Dictionary<string, OrderAction>();

        XmlReader reader = parentReader.ReadSubtree();
        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "BaseMovementCost":
                    reader.Read();
                    BaseMovementCost = reader.ReadContentAsFloat();
                    break;
                case "PathfindingModifier":
                    reader.Read();
                    PathfindingModifier = reader.ReadContentAsFloat();
                    break;
                case "PathfindingWeight":
                    reader.Read();
                    PathfindingWeight = reader.ReadContentAsFloat();
                    break;
                case "LinksToNeighbours":
                    reader.Read();
                    LinksToNeighbours = reader.ReadContentAsBoolean();
                    break;
                case "OrderAction":
                    OrderAction orderAction = OrderAction.Deserialize(reader);
                    if (orderAction != null)
                    {
                        OrderActions[orderAction.Type] = orderAction;
                    }

                    break;
                case "CanPlaceHere":
                    CanBuildHereLua = reader.GetAttribute("functionName");
                    break;
                case "LocalizationCode":
                    reader.Read();
                    LocalizationCode = reader.ReadContentAsString();
                    break;
                case "UnlocalizedDescription":
                    reader.Read();
                    UnlocalizedDescription = reader.ReadContentAsString();
                    break;
            }
        }
    }
}